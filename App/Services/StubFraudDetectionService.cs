
namespace PaymentProcessingSystem.Services
{
    public class StubFraudDetectionService : IFraudDetectionService
    {
        private readonly StubFraudDetectionServiceOptions _options;

        public class StubFraudDetectionServiceOptions
        {
            public required HashSet<Guid> FraudulentUserIds { get; set; } 
        }

        public StubFraudDetectionService(StubFraudDetectionServiceOptions options)
        {
            _options = options;
        }

        public Task<bool> CheckForFraudAsync(Guid userId, decimal amount, string paymentMethod, CancellationToken cancellationToken)
        {
            if (_options.FraudulentUserIds.Contains(userId))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
