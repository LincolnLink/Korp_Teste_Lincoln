namespace Korp.Faturamento.Application.Messages
{
    public class ItemNotaFiscalMessage
    {
        public Guid ProdutoId { get; set; }

        public int Quantidade { get; set; }
    }
}
