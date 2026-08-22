using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Faturamento.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string mensagem)
            : base(mensagem)
        {
        }
    }
}
