using PaymentProcessingSystem.Gateways.Models;
using Stripe;

namespace PaymentProcessingSystem.Gateways
{
    public class VoidPaymentGateway : IPaymentGateway
    {
        public Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayProcessRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new PaymentGatewayResponse
            {
                IsSuccess = true,
                TransactionId = Guid.NewGuid().ToString()
            });
        }
    }
}
