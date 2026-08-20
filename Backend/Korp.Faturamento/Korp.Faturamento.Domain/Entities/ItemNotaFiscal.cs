namespace Korp.Faturamento.Domain.Entities
{
    public class ItemNotaFiscal
    {
        public Guid Id { get; set; }

        public Guid NotaFiscalId { get; set; }

        public Guid ProdutoId { get; set; }

        public int Quantidade { get; set; }

        public NotaFiscal NotaFiscal { get; set; } = null!;
    }
}
