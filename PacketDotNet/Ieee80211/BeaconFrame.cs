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
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Format of an 802.11 management beacon frame.
    ///     Beacon frames are used to annouce the existance of a wireless network. If an
    ///     access point has been configured to not broadcast its SSID then it may not transmit
    ///     beacon frames.
    /// </summary>
    public class BeaconFrame : ManagementFrame
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public BeaconFrame(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas);

            this.FrameControl = new FrameControlField(this.FrameControlBytes);
            this.Duration = new DurationField(this.DurationBytes);
            this.DestinationAddress = this.GetAddress(0);
            this.SourceAddress = this.GetAddress(1);
            this.BssId = this.GetAddress(2);
            this.SequenceControl = new SequenceControlField(this.SequenceControlBytes);
            this.Timestamp = this.TimestampBytes;
            this.BeaconInterval = this.BeaconIntervalBytes;
            this.CapabilityInformation = new CapabilityInformationField(this.CapabilityInformationBytes);

            if (bas.Length > BeaconFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment(bas.Bytes,
                    (bas.Offset + BeaconFields.InformationElement1Position),
                    (bas.Length - BeaconFields.InformationElement1Position));

                this.InformationElements = new InformationElementList(infoElementsSegment);
            }
            else
            {
                this.InformationElements = new InformationElementList();
            }

            //cant set length until after we have handled the information elements
            //as they vary in length
            this.HeaderByteArraySegment.Length = this.FrameSize;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.BeaconFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        ///     Source address.
        /// </param>
        /// <param name='bssId'>
        ///     Bss identifier (MAC Address of the Access Point).
        /// </param>
        /// <param name='informationElements'>
        ///     Information elements.
        /// </param>
        public BeaconFrame(PhysicalAddress sourceAddress,
            PhysicalAddress bssId,
            InformationElementList informationElements)
        {
            this.FrameControl = new FrameControlField();
            this.Duration = new DurationField();
            this.SequenceControl = new SequenceControlField();
            this.CapabilityInformation = new CapabilityInformationField();
            this.InformationElements = new InformationElementList(informationElements);
            this.FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementBeacon;
            this.SourceAddress = sourceAddress;
            this.DestinationAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");
            this.BssId = bssId;
            this.BeaconInterval = 100;
        }

        /// <summary>
        ///     Gets the size of the frame.
        /// </summary>
        /// <value>
        ///     The size of the frame.
        /// </value>
        public override Int32 FrameSize => (MacFields.FrameControlLength +
                                            MacFields.DurationIDLength +
                                            (MacFields.AddressLength * 3) +
                                            MacFields.SequenceControlLength +
                                            BeaconFields.TimestampLength +
                                            BeaconFields.BeaconIntervalLength +
                                            BeaconFields.CapabilityInformationLength + this.InformationElements.Length);

        /// <summary>
        ///     The number of "time units" between beacon frames.
        ///     A time unit is 1,024 microseconds. This interval is usually set to 100 which equates to approximately 100
        ///     milliseconds or 0.1 seconds.
        /// </summary>
        public UInt16 BeaconInterval { get; set; }

        /// <summary>
        ///     Defines the capabilities of the network.
        /// </summary>
        public CapabilityInformationField CapabilityInformation { get; set; }

        /// <summary>
        ///     The information elements included in the frame
        ///     Most (but not all) beacons frames will contain an Information element that contains the SSID.
        /// </summary>
        public InformationElementList InformationElements { get; private set; }


        /// <summary>
        ///     The number of microseconds the networks master timekeeper has been active.
        ///     Used for synchronisation between stations in an IBSS. When it reaches the maximum value the timestamp will wrap
        ///     (not very likely).
        /// </summary>
        public UInt64 Timestamp { get; set; }

        private UInt16 BeaconIntervalBytes
        {
            get
            {
                if (this.HeaderByteArraySegment.Length >=
                    (BeaconFields.BeaconIntervalPosition + BeaconFields.BeaconIntervalLength))
                {
                    return EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset + BeaconFields.BeaconIntervalPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + BeaconFields.BeaconIntervalPosition);
        }

        /// <summary>
        ///     Frame control bytes are the first two bytes of the frame
        /// </summary>
        private UInt16 CapabilityInformationBytes
        {
            get
            {
                if (this.HeaderByteArraySegment.Length >=
                    (BeaconFields.CapabilityInformationPosition + BeaconFields.CapabilityInformationLength))
                {
                    return EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset + BeaconFields.CapabilityInformationPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + BeaconFields.CapabilityInformationPosition);
        }

        private UInt64 TimestampBytes
        {
            get
            {
                if (this.HeaderByteArraySegment.Length >=
                    (BeaconFields.TimestampPosition + BeaconFields.TimestampLength))
                {
                    return EndianBitConverter.Little.ToUInt64(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset + BeaconFields.TimestampPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + BeaconFields.TimestampPosition);
        }

        /// <summary>
        ///     Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if ((this.HeaderByteArraySegment == null) ||
                (this.HeaderByteArraySegment.Length >
                 (this.HeaderByteArraySegment.BytesLength - this.HeaderByteArraySegment.Offset)) ||
                (this.HeaderByteArraySegment.Length < this.FrameSize))
            {
                //the backing buffer isnt big enough to accommodate the info elements so we need to resize it
                this.HeaderByteArraySegment = new ByteArraySegment(new Byte[this.FrameSize]);
            }

            this.FrameControlBytes = this.FrameControl.Field;
            this.DurationBytes = this.Duration.Field;
            this.SetAddress(0, this.DestinationAddress);
            this.SetAddress(1, this.SourceAddress);
            this.SetAddress(2, this.BssId);
            this.SequenceControlBytes = this.SequenceControl.Field;
            this.TimestampBytes = this.Timestamp;
            this.BeaconIntervalBytes = this.BeaconInterval;
            this.CapabilityInformationBytes = this.CapabilityInformation.Field;

            //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
            this.InformationElements.CopyTo(this.HeaderByteArraySegment,
                this.HeaderByteArraySegment.Offset + BeaconFields.InformationElement1Position);

            this.HeaderByteArraySegment.Length = this.FrameSize;
        }

        private class BeaconFields
        {
            public static readonly Int32 BeaconIntervalLength = 2;
            public static readonly Int32 BeaconIntervalPosition;
            public static readonly Int32 CapabilityInformationLength = 2;
            public static readonly Int32 CapabilityInformationPosition;
            public static readonly Int32 InformationElement1Position;
            public static readonly Int32 TimestampLength = 8;

            public static readonly Int32 TimestampPosition;

            static BeaconFields()
            {
                TimestampPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                BeaconIntervalPosition = TimestampPosition + TimestampLength;
                CapabilityInformationPosition = BeaconIntervalPosition + BeaconIntervalLength;
                InformationElement1Position = CapabilityInformationPosition + CapabilityInformationLength;
            }
        }
    }
}