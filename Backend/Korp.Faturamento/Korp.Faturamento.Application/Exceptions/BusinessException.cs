using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Faturamento.Application.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string mensagem)
            : base(mensagem)
        {
        }
    }
}
