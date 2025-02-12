using Microsoft.Azure.Cosmos;
using PaymentProcessingSystem.Models;
using System.Net;

namespace PaymentProcessingSystem.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly Container _container;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(CosmosClient client, ILogger<PaymentRepository> logger)
        {
            _logger = logger;
            var database = client.GetDatabase("PaymentDB");
            _container = database.GetContainer("Payments");
        }

        public async Task SaveAsync(Payment payment, CancellationToken cancellationToken)
        {
            try
            {
                var partitionKey = new PartitionKey(payment.UserId);
                await _container.CreateItemAsync(payment, partitionKey, null, cancellationToken);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogError(ex, $"{payment.Id} is invalid");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogError(ex, $"{payment.Id} already exists");
            }
        }
    }
}
