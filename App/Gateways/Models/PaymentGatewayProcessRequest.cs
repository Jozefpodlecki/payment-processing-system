using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Models;

namespace PaymentProcessingSystem.Gateways.Models
{
    public class PaymentGatewayProcessRequest
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string PaymentMethod { get; set; }
    }
}
