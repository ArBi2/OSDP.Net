﻿using OSDP.Net.Messages;
using System;

namespace OSDP.Net.Model.ReplyData
{
    /// <summary>
    /// Represents a reply to osdp_CHLNG command and contain's PD's ID, 
    /// PD-generated random number and PD cryptogram
    /// </summary>
    public class ChallengeResponse : ReplyData
    {
        /// <summary>
        /// Create a new instance of the ChallengeResponse
        /// </summary>
        /// <param name="cUID">PD(Client) ID to send back to ACU</param>
        /// <param name="rndB">PD random number</param>
        /// <param name="cryptogram">PD Cryptogram</param>
        public ChallengeResponse(byte[] cUID, byte[] rndB, byte[] cryptogram)
        {
            ClientUID = cUID;
            RndB = rndB;
            Cryptogram = cryptogram;
        }

        /// <summary>
        /// Client Unique ID
        /// </summary>
        public byte[] ClientUID {get; private set;}

        /// <summary>
        /// Rnd.B as defined in OSDP.NET spec
        /// </summary>
        public byte[] RndB { get; private set; }

        /// <summary>
        /// Client (PD-side) cryptogram as defined in OSDP.NET spec
        /// </summary>
        public byte[] Cryptogram { get; private set; }

        /// <summary>Parses the message payload bytes</summary>
        /// <param name="data">Message payload as bytes</param>
        /// <returns>An instance of ChallengeResponse representing the message payload</returns>
        public static ChallengeResponse ParseData(ReadOnlySpan<byte> data)
        {
            if (data.Length != 32)
            {
                throw new InvalidPayloadException($"Challenge response must be 32 bytes, received {data.Length}");
            }

            return new ChallengeResponse(
                data.Slice(0, 8).ToArray(),
                data.Slice(8, 8).ToArray(),
                data.Slice(16, 16).ToArray()
            );
        }

        /// <inheritdoc/>
        public override byte[] BuildData(bool withPadding = false)
        {
            if (withPadding)
            {
                // This response is sent IN ORDER to establish
                // security. Therefore padding for AES should never be
                // applied to it
                throw new InvalidOperationException("Challenge response should never be padded!");
            }

            var payload = new byte[ClientUID.Length + RndB.Length + Cryptogram.Length];
            ClientUID.CopyTo(payload, 0);
            RndB.CopyTo(payload, ClientUID.Length);
            Cryptogram.CopyTo(payload, ClientUID.Length + RndB.Length);

            return payload;
        }

        /// <inheritdoc/>
        public override ReplyType ReplyType => ReplyType.CrypticData;
    }
}
