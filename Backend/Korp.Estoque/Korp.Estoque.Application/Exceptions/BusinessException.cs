namespace Korp.Estoque.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string mensagem) : base(mensagem){}
    }
}
