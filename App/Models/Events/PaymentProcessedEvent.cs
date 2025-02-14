using PaymentProcessingSystem.Abstractions.Models;

namespace PaymentProcessingSystem.Models.Events
{
    public class PaymentProcessedEvent
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTimeOffset ProcessedOn { get; set; }
    }
}
