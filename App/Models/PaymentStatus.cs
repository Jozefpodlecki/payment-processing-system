namespace PaymentProcessingSystem.Models
{
    public enum PaymentStatus
    {
        Pending = 0,
        Cancelled = 1,
        Completed = 2,
        Refunded = 3,
        FraudDetected = 4,
        Failed = 5
    }
}
