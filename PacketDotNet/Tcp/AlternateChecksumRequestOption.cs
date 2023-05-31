/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Tcp;

    /// <summary>
    /// AlternateChecksumRequestOption Option
    /// </summary>
    public class AlternateChecksumRequestOption : TcpOption
    {
        // the offset (in bytes) of the Checksum field
        private const int ChecksumFieldOffset = 2;

        /// <summary>
        /// Creates an Alternate Checksum Request Option
        /// Used to negotiate an alternative checksum algorithm in a connection
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
        /// <remarks>
        /// References:
        /// http://datatracker.ietf.org/doc/rfc1146/
        /// </remarks>
        public AlternateChecksumRequestOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The Checksum
        /// </summary>
        public ChecksumAlgorithmType Checksum
        {
            get => (ChecksumAlgorithmType) OptionData.Bytes[OptionData.Offset + ChecksumFieldOffset];
            set => OptionData.Bytes[OptionData.Offset + ChecksumFieldOffset] = (byte) value;
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": ChecksumType=" + Checksum + "]";
        }
    }