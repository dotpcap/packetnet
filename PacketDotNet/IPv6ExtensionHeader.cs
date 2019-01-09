using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Text;

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
 * Copyright 2018 Steven Haufe<haufes@hotmail.com>
 */

namespace PacketDotNet
{
    [Serializable]
    public class IPv6ExtensionHeader
    {
        public static HashSet<IPProtocolType> extensionHeaderTypes = new HashSet<IPProtocolType>{ IPProtocolType.HOPOPTS,
            IPProtocolType.DSTOPTS,
            IPProtocolType.ROUTING,
            IPProtocolType.FRAGMENT,
            IPProtocolType.AH,
            IPProtocolType.ENCAP,
            IPProtocolType.DSTOPTS,
            IPProtocolType.MOBILITY,
            IPProtocolType.HOSTIDENTITY,
            IPProtocolType.SHIM6,
            IPProtocolType.RESERVEDTYPE253,
            IPProtocolType.RESERVEDTYPE254};


        protected ByteArraySegment Header { get; set; }


        public IPv6ExtensionHeader(ByteArraySegment bas)
        {
            Header = bas;
        }

        public UInt16 PayloadLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                   Header.Offset + IPv6Fields.PayloadLengthPosition);

            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Header.Bytes,
                                                    Header.Offset + IPv6Fields.PayloadLengthPosition);
        }

        public IPProtocolType NextHeader
        {
            get => (IPProtocolType)EndianBitConverter.Big.ToUInt16(Header.Bytes,
                                                                    Header.Offset);

            set => EndianBitConverter.Big.CopyBytes((Byte)value,
                                                    Header.Bytes,
                                                    Header.Offset);
        }

        public UInt16 Length
        {
            get => (ushort)((PayloadLength + 1) * 8);
        }

        public ByteArraySegment OptionsAndPadding
        {
            get
            {
                return new ByteArraySegment(Header.Bytes, Header.Offset + 16, PayloadLength - 8);
            }
        }


    }
}
