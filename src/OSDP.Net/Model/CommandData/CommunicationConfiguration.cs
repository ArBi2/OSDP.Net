using System;
using System.Collections.Generic;
using System.Linq;
using OSDP.Net.Messages;

namespace OSDP.Net.Model.CommandData;

/// <summary>
/// Command data to set the communication configuration on a PD.
/// </summary>
public class CommunicationConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationConfiguration"/> class.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <param name="baudRate">The baud rate.</param>
    public CommunicationConfiguration(byte address, int baudRate)
    {
        Address = address;
        BaudRate = baudRate;
    }

    /// <summary>
    /// Gets the address.
    /// </summary>
    public byte Address { get; }

    /// <summary>
    /// Gets the baud rate.
    /// </summary>
    public int BaudRate { get; }

    internal IEnumerable<byte> BuildData()
    {
        var baudRateBytes = Message.ConvertIntToBytes(BaudRate).ToArray();
            
        return new[]
        {
            Address,
            baudRateBytes[0],
            baudRateBytes[1],
            baudRateBytes[2],
            baudRateBytes[3]
        };
    }

    /// <summary>Parses the message payload bytes</summary>
    /// <param name="data">Message payload as bytes</param>
    /// <returns>An instance of CommunicationConfiguration representing the message payload</returns>
    public static CommunicationConfiguration ParseData(ReadOnlySpan<byte> data)
    {
        return new CommunicationConfiguration(data[0], Message.ConvertBytesToInt(data.Slice(1, 4).ToArray()));
    }
}