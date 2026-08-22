using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Korp.Faturamento.Domain.Interfaces;
using Korp.Faturamento.Domain.Messages;

namespace Korp.Faturamento.Infrastructure.Messaging.Publishers
{
    public class RabbitMqPublisher : IProcessamentoNotaPublisher
    {
        private readonly RabbitMqConnection _connection;
        private readonly RabbitMqSettings _settings;

        public RabbitMqPublisher(
            RabbitMqConnection connection,
            IOptions<RabbitMqSettings> options)
        {
            _connection = connection;
            _settings = options.Value;
        }

        public async Task PublicarProcessamentoNotaAsync(
            ProcessarNotaFiscalMessage message)
        {
            await using var channel =
                await _connection.CreateChannelAsync();

            var json =
                JsonSerializer.Serialize(message);

            var body =
                Encoding.UTF8.GetBytes(json);

            var properties =
                new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };

            await channel.BasicPublishAsync(
                exchange: _settings.ProcessarNotaExchange,
                routingKey: _settings.ProcessarNotaRoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }
    }
}
