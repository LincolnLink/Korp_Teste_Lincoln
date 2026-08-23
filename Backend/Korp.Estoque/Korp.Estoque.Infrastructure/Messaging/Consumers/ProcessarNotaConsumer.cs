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

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
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
                ProcessarNotaFiscalMessage? message = null;

                try
                {
                    var body = eventArgs.Body.ToArray();

                    var json = Encoding.UTF8.GetString(body);

                    message = JsonSerializer.Deserialize<ProcessarNotaFiscalMessage>(json);

                    if (message is null)
                    {
                        _logger.LogWarning(
                            "Mensagem inválida recebida do RabbitMQ.");

                        await _channel.BasicNackAsync(
                            deliveryTag: eventArgs.DeliveryTag,
                            multiple: false,
                            requeue: false);

                        return;
                    }

                    using var scope = _scopeFactory.CreateScope();

                    var processamentoService = scope.ServiceProvider.GetRequiredService<IProcessamentoEstoqueService>();

                    var resultadoPublisher = scope.ServiceProvider.GetRequiredService<IResultadoNotaPublisher>();

                    await processamentoService.ProcessarNotaAsync(message);

                    await resultadoPublisher.PublicarAsync(
                        new ResultadoProcessamentoNotaMessage
                        {
                            NotaFiscalId = message.NotaFiscalId,
                            Sucesso = true,
                            Mensagem =
                                "Estoque processado com sucesso."
                        });

                    await _channel.BasicAckAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false);

                    _logger.LogInformation(
                        "Nota fiscal {NotaFiscalId} processada com sucesso.",
                        message.NotaFiscalId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Erro ao processar mensagem de estoque.");

                    if (message is not null)
                    {
                        try
                        {
                            using var scope = _scopeFactory.CreateScope();

                            var resultadoPublisher = scope.ServiceProvider.GetRequiredService<IResultadoNotaPublisher>();

                            await resultadoPublisher.PublicarAsync(
                                new ResultadoProcessamentoNotaMessage
                                {
                                    NotaFiscalId = message.NotaFiscalId,

                                    Sucesso = false,

                                    Mensagem = ex.Message
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

                    await _channel.BasicNackAsync(
                        deliveryTag: eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.ProcessarNotaQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }

        public override async Task StopAsync(
            CancellationToken cancellationToken)
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}