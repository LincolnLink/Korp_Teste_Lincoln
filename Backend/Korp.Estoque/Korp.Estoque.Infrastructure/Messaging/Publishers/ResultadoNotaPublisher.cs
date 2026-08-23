using Korp.Estoque.Domain.Interfaces;
using Korp.Estoque.Domain.Messages;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Korp.Estoque.Infrastructure.Messaging.Publishers
{
    public class ResultadoNotaPublisher : IResultadoNotaPublisher
    {
        private readonly RabbitMqConnection _connection;
        private readonly RabbitMqSettings _settings;

        public ResultadoNotaPublisher(
            RabbitMqConnection connection,
            IOptions<RabbitMqSettings> options)
        {
            _connection = connection;
            _settings = options.Value;
        }

        public async Task PublicarAsync(
            ResultadoProcessamentoNotaMessage message)
        {
            await using var channel =
                await _connection.CreateChannelAsync();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: _settings.ResultadoNotaExchange,
                routingKey: _settings.ResultadoNotaRoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }
    }
}
