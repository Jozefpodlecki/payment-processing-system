namespace PaymentProcessingSystem.Models.Reponse
{
    public class RefundPaymentResponse
    {
        public Guid RefundId { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTimeOffset RefundedOn { get; set; }
    }
}
