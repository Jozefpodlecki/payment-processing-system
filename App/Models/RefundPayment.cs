namespace PaymentProcessingSystem.Models
{
    public class RefundPayment
    {
        public Guid PaymentId { get; set; }
        public string Reason { get; set; }
    }
}
