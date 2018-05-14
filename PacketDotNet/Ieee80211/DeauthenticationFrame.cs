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

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Deauthentication frame.
        /// </summary>
        public class DeauthenticationFrame : ManagementFrame
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment" />
            /// </param>
            public DeauthenticationFrame(ByteArraySegment bas)
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
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.DeauthenticationFrame" /> class.
            /// </summary>
            /// <param name='SourceAddress'>
            /// Source address.
            /// </param>
            /// <param name='DestinationAddress'>
            /// Destination address.
            /// </param>
            /// <param name='BssId'>
            /// Bss identifier (MAC Address of the Access Point).
            /// </param>
            public DeauthenticationFrame
            (
                PhysicalAddress SourceAddress,
                PhysicalAddress DestinationAddress,
                PhysicalAddress BssId)
            {
                FrameControl = new FrameControlField();
                Duration = new DurationField();
                this.DestinationAddress = DestinationAddress;
                this.SourceAddress = SourceAddress;
                this.BssId = BssId;
                SequenceControl = new SequenceControlField();

                FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementDeauthentication;
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
                                               DeauthenticationFields.ReasonCodeLength;

            /// <summary>
            /// Gets the reason for deauthentication.
            /// </summary>
            /// <value>
            /// The reason.
            /// </value>
            public ReasonCode Reason { get; set; }

            private ReasonCode ReasonBytes
            {
                get
                {
                    if (Header.Length >= DeauthenticationFields.ReasonCodePosition + DeauthenticationFields.ReasonCodeLength)
                    {
                        return (ReasonCode) EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                                               Header.Offset + DeauthenticationFields.ReasonCodePosition);
                    }

                    return ReasonCode.Unspecified;
                }

                set => EndianBitConverter.Little.CopyBytes((UInt16) value,
                                                           Header.Bytes,
                                                           Header.Offset + DeauthenticationFields.ReasonCodePosition);
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

            private class DeauthenticationFields
            {
                public static readonly Int32 ReasonCodeLength = 2;

                public static readonly Int32 ReasonCodePosition;

                static DeauthenticationFields()
                {
                    ReasonCodePosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                }
            }
        }
    }
}