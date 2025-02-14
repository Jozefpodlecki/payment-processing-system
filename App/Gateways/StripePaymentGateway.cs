using PaymentProcessingSystem.Gateways.Models;
using Stripe;

namespace PaymentProcessingSystem.Gateways
{
    public class StripePaymentGateway : IPaymentGateway
    {
        public async Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayProcessRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Amount * 100),
                    Currency = request.Currency.ToString(),
                    PaymentMethod = request.PaymentMethod,
                    Confirm = true,
                    OffSession = true,
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options, null, cancellationToken);

                if (paymentIntent.Status == "succeeded")
                {
                    return new PaymentGatewayResponse
                    {
                        IsSuccess = true,
                        TransactionId = paymentIntent.Id
                    };
                }
                else
                {
                    return new PaymentGatewayResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = paymentIntent.Status
                    };
                }
            }
            catch (StripeException ex)
            {
                return new PaymentGatewayResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
