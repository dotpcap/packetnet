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
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Disassociation frame.
    /// </summary>
    public sealed class DisassociationFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public DisassociationFrame(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);
            Reason = ReasonBytes;

            Header.Length = FrameSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisassociationFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        /// Source address.
        /// </param>
        /// <param name='destinationAddress'>
        /// Destination address.
        /// </param>
        /// <param name='bssId'>
        /// Bss identifier (MAC Address of the Access Point).
        /// </param>
        public DisassociationFrame
        (
            PhysicalAddress sourceAddress,
            PhysicalAddress destinationAddress,
            PhysicalAddress bssId)
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            DestinationAddress = destinationAddress;
            SourceAddress = sourceAddress;
            BssId = bssId;
            SequenceControl = new SequenceControlField();

            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementDisassociation;
        }

        /// <summary>
        /// Gets the size of the frame.
        /// </summary>
        /// <value>
        /// The size of the frame.
        /// </value>
        public override Int32 FrameSize => MacFields.FrameControlLength +
                                           MacFields.DurationIDLength +
                                           (MacFields.AddressLength * 3) +
                                           MacFields.SequenceControlLength +
                                           DisassociationFields.ReasonCodeLength;

        /// <summary>
        /// Gets or sets the reason for disassociation.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public ReasonCode Reason { get; set; }

        private ReasonCode ReasonBytes
        {
            get
            {
                if (Header.Length >= DisassociationFields.ReasonCodePosition + DisassociationFields.ReasonCodeLength)
                {
                    return (ReasonCode) EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                                           Header.Offset + DisassociationFields.ReasonCodePosition);
                }

                return ReasonCode.Unspecified;
            }

            set => EndianBitConverter.Little.CopyBytes((UInt16) value,
                                                       Header.Bytes,
                                                       Header.Offset + DisassociationFields.ReasonCodePosition);
        }

        /// <summary>
        /// Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (Header == null || Header.Length > Header.BytesLength - Header.Offset || Header.Length < FrameSize)
            {
                Header = new ByteArraySegment(new Byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SetAddress(0, DestinationAddress);
            SetAddress(1, SourceAddress);
            SetAddress(2, BssId);
            SequenceControlBytes = SequenceControl.Field;
            ReasonBytes = Reason;

            Header.Length = FrameSize;
        }

        private class DisassociationFields
        {
            public static readonly Int32 ReasonCodeLength = 2;

            public static readonly Int32 ReasonCodePosition;

            static DisassociationFields()
            {
                ReasonCodePosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            }
        }
    }
}