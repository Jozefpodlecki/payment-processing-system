using k8s.KubeConfigModels;
using Microsoft.Azure.Cosmos;
using PaymentProcessingSystem.Models;
using System.Net;

namespace PaymentProcessingSystem.Repositories
{
    public class RefundRepository : IRefundRepository
    {
        private readonly Container _container;
        private readonly ILogger<PaymentRepository> _logger;

        public RefundRepository(CosmosClient client, ILogger<PaymentRepository> logger)
        {
            _logger = logger;
            var database = client.GetDatabase("PaymentDB");
            _container = database.GetContainer("Refunds");
        }

        public async Task SaveAsync(Refund refund, CancellationToken cancellationToken)
        {
            try
            {
                var partitionKey = new PartitionKey(refund.UserId.ToString());
                await _container.CreateItemAsync(refund, partitionKey, null, cancellationToken);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogError(ex, $"{refund.Id} is invalid");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogError(ex, $"{refund.Id} already exists");
            }
        }
    }
}
