/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// Base class for several TLV types that all contain strings
    /// </summary>
    public class StringTlv : Tlv
    {
        /// <summary>
        /// Creates a String Tlv
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Port Description TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public StringTlv(byte[] bytes, int offset) :
            base(bytes, offset)
        { }

        /// <summary>
        /// Create from a type and string value
        /// </summary>
        /// <param name="tlvType">
        /// A <see cref="TlvType" />
        /// </param>
        /// <param name="value">
        /// A <see cref="string" />
        /// </param>
        public StringTlv(TlvType tlvType, string value)
        {
            var bytes = new byte[TlvTypeLength.TypeLengthLength];
            Data = new ByteArraySegment(bytes, 0, bytes.Length);

            Type = tlvType;
            Value = value;
        }

        /// <value>
        /// A textual Description of the port
        /// </value>
        public string Value
        {
            get => Encoding.ASCII.GetString(Data.Bytes,
                                            ValueOffset,
                                            Length);
            set
            {
                var bytes = Encoding.ASCII.GetBytes(value);
                var length = TlvTypeLength.TypeLengthLength + bytes.Length;

                // is the TLV the correct size?
                if (Data.Length != length)
                {
                    // allocate new memory for this tlv
                    var newTLVBytes = new byte[length];
                    var offset = 0;

                    // copy header over
                    Array.Copy(Data.Bytes,
                               Data.Offset,
                               newTLVBytes,
                               0,
                               TlvTypeLength.TypeLengthLength);

                    Data = new ByteArraySegment(newTLVBytes, offset, length);
                }

                // set the description
                Array.Copy(bytes,
                           0,
                           Data.Bytes,
                           ValueOffset,
                           bytes.Length);
            }
        }

        /// <summary>
        /// Convert this Port Description TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return $"[{Type}: Description={Value}]";
        }
    }
}