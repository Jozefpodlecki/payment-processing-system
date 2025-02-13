namespace PaymentProcessingSystem.Models.Response
{
    public class CancelPaymentResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public PaymentStatus? UpdatedStatus { get; set; }
        public DateTimeOffset? CancelledOn { get; set; }
    }
}
