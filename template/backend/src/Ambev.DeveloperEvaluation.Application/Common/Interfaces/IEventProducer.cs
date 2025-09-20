namespace Ambev.DeveloperEvaluation.Application.Common.Interfaces
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, T message);
    }
}