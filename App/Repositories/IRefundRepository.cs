using PaymentProcessingSystem.Models;

namespace PaymentProcessingSystem.Repositories
{
    public interface IRefundRepository
    {
        Task SaveAsync(Refund refund, CancellationToken cancellationToken = default);
    }
}
