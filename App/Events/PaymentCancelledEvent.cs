namespace PaymentProcessingSystem.Events
{
    public class PaymentCancelledEvent
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset CancelledOn { get; set; }
    }
}
