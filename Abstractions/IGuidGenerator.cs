namespace PaymentProcessingSystem.Abstractions
{
    public interface IGuidGenerator
    {
        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        Guid NewGuid();
    }
}
