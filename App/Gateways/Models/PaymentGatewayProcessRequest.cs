using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Models;

namespace PaymentProcessingSystem.Gateways.Models
{
    public class PaymentGatewayProcessRequest
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string PaymentMethod { get; set; }
    }
}
