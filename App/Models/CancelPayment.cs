namespace PaymentProcessingSystem.Models
{
    public class CancelPayment
    {
        public Guid PaymentId { get; set; }
        public string Reason { get; set; }
    }
}
