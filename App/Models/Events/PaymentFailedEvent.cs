namespace PaymentProcessingSystem.Models.Events
{
    public class PaymentFailedEvent
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
    }
}
