namespace PaymentProcessingSystem.Abstractions.Models
{
    public class PaymentProcessingStats
    {
        public DateTimeOffset UpdatedOn { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int RefundedCount { get; set; }
        public int ProcessedCount { get; set; }
        public int FailedCount { get; set; }
    }
}
