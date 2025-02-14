
namespace PaymentProcessingSystem.Abstractions
{
    internal sealed class SystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
