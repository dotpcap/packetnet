/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// Base class for several TLV types that all contain strings
    /// </summary>
    public class StringTLV : TLV
    {
        #region Constructors

        /// <summary>
        /// Creates a String TLV
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Port Description TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public StringTLV(byte[] bytes, int offset) :
            base(bytes, offset)
        {}

        /// <summary>
        /// Create from a type and string value
        /// </summary>
        /// <param name="tlvType">
        /// A <see cref="TLVTypes"/>
        /// </param>
        /// <param name="StringValue">
        /// A <see cref="System.String"/>
        /// </param>
        public StringTLV(TLVTypes tlvType, string StringValue)
        {
            var bytes = new byte[TLVTypeLength.TypeLengthLength];
            var offset = 0;
            tlvData = new ByteArraySegment(bytes, offset, bytes.Length);

            Type = tlvType;
            this.StringValue = StringValue;
        }

        #endregion

        #region Properties

        /// <value>
        /// A textual Description of the port
        /// </value>
        public string StringValue
        {
            get
            {
                return System.Text.ASCIIEncoding.ASCII.GetString(tlvData.Bytes,
                                                                 ValueOffset,
                                                                 Length);
            }

            set
            {
                var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                var length = TLVTypeLength.TypeLengthLength + bytes.Length;

                // is the tlv the correct size?
                if(tlvData.Length != length)
                {
                    // allocate new memory for this tlv
                    var newTLVBytes = new byte[length];
                    var offset = 0;

                    // copy header over
                    Array.Copy(tlvData.Bytes, tlvData.Offset,
                               newTLVBytes, 0,
                               TLVTypeLength.TypeLengthLength);

                    tlvData = new ByteArraySegment(newTLVBytes, offset, length);
                }

                // set the description
                Array.Copy(bytes, 0,
                           tlvData.Bytes, ValueOffset,
                           bytes.Length);
            }
        }

        /// <summary>
        /// Convert this Port Description TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[{0}: Description={0}]", Type, StringValue);
        }

        #endregion
    }
}
