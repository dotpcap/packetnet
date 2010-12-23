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

using System;

namespace PacketDotNet
{
    /// <summary> IP protocol field encoding information.
    ///
    /// </summary>
    public struct TcpFields
    {
#pragma warning disable 1591
        // flag bitmasks
        public readonly static int TCP_CWR_MASK = 0x0080;
        public readonly static int TCP_ECN_MASK = 0x0040;
        public readonly static int TCP_URG_MASK = 0x0020;
        public readonly static int TCP_ACK_MASK = 0x0010;
        public readonly static int TCP_PSH_MASK = 0x0008;
        public readonly static int TCP_RST_MASK = 0x0004;
        public readonly static int TCP_SYN_MASK = 0x0002;
        public readonly static int TCP_FIN_MASK = 0x0001;
#pragma warning restore 1591

        /// <summary> Length of a TCP port in bytes.</summary>
        public readonly static int PortLength = 2;

        /// <summary> Length of the sequence number in bytes.</summary>
        public readonly static int SequenceNumberLength = 4;
        /// <summary> Length of the acknowledgment number in bytes.</summary>
        public readonly static int AckNumberLength = 4;
        /// <summary> Length of the data offset and flags field in bytes.</summary>
        public readonly static int DataOffsetLength = 1;
        /// <summary> The length of the flags field </summary>
        public readonly static int FlagsLength = 1;
        /// <summary> Length of the window size field in bytes.</summary>
        public readonly static int WindowSizeLength = 2;
        /// <summary> Length of the checksum field in bytes.</summary>
        public readonly static int ChecksumLength = 2;
        /// <summary> Length of the urgent field in bytes.</summary>
        public readonly static int UrgentPointerLength = 2;

        /// <summary> Position of the source port field.</summary>
        public readonly static int SourcePortPosition = 0;
        /// <summary> Position of the destination port field.</summary>
        public readonly static int DestinationPortPosition;
        /// <summary> Position of the sequence number field.</summary>
        public readonly static int SequenceNumberPosition;
        /// <summary> Position of the acknowledgment number field.</summary>
        public readonly static int AckNumberPosition;
        /// <summary> Position of the data offset </summary>
        public readonly static int DataOffsetPosition;
        /// <summary> Position of the flags field </summary>
        public readonly static int FlagsPosition;
        /// <summary> Position of the window size field.</summary>
        public readonly static int WindowSizePosition;
        /// <summary> Position of the checksum field.</summary>
        public readonly static int ChecksumPosition;
        /// <summary> Position of the urgent pointer field.</summary>
        public readonly static int UrgentPointerPosition;

        /// <summary> Length in bytes of a TCP header.</summary>
        public readonly static int HeaderLength; // == 20

        static TcpFields()
        {
            DestinationPortPosition = PortLength;
            SequenceNumberPosition = DestinationPortPosition + PortLength;
            AckNumberPosition = SequenceNumberPosition + SequenceNumberLength;
            DataOffsetPosition = AckNumberPosition + AckNumberLength;
            FlagsPosition = DataOffsetPosition + DataOffsetLength;
            WindowSizePosition = FlagsPosition + FlagsLength;
            ChecksumPosition = WindowSizePosition + WindowSizeLength;
            UrgentPointerPosition = ChecksumPosition + ChecksumLength;
            HeaderLength = UrgentPointerPosition + UrgentPointerLength;
        }
    }
}
