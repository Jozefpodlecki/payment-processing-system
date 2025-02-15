namespace PaymentProcessingSystem.Abstractions.Models
{
    public class PaymentGatewayResponse
    {
        public bool IsSuccess { get; set; }
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
    }
}
