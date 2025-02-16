namespace Client.Models
{
    public class CancelPayment
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; }
    }
}
