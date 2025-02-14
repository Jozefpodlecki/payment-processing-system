
namespace PaymentProcessingSystem.Services
{
    public class VoidFraudDetectionService : IFraudDetectionService
    {
        public Task<bool> CheckForFraudAsync(Guid userId, decimal amount, string paymentMethod, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}
