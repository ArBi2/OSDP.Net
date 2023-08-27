using System;
using OSDP.Net.Messages.SecureChannel;

namespace OSDP.Net.Messages.ACU
{
    internal class SecurityInitializationRequestCommand : Command
    {
        private readonly bool _isDefaultKey;
        private readonly byte[] _serverRandomNumber;

        internal SecurityInitializationRequestCommand(byte address, byte[] serverRandomNumber, bool isDefaultKey)
        {
            Address = address;
            _serverRandomNumber = serverRandomNumber ?? throw new ArgumentNullException(nameof(serverRandomNumber));
            _isDefaultKey = isDefaultKey;
        }

        protected override byte CommandCode => (byte)CommandType.SessionChallenge;

        protected override ReadOnlySpan<byte> SecurityControlBlock()
        {
            return new byte[]
            {
                0x03,
                (byte)SecurityBlockType.BeginNewSecureConnectionSequence,
                (byte)(_isDefaultKey ? 0x00 : 0x01)
            };
        }

        protected override ReadOnlySpan<byte> Data()
        {
            return _serverRandomNumber;
        }

        protected override void CustomCommandUpdate(Span<byte> commandBuffer)
        {

        }
    }
}