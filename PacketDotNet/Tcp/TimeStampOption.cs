/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Tcp;

    /// <summary>
    /// A Time Stamp Option
    /// Used for RTTM (Round Trip Time Measurement)
    /// and PAWS (Protect Against Wrapped Sequences)
    /// Obsoletes the Echo and EchoReply option fields
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc1323/
    /// </remarks>
    public class TimeStampOption : TcpOption
    {
        // the offset (in bytes) of the Echo Reply Field
        private const int EchoReplyFieldOffset = 6;

        // the offset (in bytes) of the Value Field
        private const int ValueFieldOffset = 2;

        /// <summary>
        /// Creates a Timestamp Option
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="T:System.Byte[]" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public TimeStampOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The Echo Reply
        /// </summary>
        public uint EchoReply => EndianBitConverter.Big.ToUInt32(OptionData.Bytes, OptionData.Offset + EchoReplyFieldOffset);

        /// <summary>
        /// The Timestamp value
        /// </summary>
        public uint Value
        {
            get => EndianBitConverter.Big.ToUInt32(OptionData.Bytes, OptionData.Offset + ValueFieldOffset);
            set => EndianBitConverter.Big.CopyBytes(value, OptionData.Bytes, OptionData.Offset + ValueFieldOffset);
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": Value=" + Value + " EchoReply=" + EchoReply + "]";
        }
    }