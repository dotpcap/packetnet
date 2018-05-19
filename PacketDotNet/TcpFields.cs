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
    /// <summary>
    /// IP protocol field encoding information.
    /// </summary>
    public struct TcpFields
    {
#pragma warning disable 1591
        // flag bitmasks
        public static readonly Int32 TCPNsMask = 0x0100;
        public static readonly Int32 TCPCwrMask = 0x0080;
        public static readonly Int32 TCPEcnMask = 0x0040;
        public static readonly Int32 TCPUrgMask = 0x0020;
        public static readonly Int32 TCPAckMask = 0x0010;
        public static readonly Int32 TCPPshMask = 0x0008;
        public static readonly Int32 TCPRstMask = 0x0004;
        public static readonly Int32 TCPSynMask = 0x0002;
        public static readonly Int32 TCPFinMask = 0x0001;
#pragma warning restore 1591

        /// <summary> Length of a TCP port in bytes.</summary>
        public static readonly Int32 PortLength = 2;

        /// <summary> Length of the sequence number in bytes.</summary>
        public static readonly Int32 SequenceNumberLength = 4;

        /// <summary> Length of the acknowledgment number in bytes.</summary>
        public static readonly Int32 AckNumberLength = 4;

        /// <summary> Length of the data offset and flags field in bytes.</summary>
        public static readonly Int32 DataOffsetAndFlagsLength = 2;

        /// <summary> Length of the window size field in bytes.</summary>
        public static readonly Int32 WindowSizeLength = 2;

        /// <summary> Length of the checksum field in bytes.</summary>
        public static readonly Int32 ChecksumLength = 2;

        /// <summary> Length of the urgent field in bytes.</summary>
        public static readonly Int32 UrgentPointerLength = 2;

        /// <summary> Position of the source port field.</summary>
        public static readonly Int32 SourcePortPosition = 0;

        /// <summary> Position of the destination port field.</summary>
        public static readonly Int32 DestinationPortPosition;

        /// <summary> Position of the sequence number field.</summary>
        public static readonly Int32 SequenceNumberPosition;

        /// <summary> Position of the acknowledgment number field.</summary>
        public static readonly Int32 AckNumberPosition;

        /// <summary> Position of the data offset </summary>
        public static readonly Int32 DataOffsetAndFlagsPosition;

        /// <summary> Position of the window size field.</summary>
        public static readonly Int32 WindowSizePosition;

        /// <summary> Position of the checksum field.</summary>
        public static readonly Int32 ChecksumPosition;

        /// <summary> Position of the urgent pointer field.</summary>
        public static readonly Int32 UrgentPointerPosition;

        /// <summary> Length in bytes of a TCP header.</summary>
        public static readonly Int32 HeaderLength; // == 20

        static TcpFields()
        {
            DestinationPortPosition = PortLength;
            SequenceNumberPosition = DestinationPortPosition + PortLength;
            AckNumberPosition = SequenceNumberPosition + SequenceNumberLength;
            DataOffsetAndFlagsPosition = AckNumberPosition + AckNumberLength;
            WindowSizePosition = DataOffsetAndFlagsPosition + DataOffsetAndFlagsLength;
            ChecksumPosition = WindowSizePosition + WindowSizeLength;
            UrgentPointerPosition = ChecksumPosition + ChecksumLength;
            HeaderLength = UrgentPointerPosition + UrgentPointerLength;
        }
    }
}