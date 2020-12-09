/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet.Tcp
{
    /// <summary>
    /// MD5 Signature
    /// Carries the MD5 Digest used by the BGP protocol to
    /// ensure security between two endpoints
    /// </summary>
    /// <remarks>
    /// References:
    /// http://datatracker.ietf.org/doc/rfc2385/
    /// </remarks>
    public class MD5SignatureOption : TcpOption
    {
        // the offset (in bytes) of the MD5 Digest field
        private const int MD5DigestFieldOffset = 2;

        /// <summary>
        /// Creates a MD5 Signature Option
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
        public MD5SignatureOption(byte[] bytes, int offset, int length) :
            base(bytes, offset, length)
        { }

        /// <summary>
        /// The MD5 Digest
        /// </summary>
        public byte[] MD5Digest
        {
            get
            {
                var data = new byte[Length - MD5DigestFieldOffset];
                Array.Copy(OptionData.Bytes, OptionData.Offset + MD5DigestFieldOffset, data, 0, data.Length);
                return data;
            }
            set => Array.Copy(value, 0, OptionData.Bytes, OptionData.Offset + MD5DigestFieldOffset, value.Length);
        }

        /// <summary>
        /// Returns the Option info as a string
        /// </summary>
        /// <returns>
        /// A <see cref="string" />
        /// </returns>
        public override string ToString()
        {
            return "[" + Kind + ": MD5Digest=0x" + MD5Digest + "]";
        }
    }
}