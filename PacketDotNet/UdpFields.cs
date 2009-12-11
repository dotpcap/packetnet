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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
namespace PacketDotNet
{
    /// <summary>
    /// Defines the lengths and positions of the udp fields within
    /// a udp packet
    /// </summary>
    public struct UdpFields
    {
        /// <summary> Length of a UDP port in bytes.</summary>
        public readonly static int PortLength = 2;
        /// <summary> Length of the header length field in bytes.</summary>
        public readonly static int HeaderLengthLength = 2;
        /// <summary> Length of the checksum field in bytes.</summary>
        public readonly static int ChecksumLength = 2;
        /// <summary> Position of the source port.</summary>
        public readonly static int SourcePortPosition = 0;
        /// <summary> Position of the destination port.</summary>
        public readonly static int DestinationPortPosition;
        /// <summary> Position of the header length.</summary>
        public readonly static int HeaderLengthPosition;
        /// <summary> Position of the header checksum length.</summary>
        public readonly static int ChecksumPosition;
        /// <summary> Length of a UDP header in bytes.</summary>
        public readonly static int HeaderLength; // == 8

        static UdpFields()
        {
            DestinationPortPosition = PortLength;
            HeaderLengthPosition = DestinationPortPosition + PortLength;
            ChecksumPosition = HeaderLengthPosition + HeaderLengthLength;
            HeaderLength = ChecksumPosition + ChecksumLength;
        }
    }
}
