namespace PaymentProcessingSystem.Models.Events
{
    public class PaymentCompletedEvent
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
    }
}
