using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Faturamento.Application.Messages
{
    public class ProcessarNotaFiscalMessage
    {
        public Guid NotaFiscalId { get; set; }

        public List<ItemNotaFiscalMessage> Itens { get; set; } = [];
    }
}
