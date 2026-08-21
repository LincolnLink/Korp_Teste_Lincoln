namespace Korp.Estoque.Domain.Entities
{
    public class Produto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Saldo { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
