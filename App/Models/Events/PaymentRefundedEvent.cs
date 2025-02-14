namespace PaymentProcessingSystem.Models.Events
{
    public class PaymentRefundedEvent
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset RefundedOn { get; set; }
        public Guid RefundId { get; set; }
    }
}
