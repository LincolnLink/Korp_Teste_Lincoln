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

            // =========================================
            // 1 - PROCESSAMENTO PRINCIPAL
            // =========================================

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


            // =========================================
            // 2 - RESULTADO
            // =========================================

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


            // =========================================
            // 3 - RETRY
            // =========================================

            await channel.ExchangeDeclareAsync(
                exchange: _settings.RetryExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            var retryArgs =
                new Dictionary<string, object?>
                {
                    // A mensagem espera 5 segundos nessa fila
                    {
                        "x-message-ttl",
                        5000
                    },

                    // Depois dos 5 segundos volta para
                    // a exchange principal
                    {
                        "x-dead-letter-exchange",
                        _settings.ProcessarNotaExchange
                    },

                    {
                        "x-dead-letter-routing-key",
                        _settings.ProcessarNotaRoutingKey
                    }
                };

            await channel.QueueDeclareAsync(
                queue: _settings.RetryQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: retryArgs);

            await channel.QueueBindAsync(
                queue: _settings.RetryQueue,
                exchange: _settings.RetryExchange,
                routingKey: _settings.RetryRoutingKey);


            // =========================================
            // 4 - DEAD LETTER / DLQ
            // =========================================

            await channel.ExchangeDeclareAsync(
                exchange: _settings.DeadLetterExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            await channel.QueueDeclareAsync(
                queue: _settings.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await channel.QueueBindAsync(
                queue: _settings.DeadLetterQueue,
                exchange: _settings.DeadLetterExchange,
                routingKey: _settings.DeadLetterRoutingKey);
        }
    }
}