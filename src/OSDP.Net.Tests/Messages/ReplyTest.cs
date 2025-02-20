﻿using NUnit.Framework;
using OSDP.Net.Messages;
using OSDP.Net.Messages.ACU;
using System;
using OSDP.Net.Messages.SecureChannel;

namespace OSDP.Net.Tests.Messages
{
    [TestFixture]
    public class ReplyTest
    {
        [Test]
        public void ParseAckToPollCommandTest()
        {
            const byte address = 0;

            var command = new IdReportCommand(address);
            var device = new DeviceProxy(address, true, false, null);
            var connectionId = new Guid();

            // Raw bytes taken off the wire from actual reader responding to poll
            byte[] rawResponse = new byte[] {83, 128, 8, 0, 5, 64, 104, 159};

            var reply = new ReplyTracker(connectionId, new IncomingMessage(rawResponse, new ACUMessageSecureChannel()), command, device);

            Assert.That(reply.Message.Type, Is.EqualTo((byte)ReplyType.Ack));
            Assert.That(reply.IsValidReply, Is.True);
            Assert.That(reply.IsValidReply, Is.True);
            Assert.That(reply.Message.Sequence, Is.EqualTo(1));
            Assert.That(reply.Message.Address, Is.EqualTo(address));
            Assert.That(reply.MatchIssuingCommand(command), Is.True);
        }
    }
}