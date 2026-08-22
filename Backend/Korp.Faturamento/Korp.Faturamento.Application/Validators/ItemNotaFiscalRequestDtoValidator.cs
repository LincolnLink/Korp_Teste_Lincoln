using FluentValidation;
using Korp.Faturamento.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Korp.Faturamento.Application.Validators
{
    public class ItemNotaFiscalRequestDtoValidator
        : AbstractValidator<ItemNotaFiscalRequestDto>
    {
        public ItemNotaFiscalRequestDtoValidator()
        {
            RuleFor(x => x.ProdutoId)
                .NotEmpty()
                .WithMessage("O produto é obrigatório.");

            RuleFor(x => x.Quantidade)
                .GreaterThan(0)
                .WithMessage("A quantidade deve ser maior que zero.");
        }
    }
}
