using k8s.KubeConfigModels;
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

        public async Task<Payment?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var partitionKey = new PartitionKey(userId.ToString());
                var response = await _container.ReadItemAsync<Payment>(id.ToString(), partitionKey);

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, $"Payment with ID {id} and UserId {userId} not found.");
                return null;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving payment with ID {id} and UserId {userId}.");
                throw;
            }
        }

        public async Task SaveAsync(Payment payment, CancellationToken cancellationToken)
        {
            try
            {
                var partitionKey = new PartitionKey(payment.UserId.ToString());
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

        public Task SaveRefundAsync(Refund refund, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            try
            {
                var partitionKey = new PartitionKey(payment.UserId.ToString());
                await _container.ReplaceItemAsync(payment, payment.Id.ToString(), partitionKey, null, cancellationToken);

                _logger.LogInformation($"Payment with ID {payment.Id} updated successfully.");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(ex, $"Payment with ID {payment.Id} and UserId {payment.UserId} not found.");
                throw new KeyNotFoundException($"Payment with ID {payment.Id} and UserId {payment.UserId} not found.");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogError(ex, $"Invalid data for payment with ID {payment.Id}.");
                throw new ArgumentException($"Invalid data for payment with ID {payment.Id}.");
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, $"An error occurred while updating payment with ID {payment.Id}.");
                throw;
            }
        }
    }
}
