using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Validador para o comando CreateSaleCommand.
    /// </summary>
    public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleCommandValidator()
        {
            RuleFor(c => c.CustomerId)
                .NotEmpty().WithMessage("O ID do cliente é obrigatório.");

            RuleFor(c => c.BranchId)
                .NotEmpty().WithMessage("O ID da filial é obrigatório.");

            RuleFor(c => c.Items)
                .NotEmpty().WithMessage("A lista de itens não pode ser vazia.");

            // Valida cada item da lista
            RuleForEach(c => c.Items).SetValidator(new SaleItemCommandValidator());
        }
    }

    /// <summary>
    /// Validador para cada item da venda.
    /// </summary>
    public class SaleItemCommandValidator : AbstractValidator<SaleItemCommand>
    {
        public SaleItemCommandValidator()
        {
            RuleFor(item => item.ProductId)
                .NotEmpty().WithMessage("O ID do produto é obrigatório.");

            RuleFor(item => item.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
                .LessThanOrEqualTo(20).WithMessage("Não é possível vender mais de 20 itens idênticos.");
        }
    }
}