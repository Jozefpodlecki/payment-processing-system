using System.Buffers.Binary;

namespace PaymentProcessingSystem.Abstractions
{
    internal sealed class SequentialGuidGenerator : IGuidGenerator
    {
        private readonly byte[] _guidBytes;
        private readonly bool _isLittleEndian;

        public SequentialGuidGenerator()
        {
            _guidBytes = Guid.Empty.ToByteArray();
            _isLittleEndian = BitConverter.IsLittleEndian;
        }

        public Guid NewGuid()
        {
            var timestamp = DateTime.UtcNow.Ticks / 10000L;
            //var timestampBytes = BitConverter.GetBytes(timestamp);
            var timestampBytes = GetBytesSpan(timestamp);

            if (_isLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            Buffer.BlockCopy(timestampBytes, 2, _guidBytes, 0, 6);

            return new Guid(_guidBytes);
        }

        private static byte[] GetBytesSpan(long value)
        {
            Span<byte> bytes = stackalloc byte[8];
            BinaryPrimitives.WriteInt64BigEndian(bytes, value);
            return bytes.ToArray();
        }
    }
}
