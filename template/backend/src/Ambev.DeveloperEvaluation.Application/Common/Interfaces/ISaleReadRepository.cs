using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Common.Interfaces
{
    public interface ISaleReadRepository
    {
        Task UpsertAsync(Sale sale);
        Task<Sale?> GetByIdAsync(Guid id);
    }
}