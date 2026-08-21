using FluentValidation;

namespace Korp.Estoque.Application.Validators
{
    public class ProdutoRequestDtoValidator : AbstractValidator<ProdutoRequestDto>
    {
        public ProdutoRequestDtoValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty()
                .WithMessage("O código é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O código deve possuir no máximo 50 caracteres.");

            RuleFor(x => x.Descricao)
                .NotEmpty()
                .WithMessage("A descrição é obrigatória.")
                .MaximumLength(200)
                .WithMessage("A descrição deve possuir no máximo 200 caracteres.");

            RuleFor(x => x.Saldo)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O saldo não pode ser negativo.");
        }
    }
}
