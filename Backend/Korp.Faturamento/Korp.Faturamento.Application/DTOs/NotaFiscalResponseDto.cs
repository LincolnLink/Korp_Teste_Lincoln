using Korp.Faturamento.Domain.Enums;

namespace Korp.Faturamento.Application.DTOs
{
    public class NotaFiscalResponseDto
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public StatusNotaFiscal Status { get; set; }
        public DateTime DataCriacao { get; set; }

        public IEnumerable<ItemNotaFiscalResponseDto> Itens { get; set; }
            = [];
    }
}
