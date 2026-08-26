using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Korp.Faturamento.Infrastructure.Messaging
{
    public class RabbitMqConnection : IAsyncDisposable
    {
        private readonly RabbitMqSettings _settings;

        private IConnection? _connection;

        public RabbitMqConnection(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            if (_connection is not null && _connection.IsOpen)
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,

                AutomaticRecoveryEnabled = true,

                ClientProvidedName = "Korp.Faturamento"
            };

            _connection = await factory.CreateConnectionAsync();

            return _connection;
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            var connection = await GetConnectionAsync();

            return await connection.CreateChannelAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
