namespace Korp.Estoque.Domain.Messages
{
    public class ItemNotaFiscalMessage
    {
        public Guid ProdutoId { get; set; }

        public int Quantidade { get; set; }
    }
}
