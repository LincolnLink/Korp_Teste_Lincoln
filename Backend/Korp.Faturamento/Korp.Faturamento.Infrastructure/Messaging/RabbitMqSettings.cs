using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Faturamento.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        public string ProcessarNotaExchange { get; set; } = string.Empty;
        public string ProcessarNotaQueue { get; set; } = string.Empty;
        public string ProcessarNotaRoutingKey { get; set; } = string.Empty;
    }
}
