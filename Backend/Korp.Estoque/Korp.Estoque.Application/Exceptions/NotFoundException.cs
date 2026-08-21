namespace Korp.Estoque.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string mensagem)
        : base(mensagem)
        {
        }
    }
}
