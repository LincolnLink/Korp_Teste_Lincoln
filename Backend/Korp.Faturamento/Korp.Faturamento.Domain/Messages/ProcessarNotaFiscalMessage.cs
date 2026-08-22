namespace Korp.Faturamento.Domain.Messages
{
    public class ProcessarNotaFiscalMessage
    {
        public Guid NotaFiscalId { get; set; }

        public List<ItemNotaFiscalMessage> Itens { get; set; } = [];
    }
}
