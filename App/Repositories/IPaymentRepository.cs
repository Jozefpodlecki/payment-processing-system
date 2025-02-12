using PaymentProcessingSystem.Models;

namespace PaymentProcessingSystem.Repositories
{
    public interface IPaymentRepository
    {
        Task SaveAsync(Payment payment, CancellationToken cancellationToken);   
    }
}
