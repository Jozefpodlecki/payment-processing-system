using PaymentProcessingSystem.Gateways.Models;

namespace PaymentProcessingSystem.Gateways
{
    public interface IPaymentGateway
    {
        Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayProcessRequest request, CancellationToken cancellationToken = default);
    }
}
