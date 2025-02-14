using Client.Models;
using PaymentProcessingSystem.Abstractions.Models;

namespace Client
{
    public interface IPaymentProcessingApi
    {
        Task<ApiResponse> CancelPaymentAsync(CancelPayment model, CancellationToken token = default);
        Task<ApiResponse> RefundPaymentAsync(RefundPayment model, CancellationToken token = default);
        Task<ApiResponse> ProcessPaymentAsync(ProcessPayment model, CancellationToken token = default);
    }
}
