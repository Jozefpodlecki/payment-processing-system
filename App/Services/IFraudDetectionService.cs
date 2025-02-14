namespace PaymentProcessingSystem.Services
{
    public interface IFraudDetectionService
    {
        Task<bool> CheckForFraudAsync(Guid userId, decimal amount, string paymentMethod, CancellationToken cancellationToken);
    }
}
