/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet
{
    /// <summary>
    /// IP protocol field encoding information.
    /// </summary>
    public struct TcpFields
    {
        // flag bit masks
        public static readonly int NonceSumMask = 0x0100;
        public static readonly int CongestionWindowReducedMask = 0x0080;
        public static readonly int ExplicitCongestionNotificationEchoMask = 0x0040;
        public static readonly int UrgentMask = 0x0020;
        public static readonly int TCPAckMask = 0x0010;
        public static readonly int PushMask = 0x0008;
        public static readonly int ResetMask = 0x0004;
        public static readonly int SynchronizationMask = 0x0002;
        public static readonly int FinishedMask = 0x0001;

        /// <summary>Length of a TCP port in bytes.</summary>
        public static readonly int PortLength = 2;

        /// <summary>Length of the sequence number in bytes.</summary>
        public static readonly int SequenceNumberLength = 4;

        /// <summary>Length of the acknowledgment number in bytes.</summary>
        public static readonly int AcknowledgmentNumberLength = 4;

        /// <summary>Length of the data offset and flags field in bytes.</summary>
        public static readonly int DataOffsetAndFlagsLength = 2;

        /// <summary>Length of the window size field in bytes.</summary>
        public static readonly int WindowSizeLength = 2;

        /// <summary>Length of the checksum field in bytes.</summary>
        public static readonly int ChecksumLength = 2;

        /// <summary>Length of the urgent field in bytes.</summary>
        public static readonly int UrgentPointerLength = 2;

        /// <summary>Position of the source port field.</summary>
        public static readonly int SourcePortPosition = 0;

        /// <summary>Position of the destination port field.</summary>
        public static readonly int DestinationPortPosition;

        /// <summary>Position of the sequence number field.</summary>
        public static readonly int SequenceNumberPosition;

        /// <summary>Position of the acknowledgment number field.</summary>
        public static readonly int AcknowledgmentNumberPosition;

        /// <summary>Position of the data offset </summary>
        public static readonly int DataOffsetAndFlagsPosition;

        /// <summary>Position of the window size field.</summary>
        public static readonly int WindowSizePosition;

        /// <summary>Position of the checksum field.</summary>
        public static readonly int ChecksumPosition;

        /// <summary>Position of the urgent pointer field.</summary>
        public static readonly int UrgentPointerPosition;

        /// <summary>Length in bytes of a TCP header.</summary>
        public static readonly int HeaderLength; // == 20

        static TcpFields()
        {
            DestinationPortPosition = PortLength;
            SequenceNumberPosition = DestinationPortPosition + PortLength;
            AcknowledgmentNumberPosition = SequenceNumberPosition + SequenceNumberLength;
            DataOffsetAndFlagsPosition = AcknowledgmentNumberPosition + AcknowledgmentNumberLength;
            WindowSizePosition = DataOffsetAndFlagsPosition + DataOffsetAndFlagsLength;
            ChecksumPosition = WindowSizePosition + WindowSizeLength;
            UrgentPointerPosition = ChecksumPosition + ChecksumLength;
            HeaderLength = UrgentPointerPosition + UrgentPointerLength;
        }
    }
}