namespace Abstractions
{
    public class RandomGuidGenerator : IGuidGenerator
    {
        /// <summary>
        /// Calls interally <see cref="https://learn.microsoft.com/en-us/windows/win32/api/combaseapi/nf-combaseapi-cocreateguid"/>
        /// </summary>
        /// <returns>A new GUID.</returns>
        public Guid NewGuid() => Guid.NewGuid();
    }
}
