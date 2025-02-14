namespace Client.Models
{
    public class ApiResponse
    {
        public Guid? PaymentId { get; set; }
        public string Message { get; set; }
        public Guid? RefundId { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
