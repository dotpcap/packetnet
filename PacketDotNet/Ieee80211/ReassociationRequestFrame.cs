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
    /// Reassociation request frame.
    /// Sent when a wireless client is going from one access point to another
    /// http://en.wikipedia.org/wiki/IEEE_802.11#Frames
    /// </summary>
    public sealed class ReassociationRequestFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ReassociationRequestFrame(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);

            CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);
            ListenInterval = ListenIntervalBytes;
            CurrentAccessPointAddress = CurrentAccessPointAddressBytes;

            if (byteArraySegment.Length > ReassociationRequestFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                var infoElementsSegment = new ByteArraySegment(byteArraySegment.Bytes,
                                                               byteArraySegment.Offset + ReassociationRequestFields.InformationElement1Position,
                                                               byteArraySegment.Length - ReassociationRequestFields.InformationElement1Position);

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
        /// Initializes a new instance of the <see cref="ReassociationRequestFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        /// Source address.
        /// </param>
        /// <param name='destinationAddress'>
        /// Destination address.
        /// </param>
        /// <param name='bssId'>
        /// BssId.
        /// </param>
        /// <param name='informationElements'>
        /// Information elements.
        /// </param>
        public ReassociationRequestFrame
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

            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementReassociationRequest;
        }

        /// <summary>
        /// Gets or sets the capability information, the type of network the mobile station wants to join
        /// </summary>
        /// <value>
        /// The capability information.
        /// </value>
        public CapabilityInformationField CapabilityInformation { get; set; }

        /// <summary>
        /// DestinationAddress
        /// </summary>
        public PhysicalAddress CurrentAccessPointAddress { get; set; }

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
                                         ReassociationRequestFields.CapabilityInformationLength +
                                         ReassociationRequestFields.ListenIntervalLength +
                                         MacFields.AddressLength +
                                         InformationElements.Length;

        /// <summary>
        /// Gets or sets the information elements.
        /// </summary>
        /// <value>
        /// The information elements.
        /// </value>
        public InformationElementList InformationElements { get; set; }

        /// <summary>
        /// Gets or sets the listen interval. This is the number of beacon interval time periods that the access
        /// point must retain buffered packets for.
        /// </summary>
        /// <value>
        /// The listen interval.
        /// </value>
        public ushort ListenInterval { get; set; }

        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        private ushort CapabilityInformationBytes
        {
            get
            {
                if (Header.Length >=
                    ReassociationRequestFields.CapabilityInformationPosition + ReassociationRequestFields.CapabilityInformationLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + ReassociationRequestFields.CapabilityInformationPosition);
                }

                return 0;
            }
            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + ReassociationRequestFields.CapabilityInformationPosition);
        }

        private PhysicalAddress CurrentAccessPointAddressBytes => GetAddressByOffset(Header.Offset + ReassociationRequestFields.CurrentAccessPointPosition);

        /// <summary>
        /// Gets or sets the listen interval, the length of buffered frame retention
        /// </summary>
        /// <value>
        /// The listen interval.
        /// </value>
        private ushort ListenIntervalBytes
        {
            get
            {
                if (Header.Length >=
                    ReassociationRequestFields.ListenIntervalPosition + ReassociationRequestFields.ListenIntervalLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + ReassociationRequestFields.ListenIntervalPosition);
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
            InformationElements.CopyTo(Header, Header.Offset + ReassociationRequestFields.InformationElement1Position);

            Header.Length = FrameSize;
        }
    }