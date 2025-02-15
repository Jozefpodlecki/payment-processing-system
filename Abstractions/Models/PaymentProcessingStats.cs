namespace PaymentProcessingSystem.Abstractions.Models
{
    public class PaymentProcessingStats
    {
        public int CompletedCount { get; set; }
        public int ProcessedCount { get; set; }
        public int FailedCount { get; set; }
    }
}
