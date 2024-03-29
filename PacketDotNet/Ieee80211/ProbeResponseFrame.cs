﻿/*
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
    /// Probe response frames are sent by Access Points in response to probe requests by stations.
    /// An access point may respond to a probe request if it hosts a network with parameters compatible with those
    /// requested by the station.
    /// </summary>
    public sealed class ProbeResponseFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ProbeResponseFrame(ByteArraySegment byteArraySegment)
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

            if (byteArraySegment.Length > ProbeResponseFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                var infoElementsSegment = new ByteArraySegment(byteArraySegment.Bytes,
                                                               byteArraySegment.Offset + ProbeResponseFields.InformationElement1Position,
                                                               byteArraySegment.Length - ProbeResponseFields.InformationElement1Position);

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
        /// Initializes a new instance of the <see cref="ProbeResponseFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        /// Source address.
        /// </param>
        /// <param name='destinationAddress'>
        /// Destination address.
        /// </param>
        /// <param name='bssId'>
        /// Bss identifier (Mac address of the access point).
        /// </param>
        /// <param name='informationElements'>
        /// Information elements.
        /// </param>
        public ProbeResponseFrame
        (
            PhysicalAddress sourceAddress,
            PhysicalAddress destinationAddress,
            PhysicalAddress bssId,
            InformationElementList informationElements)
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            DestinationAddress = destinationAddress;
            SourceAddress = sourceAddress;
            BssId = bssId;
            SequenceControl = new SequenceControlField();
            CapabilityInformation = new CapabilityInformationField();
            InformationElements = new InformationElementList(informationElements);

            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementProbeResponse;
        }

        /// <summary>
        /// Gets or sets the beacon interval. This is the minimum time between beacon frames from the access point.
        /// </summary>
        /// <value>
        /// The beacon interval.
        /// </value>
        public ushort BeaconInterval { get; set; }

        /// <summary>
        /// Get or set the capability information field that defines the capabilities of the network.
        /// </summary>
        public CapabilityInformationField CapabilityInformation { get; set; }

        /// <summary>
        /// Length of the frame header.
        /// This does not include the FCS, it represents only the header bytes that would
        /// would proceed any payload.
        /// </summary>
        public override int FrameSize => MacFields.FrameControlLength +
                                         MacFields.DurationIDLength +
                                         (MacFields.AddressLength * 3) +
                                         MacFields.SequenceControlLength +
                                         ProbeResponseFields.TimestampLength +
                                         ProbeResponseFields.BeaconIntervalLength +
                                         ProbeResponseFields.CapabilityInformationLength +
                                         InformationElements.Length;

        /// <summary>
        /// Gets or sets the information elements included in the frame.
        /// </summary>
        /// <value>
        /// The information elements.
        /// </value>
        public InformationElementList InformationElements { get; set; }

        /// <summary>
        /// Gets or sets the timestamp. The timestamp is used by a station to ensure that it
        /// is using the most up to date parameters for the network.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public ulong Timestamp { get; set; }

        private ushort BeaconIntervalBytes
        {
            get
            {
                if (Header.Length >= ProbeResponseFields.BeaconIntervalPosition + ProbeResponseFields.BeaconIntervalLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes, Header.Offset + ProbeResponseFields.BeaconIntervalPosition);
                }

                return 0;
            }
        }

        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        private ushort CapabilityInformationBytes
        {
            get
            {
                if (Header.Length >=
                    ProbeResponseFields.CapabilityInformationPosition + ProbeResponseFields.CapabilityInformationLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + ProbeResponseFields.CapabilityInformationPosition);
                }

                return 0;
            }
            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + ProbeResponseFields.CapabilityInformationPosition);
        }

        private ulong TimestampBytes
        {
            get
            {
                if (Header.Length >= ProbeResponseFields.TimestampPosition + ProbeResponseFields.TimestampLength)
                {
                    return EndianBitConverter.Little.ToUInt64(Header.Bytes, Header.Offset + ProbeResponseFields.TimestampPosition);
                }

                return 0;
            }
        }

        /// <summary>
        /// Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (Header == null || Header.Length > Header.BytesLength - Header.Offset || Header.Length < FrameSize)
            {
                Header = new ByteArraySegment(new byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SetAddress(0, DestinationAddress);
            SetAddress(1, SourceAddress);
            SetAddress(2, BssId);
            SequenceControlBytes = SequenceControl.Field;
            CapabilityInformationBytes = CapabilityInformation.Field;

            //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
            InformationElements.CopyTo(Header, Header.Offset + ProbeResponseFields.InformationElement1Position);

            Header.Length = FrameSize;
        }
    }