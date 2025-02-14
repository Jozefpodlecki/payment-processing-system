namespace PaymentProcessingSystem.Models.Response
{
    public class ProcessPaymentResponse
    {
        public bool IsSuccess { get; set; }
        public Guid PaymentId { get; set; }
        public string Message { get; set; }
    }
}
