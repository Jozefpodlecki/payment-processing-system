namespace PaymentProcessingSystem.Gateways.Models
{
    public class PaymentGatewayResponse
    {
        public string TransactionId { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
