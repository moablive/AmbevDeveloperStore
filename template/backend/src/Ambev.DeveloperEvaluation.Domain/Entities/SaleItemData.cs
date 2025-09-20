namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Representa os dados essenciais de um item de venda, usado para desacoplar o Domínio da Aplicação.
    /// </summary>
    public record SaleItemData(Guid ProductId, int Quantity);
}