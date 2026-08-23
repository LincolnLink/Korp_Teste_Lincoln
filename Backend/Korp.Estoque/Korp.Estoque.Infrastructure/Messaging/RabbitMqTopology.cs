using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Korp.Estoque.Infrastructure.Messaging
{
    public class RabbitMqTopology
    {
        private readonly RabbitMqConnection _connection;
        private readonly RabbitMqSettings _settings;

        public RabbitMqTopology(
            RabbitMqConnection connection,
            IOptions<RabbitMqSettings> options)
        {
            _connection = connection;
            _settings = options.Value;
        }

        public async Task ConfigureAsync()
        {
            await using var channel =
                await _connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _settings.ProcessarNotaExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            await channel.QueueDeclareAsync(
                queue: _settings.ProcessarNotaQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await channel.QueueBindAsync(
                queue: _settings.ProcessarNotaQueue,
                exchange: _settings.ProcessarNotaExchange,
                routingKey: _settings.ProcessarNotaRoutingKey);

            await channel.ExchangeDeclareAsync(
                exchange: _settings.ResultadoNotaExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            await channel.QueueDeclareAsync(
                queue: _settings.ResultadoNotaQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await channel.QueueBindAsync(
                queue: _settings.ResultadoNotaQueue,
                exchange: _settings.ResultadoNotaExchange,
                routingKey: _settings.ResultadoNotaRoutingKey);
        }
    }
}
