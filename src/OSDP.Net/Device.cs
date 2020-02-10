using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OSDP.Net.Messages;

namespace OSDP.Net
{
    internal class Device : IComparable<Device>
    {
        private readonly ConcurrentQueue<Command> _commands = new ConcurrentQueue<Command>();
        private readonly SecureChannel _secureChannel = new SecureChannel();
        private readonly bool _useSecureChannel;
        private DateTime _lastValidReply = DateTime.MinValue;

        public Device(byte address, bool useCrc, bool useSecureChannel)
        {
            _useSecureChannel = useSecureChannel;
            Address = address;
            MessageControl = new Control(0, useCrc, useSecureChannel);
        }

        public byte Address { get; }

        public Control MessageControl { get; }

        public bool IsSecurityEstablished => MessageControl.HasSecurityControlBlock && _secureChannel.IsEstablished;

        public bool IsConnected => _lastValidReply + TimeSpan.FromSeconds(5) >= DateTime.UtcNow;

        /// <inheritdoc />
        public int CompareTo(Device other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Address.CompareTo(other.Address);
        }

        public Command GetNextCommandData()
        {
            if (MessageControl.Sequence == 0)
            {
                return new PollCommand(Address);
            }
            
            if (_useSecureChannel && !_secureChannel.IsInitialized)
            {
                return new SecurityInitializationRequestCommand(Address, _secureChannel.ServerRandomNumber().ToArray());
            }

            if (_useSecureChannel && !_secureChannel.IsEstablished)
            {
                return new ServerCryptogramCommand(Address, _secureChannel.ServerCryptogram);
            }

            if (!_commands.TryDequeue(out var command))
            {
                return new PollCommand(Address);
            }

            return command;
        }

        public void SendCommand(Command command)
        {
            _commands.Enqueue(command);
        }

        public void ValidReplyHasBeenReceived(byte sequence)
        {
            MessageControl.IncrementSequence(sequence);
            _lastValidReply = DateTime.UtcNow;
        }

        public void InitializeSecureChannel(Reply reply)
        {
            var replyData = reply.ExtractReplyData.ToArray();
            
            _secureChannel.Initialize(replyData.Take(8).ToArray(),
                replyData.Skip(8).Take(8).ToArray(),
                replyData.Skip(16).Take(16).ToArray());
        }

        public bool ValidateSecureChannelEstablishment(Reply reply)
        {
            if (!reply.SecureCryptogramHasBeenAccepted())
            {
                return false;
            }
            
            _secureChannel.Establish(reply.ExtractReplyData.ToArray());

            return true;
        }

        public IEnumerable<byte> GenerateMac(byte[] message, bool isCommand)
        {
            return _secureChannel.GenerateMac(message, isCommand);
        }

        public void ResetSecurity()
        {
            _secureChannel.Reset();
        }

        public IEnumerable<byte> EncryptData(IEnumerable<byte> data)
        {
            return _secureChannel.EncryptData(data);
        }

        public IEnumerable<byte> DecryptData(IEnumerable<byte> data)
        {
            return _secureChannel.DecryptData(data);
        }
    }
}