
namespace PaymentProcessingSystem.Models.Response
{
    public class ApiResponse
    {
        public Guid? PaymentId { get; set; }
        public string Message { get; set; }
        public Guid? RefundId { get; set; }
    }
}
