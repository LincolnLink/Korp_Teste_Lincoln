using Korp.Faturamento.Domain.Enums;

namespace Korp.Faturamento.Domain.Entities
{
    public class NotaFiscal
    {
        public Guid Id { get; set; }

        public int Numero { get; set; }

        public StatusNotaFiscal Status { get; set; }

        public DateTime DataCriacao { get; set; }

        public ICollection<ItemNotaFiscal> Itens { get; set; }
            = new List<ItemNotaFiscal>();
    }
}
