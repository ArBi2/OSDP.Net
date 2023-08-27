using System;
using System.Collections.Generic;

namespace OSDP.Net.Model.CommandData
{
    /// <summary>
    /// Command data to set the encryption key configuration on a PD.
    /// </summary>
    public class EncryptionKeyConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionKeyConfiguration"/> class.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="keyData">16 bytes of key data.</param>
        public EncryptionKeyConfiguration(KeyType keyType, byte[] keyData)
        {
            KeyType = keyType;
            KeyData = keyData;
        }

        /// <summary>
        /// Gets the type of the key.
        /// </summary>
        public KeyType KeyType { get; }

        /// <summary>
        /// Gets 16 bytes of key data.
        /// </summary>
        public byte[] KeyData { get; }

        internal byte[] BuildData()
        {
            var data = new List<byte>
            {
                (byte)KeyType,
                (byte)KeyData.Length
            };
            data.AddRange(KeyData);

            return data.ToArray();
        }
        
        /// <summary>Parses the message payload bytes</summary>
        /// <param name="data">Message payload as bytes</param>
        /// <returns>An instance of EncryptionKeyConfiguration representing the message payload</returns>
        public static EncryptionKeyConfiguration ParseData(ReadOnlySpan<byte> data)
        {
            return new EncryptionKeyConfiguration((KeyType)data[0], data.Slice(2).ToArray());
        }
    }
}