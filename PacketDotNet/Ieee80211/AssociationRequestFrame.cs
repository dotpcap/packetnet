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
    /// Format of an 802.11 management association frame.
    /// </summary>
    public sealed class AssociationRequestFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public AssociationRequestFrame(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);

            CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);
            ListenInterval = ListenIntervalBytes;

            if (bas.Length > AssociationRequestFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                var infoElementsSegment = new ByteArraySegment(bas.Bytes,
                                                               bas.Offset + AssociationRequestFields.InformationElement1Position,
                                                               bas.Length - AssociationRequestFields.InformationElement1Position);

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
        /// Initializes a new instance of the <see cref="AssociationRequestFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        /// Source address.
        /// </param>
        /// <param name='destinationAddress'>
        /// Destination address.
        /// </param>
        /// <param name='bssId'>
        /// Bss identifier (MAC Address of Access Point).
        /// </param>
        /// <param name='informationElements'>
        /// Information elements.
        /// </param>
        public AssociationRequestFrame
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

            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementAssociationRequest;
        }

        /// <summary>
        /// Gets or sets the capability information.
        /// </summary>
        /// <value>
        /// The capability information.
        /// </value>
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
                                           AssociationRequestFields.CapabilityInformationLength +
                                           AssociationRequestFields.ListenIntervalLength +
                                           InformationElements.Length;

        /// <summary>
        /// Gets or sets the information elements.
        /// </summary>
        /// <value>
        /// The information elements.
        /// </value>
        public InformationElementList InformationElements { get; set; }

        /// <summary>
        /// Gets or sets the listen interval.
        /// </summary>
        /// <value>
        /// The listen interval.
        /// </value>
        public UInt16 ListenInterval { get; set; }

        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        private UInt16 CapabilityInformationBytes
        {
            get
            {
                if (Header.Length >=
                    AssociationRequestFields.CapabilityInformationPosition + AssociationRequestFields.CapabilityInformationLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + AssociationRequestFields.CapabilityInformationPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + AssociationRequestFields.CapabilityInformationPosition);
        }

        private UInt16 ListenIntervalBytes
        {
            get
            {
                if (Header.Length >= AssociationRequestFields.ListenIntervalPosition + AssociationRequestFields.ListenIntervalLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + AssociationRequestFields.ListenIntervalPosition);
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
                Header = new ByteArraySegment(new Byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SetAddress(0, DestinationAddress);
            SetAddress(1, SourceAddress);
            SetAddress(2, BssId);
            SequenceControlBytes = SequenceControl.Field;
            CapabilityInformationBytes = CapabilityInformation.Field;

            //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
            InformationElements.CopyTo(Header, Header.Offset + AssociationRequestFields.InformationElement1Position);

            Header.Length = FrameSize;
        }
    }
}