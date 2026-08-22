using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Faturamento.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = string.Empty;

        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";
    }
}
