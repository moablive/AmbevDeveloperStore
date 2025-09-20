using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    /// <summary>
    /// Define o contrato para as operações de repositório da entidade Sale.
    /// </summary>
    public interface ISaleRepository
    {
        /// <summary>
        /// Adiciona uma nova venda no repositório.
        /// </summary>
        /// <param name="sale">A entidade de venda a ser criada.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        Task AddAsync(Sale sale, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca uma venda pelo seu identificador único.
        /// </summary>
        /// <param name="id">O ID da venda.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>A entidade de venda se encontrada; caso contrário, nulo.</returns>
        Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza uma venda existente no repositório.
        /// </summary>
        /// <param name="sale">A entidade de venda a ser atualizada.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

        Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string orderBy);
    }
}