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
    /// Format of an 802.11 management beacon frame.
    /// Beacon frames are used to annouce the existance of a wireless network. If an
    /// access point has been configured to not broadcast its SSID then it may not transmit
    /// beacon frames.
    /// </summary>
    public sealed class BeaconFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public BeaconFrame(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);
            Timestamp = TimestampBytes;
            BeaconInterval = BeaconIntervalBytes;
            CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);

            if (bas.Length > BeaconFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                var infoElementsSegment = new ByteArraySegment(bas.Bytes,
                                                               bas.Offset + BeaconFields.InformationElement1Position,
                                                               bas.Length - BeaconFields.InformationElement1Position);

                InformationElements = new InformationElementList(infoElementsSegment);
            }
            else
            {
                InformationElements = new InformationElementList();
            }

            //cant set length until after we have handled the information elements
            //as they vary in length
            Header.Length = FrameSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeaconFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        /// Source address.
        /// </param>
        /// <param name='bssId'>
        /// Bss identifier (MAC Address of the Access Point).
        /// </param>
        /// <param name='informationElements'>
        /// Information elements.
        /// </param>
        public BeaconFrame
        (
            PhysicalAddress sourceAddress,
            PhysicalAddress bssId,
            InformationElementList informationElements)
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            SequenceControl = new SequenceControlField();
            CapabilityInformation = new CapabilityInformationField();
            InformationElements = new InformationElementList(informationElements);
            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementBeacon;
            SourceAddress = sourceAddress;
            DestinationAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");
            BssId = bssId;
            BeaconInterval = 100;
        }

        /// <summary>
        /// The number of "time units" between beacon frames.
        /// A time unit is 1,024 microseconds. This interval is usually set to 100 which equates to approximately 100 milliseconds or 0.1 seconds.
        /// </summary>
        public UInt16 BeaconInterval { get; set; }

        /// <summary>
        /// Defines the capabilities of the network.
        /// </summary>
        public CapabilityInformationField CapabilityInformation { get; set; }

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
                                           BeaconFields.TimestampLength +
                                           BeaconFields.BeaconIntervalLength +
                                           BeaconFields.CapabilityInformationLength +
                                           InformationElements.Length;

        /// <summary>
        /// The information elements included in the frame
        /// Most (but not all) beacons frames will contain an Information element that contains the SSID.
        /// </summary>
        public InformationElementList InformationElements { get; private set; }


        /// <summary>
        /// The number of microseconds the networks master timekeeper has been active.
        /// Used for synchronisation between stations in an IBSS. When it reaches the maximum value the timestamp will wrap (not very likely).
        /// </summary>
        public UInt64 Timestamp { get; set; }

        private UInt16 BeaconIntervalBytes
        {
            get
            {
                if (Header.Length >= BeaconFields.BeaconIntervalPosition + BeaconFields.BeaconIntervalLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes, Header.Offset + BeaconFields.BeaconIntervalPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + BeaconFields.BeaconIntervalPosition);
        }

        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        private UInt16 CapabilityInformationBytes
        {
            get
            {
                if (Header.Length >= BeaconFields.CapabilityInformationPosition + BeaconFields.CapabilityInformationLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + BeaconFields.CapabilityInformationPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + BeaconFields.CapabilityInformationPosition);
        }

        private UInt64 TimestampBytes
        {
            get
            {
                if (Header.Length >= BeaconFields.TimestampPosition + BeaconFields.TimestampLength)
                {
                    return EndianBitConverter.Little.ToUInt64(Header.Bytes, Header.Offset + BeaconFields.TimestampPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + BeaconFields.TimestampPosition);
        }

        /// <summary>
        /// Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (Header == null || Header.Length > Header.BytesLength - Header.Offset || Header.Length < FrameSize)
            {
                //the backing buffer isnt big enough to accommodate the info elements so we need to resize it
                Header = new ByteArraySegment(new Byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SetAddress(0, DestinationAddress);
            SetAddress(1, SourceAddress);
            SetAddress(2, BssId);
            SequenceControlBytes = SequenceControl.Field;
            TimestampBytes = Timestamp;
            BeaconIntervalBytes = BeaconInterval;
            CapabilityInformationBytes = CapabilityInformation.Field;

            //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
            InformationElements.CopyTo(Header, Header.Offset + BeaconFields.InformationElement1Position);

            Header.Length = FrameSize;
        }
    }
}