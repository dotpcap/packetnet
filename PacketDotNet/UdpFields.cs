/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// Defines the lengths and positions of the udp fields within
    /// a udp packet
    /// </summary>
    public struct UdpFields
    {
        /// <summary>Length of a UDP port in bytes.</summary>
        public static readonly int PortLength = 2;

        /// <summary>Length of the header length field in bytes.</summary>
        public static readonly int HeaderLengthLength = 2;

        /// <summary>Length of the checksum field in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Position of the source port.</summary>
        public static readonly int SourcePortPosition = 0;

        /// <summary>Position of the destination port.</summary>
        public static readonly int DestinationPortPosition;

        /// <summary>Position of the header length.</summary>
        public static readonly int HeaderLengthPosition;

        /// <summary>Position of the header checksum length.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Length of a UDP header in bytes.</summary>
        public static readonly int HeaderLength; // == 8

        static UdpFields()
        {
            DestinationPortPosition = PortLength;
            HeaderLengthPosition = DestinationPortPosition + PortLength;
            ChecksumPosition = HeaderLengthPosition + HeaderLengthLength;
            HeaderLength = ChecksumPosition + ChecksumLength;
        }
    }
}