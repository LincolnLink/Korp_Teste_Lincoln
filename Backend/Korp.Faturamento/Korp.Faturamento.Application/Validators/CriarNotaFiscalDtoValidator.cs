using FluentValidation;
using Korp.Faturamento.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Faturamento.Application.Validators
{
    public class CriarNotaFiscalDtoValidator
        : AbstractValidator<CriarNotaFiscalDto>
    {
        public CriarNotaFiscalDtoValidator()
        {
            RuleFor(x => x.Itens)
                .NotNull()
                .WithMessage("A nota fiscal deve possuir itens.")
                .NotEmpty()
                .WithMessage("A nota fiscal deve possuir pelo menos um produto.");

            RuleForEach(x => x.Itens)
                .SetValidator(new ItemNotaFiscalRequestDtoValidator());
        }
    }
}
