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
    internal class IPv6ExtensionHeader
    {
        static public List<IPProtocolType> extensionHeaderTypes = new List<IPProtocolType>{ IPProtocolType.HOPOPTS,
            IPProtocolType.DSTOPTS,
            IPProtocolType.ROUTING,
            IPProtocolType.FRAGMENT,
            IPProtocolType.AH,
            IPProtocolType.ENCAP,
            IPProtocolType.DSTOPTS,
            IPProtocolType.MOBILITY,
            IPProtocolType.HOSTIDENTITY,
            IPProtocolType.SHIM6 };


        protected ByteArraySegment Header;


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


        public UInt16 Length
        {
            get => (ushort)((PayloadLength + 1) * 8);
        }


    }
}
