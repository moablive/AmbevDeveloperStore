using Ambev.DeveloperEvaluation.Application.Common.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleReadRepository : ISaleReadRepository
    {
        private readonly IMongoCollection<Sale> _salesCollection;

        public SaleReadRepository(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDb:ConnectionString"];
            var databaseName = configuration["MongoDb:DatabaseName"];
            var collectionName = configuration["MongoDb:CollectionName"];

            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(databaseName);
            _salesCollection = database.GetCollection<Sale>(collectionName);
        }

        public async Task<Sale?> GetByIdAsync(Guid id)
        {
            return await _salesCollection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpsertAsync(Sale sale)
        {
            var filter = Builders<Sale>.Filter.Eq(s => s.Id, sale.Id);
            await _salesCollection.ReplaceOneAsync(filter, sale, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            // Filtra para não trazer vendas canceladas
            var filter = Builders<Sale>.Filter.Eq(s => s.IsCancelled, false);

            // Executa a contagem total e a busca da página em paralelo para melhor performance
            var totalCountTask = _salesCollection.CountDocumentsAsync(filter);

            var salesTask = _salesCollection.Find(filter)
                                        .SortBy(s => s.SaleDate)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Limit(pageSize)
                                        .ToListAsync();

            await Task.WhenAll(totalCountTask, salesTask);

            return (salesTask.Result, (int)totalCountTask.Result);
        }
    }
}