namespace Korp.Estoque.Domain.Messages
{
    public class ResultadoProcessamentoNotaMessage
    {
        public Guid NotaFiscalId { get; set; }

        public bool Sucesso { get; set; }

        public string Mensagem { get; set; } = string.Empty;
    }
}
