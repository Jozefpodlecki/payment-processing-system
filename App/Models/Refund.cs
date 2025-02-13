namespace PaymentProcessingSystem.Models
{
    public class Refund
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
    }
}
