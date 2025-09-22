using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Common.Interfaces
{
    public interface ISaleReadRepository
    {
        Task UpsertAsync(Sale sale);
        Task<Sale?> GetByIdAsync(Guid id);
        Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    }
}