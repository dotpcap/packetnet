/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System.Net.NetworkInformation;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Ieee80211;

    /// <summary>
    /// Format of an 802.11 management beacon frame.
    /// Beacon frames are used to announce the existence of a wireless network. If an
    /// access point has been configured to not broadcast its SSID then it may not transmit
    /// beacon frames.
    /// </summary>
    public sealed class BeaconFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public BeaconFrame(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);
            Timestamp = TimestampBytes;
            BeaconInterval = BeaconIntervalBytes;
            CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);

            if (byteArraySegment.Length > BeaconFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                var infoElementsSegment = new ByteArraySegment(byteArraySegment.Bytes,
                                                               byteArraySegment.Offset + BeaconFields.InformationElement1Position,
                                                               byteArraySegment.Length - BeaconFields.InformationElement1Position);

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
        public ushort BeaconInterval { get; set; }

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
        public override int FrameSize => MacFields.FrameControlLength +
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
        public InformationElementList InformationElements { get; }

        /// <summary>
        /// The number of microseconds the networks master timekeeper has been active.
        /// Used for synchronization between stations in an IBSS. When it reaches the maximum value the timestamp will wrap (not very likely).
        /// </summary>
        public ulong Timestamp { get; set; }

        private ushort BeaconIntervalBytes
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
        private ushort CapabilityInformationBytes
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

        private ulong TimestampBytes
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
                //the backing buffer isn't big enough to accommodate the info elements so we need to resize it
                Header = new ByteArraySegment(new byte[FrameSize]);
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