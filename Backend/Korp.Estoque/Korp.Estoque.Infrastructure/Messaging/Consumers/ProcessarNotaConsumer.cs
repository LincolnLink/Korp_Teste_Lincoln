using System.Text;
using System.Text.Json;
using Korp.Estoque.Domain.Interfaces;
using Korp.Estoque.Domain.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Korp.Estoque.Infrastructure.Messaging.Consumers
{
    public class ProcessarNotaConsumer : BackgroundService
    {
        private const int MaxRetries = 3;

        private readonly RabbitMqConnection _connection;
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProcessarNotaConsumer> _logger;

        private IChannel? _channel;

        public ProcessarNotaConsumer(
            RabbitMqConnection connection,
            IOptions<RabbitMqSettings> options,
            IServiceScopeFactory scopeFactory,
            ILogger<ProcessarNotaConsumer> logger)
        {
            _connection = connection;
            _settings = options.Value;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _connection.CreateChannelAsync();

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();

                ProcessarNotaFiscalMessage? message = null;

                try
                {
                    var json = Encoding.UTF8.GetString(body);

                    message = JsonSerializer.Deserialize<ProcessarNotaFiscalMessage>(json);

                    if (message is null)
                    {
                        _logger.LogWarning("Mensagem inválida recebida do RabbitMQ.");

                        await PublicarDlqAsync(body);

                        await _channel.BasicAckAsync( deliveryTag: eventArgs.DeliveryTag, multiple: false);

                        return;
                    }

                    _logger.LogInformation("Processando nota fiscal {NotaFiscalId}.", message.NotaFiscalId);

                    using var scope = _scopeFactory.CreateScope();

                    var processamentoService = scope.ServiceProvider
                        .GetRequiredService<IProcessamentoEstoqueService>();

                    var resultadoPublisher = scope.ServiceProvider
                        .GetRequiredService<IResultadoNotaPublisher>();

                    // Valida produtos, saldo e realiza a baixa.
                    await processamentoService.ProcessarNotaAsync(message);

                    // Informa ao Faturamento que o processamento deu certo.
                    await resultadoPublisher.PublicarAsync(new ResultadoProcessamentoNotaMessage
                    {
                        NotaFiscalId = message.NotaFiscalId,
                        Sucesso = true,
                        Mensagem ="Estoque processado com sucesso."
                    });

                    // Somente depois de tudo dar certo,
                    // confirmamos a mensagem para o RabbitMQ.
                    await _channel.BasicAckAsync( deliveryTag: eventArgs.DeliveryTag, multiple: false);

                    _logger.LogInformation( "Nota fiscal {NotaFiscalId} processada com sucesso.", message.NotaFiscalId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem de estoque.");

                    var retryCount = ObterRetryCount(eventArgs.BasicProperties);

                    if (retryCount < MaxRetries)
                    {
                        var proximaTentativa = retryCount + 1;

                        _logger.LogWarning(
                            "Nota {NotaFiscalId} falhou. " +
                            "Enviando para Retry. Tentativa {Tentativa}/{MaxRetries}.",
                            message?.NotaFiscalId,
                            proximaTentativa,
                            MaxRetries);

                        // Publica uma nova cópia na fila de Retry.
                        await PublicarRetryAsync(body, proximaTentativa);

                        // A mensagem ORIGINAL pode ser confirmada,
                        // pois agora existe uma cópia na fila Retry.
                        await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);

                        return;
                    }

                    _logger.LogError(
                        "Nota {NotaFiscalId} atingiu o limite de {MaxRetries} tentativas. " +
                        "Enviando para DLQ.",
                        message?.NotaFiscalId,
                        MaxRetries);

                    // Depois de esgotar as tentativas,
                    // enviamos para a DLQ.
                    await PublicarDlqAsync(body);

                    // Agora avisamos o Faturamento da falha definitiva.
                    if (message is not null)
                    {
                        await PublicarResultadoFalhaAsync(
                            message,
                            ex.Message);
                    }

                    // Confirma a mensagem original.
                    await _channel.BasicAckAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.ProcessarNotaQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation( "Consumer iniciado. Escutando a fila {Queue}.", _settings.ProcessarNotaQueue);

            await Task.Delay( Timeout.Infinite, stoppingToken);
        }

        private async Task PublicarRetryAsync( byte[] body, int retryCount)
        {
            if (_channel is null) return;

            var properties = new BasicProperties 
            {
                Persistent = true,
                ContentType = "application/json",

                Headers = new Dictionary<string, object?>
                {
                    {
                        "x-retry-count",
                        retryCount
                    }
                }
            };

            await _channel.BasicPublishAsync(
                exchange: _settings.RetryExchange,
                routingKey: _settings.RetryRoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);

            _logger.LogInformation( "Mensagem enviada para a fila de Retry. Tentativa {RetryCount}.", retryCount);
        }

        private async Task PublicarDlqAsync(byte[] body)
        {
            if (_channel is null) return;

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await _channel.BasicPublishAsync(
                exchange: _settings.DeadLetterExchange,
                routingKey: _settings.DeadLetterRoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);

            _logger.LogWarning( "Mensagem enviada para a DLQ {Queue}.", _settings.DeadLetterQueue);
        }

        private async Task PublicarResultadoFalhaAsync( ProcessarNotaFiscalMessage message, string mensagemErro)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var resultadoPublisher = scope.ServiceProvider.GetRequiredService<IResultadoNotaPublisher>();

                await resultadoPublisher.PublicarAsync( new ResultadoProcessamentoNotaMessage
                {
                    NotaFiscalId = message.NotaFiscalId,

                    Sucesso = false,

                    Mensagem = mensagemErro
                });
            }
            catch (Exception publisherException)
            {
                _logger.LogError(
                    publisherException,
                    "Erro ao publicar resultado de falha da nota {NotaFiscalId}.",
                    message.NotaFiscalId);
            }
        }

        private static int ObterRetryCount(IReadOnlyBasicProperties properties)
        {
            if (properties.Headers is null) return 0;

            if (!properties.Headers.TryGetValue("x-retry-count", out var value))
            {
                return 0;
            }

            if (value is int intValue) return intValue;

            if (value is long longValue) return (int)longValue;

            if (value is byte[] bytes && int.TryParse(Encoding.UTF8.GetString(bytes), out var parsedValue))
            {
                return parsedValue;
            }

            return 0;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}