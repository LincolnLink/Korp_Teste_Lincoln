using Korp.Faturamento.Domain.Interfaces;
using Korp.Faturamento.Domain.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Korp.Faturamento.Infrastructure.Messaging.Consumers
{
    public class ResultadoNotaConsumer : BackgroundService
    {
        private readonly RabbitMqConnection _connection;
        private readonly RabbitMqSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ResultadoNotaConsumer> _logger;

        private IChannel? _channel;

        public ResultadoNotaConsumer(
            RabbitMqConnection connection,
            IOptions<RabbitMqSettings> options,
            IServiceScopeFactory scopeFactory,
            ILogger<ResultadoNotaConsumer> logger)
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

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var message = JsonSerializer.Deserialize<ResultadoProcessamentoNotaMessage>(json);

                    if (message is null)
                    {
                        await _channel.BasicNackAsync( ea.DeliveryTag, false, false);
                        return;
                    }

                    using var scope = _scopeFactory.CreateScope();

                    var service = scope.ServiceProvider.GetRequiredService<IResultadoNotaService>();

                    await service.ProcessarResultadoAsync(message);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Erro ao processar resultado da nota.");

                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.ResultadoNotaQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay( Timeout.Infinite, stoppingToken);
        }
    }
}
