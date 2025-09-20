using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    /// <summary>
    /// Validador para a requisição de criação de venda (CreateSaleRequest).
    /// </summary>
    public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
    {
        public CreateSaleRequestValidator()
        {
            RuleFor(r => r.CustomerId).NotEmpty();
            RuleFor(r => r.BranchId).NotEmpty();
            RuleFor(r => r.Items).NotEmpty();
            RuleForEach(r => r.Items).SetValidator(new SaleItemRequestValidator());
        }
    }

    public class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
    {
        public SaleItemRequestValidator()
        {
            RuleFor(item => item.ProductId).NotEmpty();
            RuleFor(item => item.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(20).WithMessage("A quantidade máxima de um item por venda é 20.");
        }
    }
}