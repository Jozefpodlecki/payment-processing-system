using PaymentProcessingSystem.Models;

namespace PaymentProcessingSystem.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task SaveAsync(Payment payment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
        Task SaveRefundAsync(Refund refund, CancellationToken cancellationToken = default);
    }
}
