using PaymentProcessingSystem.Gateways.Models;
using Stripe;

namespace PaymentProcessingSystem.Gateways
{
    public class StubPaymentGateway : IPaymentGateway
    {
        private readonly StubPaymentGatewayOptions _options;

        public class StubPaymentGatewayOptions
        {
            public required HashSet<Guid> UserIdsToFail { get; set; }
        }

        public StubPaymentGateway(StubPaymentGatewayOptions options)
        {
            _options = options;
        }

        public Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayProcessRequest request, CancellationToken cancellationToken = default)
        {
            if(_options.UserIdsToFail.Contains(request.UserId))
            {
                return Task.FromResult(new PaymentGatewayResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Payment failed"
                });
            }

            return Task.FromResult(new PaymentGatewayResponse
            {
                IsSuccess = true,
                TransactionId = Guid.NewGuid().ToString()
            });
        }
    }
}
