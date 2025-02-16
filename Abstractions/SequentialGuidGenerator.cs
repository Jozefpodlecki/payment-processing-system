using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace PaymentProcessingSystem.Abstractions
{
    internal sealed class SequentialGuidGenerator : IGuidGenerator
    {
        private readonly byte[] _guidBytes;
        private readonly RandomNumberGenerator _random;
        private readonly bool _isLittleEndian;

        public SequentialGuidGenerator()
        {
            _guidBytes = Guid.Empty.ToByteArray();
            _random = RandomNumberGenerator.Create();
            _isLittleEndian = BitConverter.IsLittleEndian;
        }

        public Guid NewGuid()
        {
            var timestamp = DateTime.UtcNow.Ticks;
            //var timestampBytes = BitConverter.GetBytes(timestamp);
            var timestampBytes = GetBytesSpan(timestamp);

            if (_isLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            Buffer.BlockCopy(timestampBytes, 2, _guidBytes, 0, 6);
            
            _random.GetBytes(_guidBytes, 6, 10);

            _guidBytes[7] = (byte)((_guidBytes[7] & 0x0F) | 0x40);
            _guidBytes[8] = (byte)((_guidBytes[8] & 0x3F) | 0x80);

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
