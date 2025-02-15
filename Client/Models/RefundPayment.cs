namespace Client.Models
{
    public class RefundPayment
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
    }
}
