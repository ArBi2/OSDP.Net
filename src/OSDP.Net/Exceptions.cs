﻿using OSDP.Net.Messages;
using OSDP.Net.Model.ReplyData;
using System;

namespace OSDP.Net
{
    /// <summary>
    /// Represents a custom exception defined in OSDP.Net library
    /// </summary>
    public class OSDPNetException : Exception 
    {
        /// <summary>
        /// Initializes a new instance of OSDP.Net.OSDPNetException with a specified
        /// error message
        /// </summary>
        /// <param name="message">Message that describes the error</param>
        public OSDPNetException(string message) : base(message) {}
    }

    /// <summary>
    /// Represent a failure condition where PD indicates that it didn't accept the 
    /// command by replying with osdp_NAK packet
    /// </summary>
    public sealed class NackReplyException : OSDPNetException
    {
        /// <summary>
        /// Parsed osdp_NAK packet data
        /// </summary>
        public Nak Reply { get; }

        /// <summary>
        /// Initializes a new instance of OSDP.Net.NackReplyException class
        /// </summary>
        /// <param name="replyData">osdp_NAK packet data returned from PD</param>
        /// <param name="message">Optional message to be included with the exception</param>
        public NackReplyException(Nak replyData, string message = null) : base(message)
        {
            Message =
                $"Received NAK error '{Helpers.SplitCamelCase(replyData.ErrorCode.ToString())}'.{(string.IsNullOrEmpty(Message) ? string.Empty : $" {Message}")}";
            Reply = replyData;
        }

        /// <inheritdoc />
        public override string Message { get; }
    }
}