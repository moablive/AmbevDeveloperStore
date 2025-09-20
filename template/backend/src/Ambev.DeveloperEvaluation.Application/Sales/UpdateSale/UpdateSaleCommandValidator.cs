using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    /// <summary>
    /// Validador para o comando UpdateSaleCommand.
    /// </summary>
    public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
    {
        public UpdateSaleCommandValidator()
        {
            RuleFor(c => c.SaleId)
                .NotEmpty().WithMessage("O ID da venda é obrigatório para a atualização.");

            RuleFor(c => c.CustomerId)
                .NotEmpty().WithMessage("O ID do cliente é obrigatório.");

            RuleFor(c => c.BranchId)
                .NotEmpty().WithMessage("O ID da filial é obrigatório.");

            RuleFor(c => c.Items)
                .NotEmpty().WithMessage("A lista de itens não pode ser vazia.");

            RuleForEach(c => c.Items)
                .SetValidator(new SaleItemCommandValidator());
        }
    }
}