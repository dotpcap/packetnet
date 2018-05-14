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
    /// Format of an 802.11 management association response frame.
    /// </summary>
    public sealed class AssociationResponseFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public AssociationResponseFrame(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);

            CapabilityInformation = new CapabilityInformationField(CapabilityInformationBytes);
            StatusCode = StatusCodeBytes;
            AssociationId = AssociationIdBytes;

            if (bas.Length > AssociationResponseFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                var infoElementsSegment = new ByteArraySegment(bas.Bytes,
                                                               bas.Offset + AssociationResponseFields.InformationElement1Position,
                                                               bas.Length - AssociationResponseFields.InformationElement1Position);

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
        /// Initializes a new instance of the <see cref="AssociationResponseFrame" /> class.
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
        public AssociationResponseFrame
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

            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementAssociationResponse;
        }

        /// <summary>
        /// The id assigned to the station by the access point to assist in management and control functions.
        /// Although this is a 16bit field only 14 of the bits are used to represent the id. Therefore the available values
        /// for this field are inthe range 1-2,007.
        /// </summary>
        public UInt16 AssociationId { get; set; }

        /// <summary>
        /// The capability information field that describes the networks capabilities.
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
                                           AssociationResponseFields.CapabilityInformationLength +
                                           AssociationResponseFields.StatusCodeLength +
                                           AssociationResponseFields.AssociationIdLength +
                                           InformationElements.Length;

        /// <summary>
        /// The information elements included in the frame
        /// </summary>
        public InformationElementList InformationElements { get; set; }

        /// <summary>
        /// Value indicating the success or failure of the association.
        /// </summary>
        public AuthenticationStatusCode StatusCode { get; set; }

        private UInt16 AssociationIdBytes
        {
            get
            {
                if (Header.Length >= AssociationResponseFields.AssociationIdPosition + AssociationResponseFields.AssociationIdLength)
                {
                    var associationID = EndianBitConverter.Little.ToUInt16(Header.Bytes, Header.Offset + AssociationResponseFields.AssociationIdPosition);
                    return (UInt16) (associationID & 0xCF);
                }

                return 0;
            }

            set
            {
                var associationID = (UInt16) (value & 0xCF);
                EndianBitConverter.Little.CopyBytes(associationID,
                                                    Header.Bytes,
                                                    Header.Offset + AssociationResponseFields.AssociationIdPosition);
            }
        }

        /// <summary>
        /// The raw capability information bytes
        /// </summary>
        private UInt16 CapabilityInformationBytes
        {
            get
            {
                if (Header.Length >=
                    AssociationResponseFields.CapabilityInformationPosition + AssociationResponseFields.CapabilityInformationLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + AssociationResponseFields.CapabilityInformationPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + AssociationResponseFields.CapabilityInformationPosition);
        }

        private AuthenticationStatusCode StatusCodeBytes
        {
            get
            {
                if (Header.Length >= AssociationResponseFields.StatusCodePosition + AssociationResponseFields.StatusCodeLength)
                {
                    return (AuthenticationStatusCode) EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                                                         Header.Offset + AssociationResponseFields.StatusCodePosition);
                }

                //This seems the most sensible value to return when it is not possible
                //to extract a meaningful value
                return AuthenticationStatusCode.UnspecifiedFailure;
            }

            set => EndianBitConverter.Little.CopyBytes((UInt16) value,
                                                       Header.Bytes,
                                                       Header.Offset + AssociationResponseFields.StatusCodePosition);
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
            StatusCodeBytes = StatusCode;
            AssociationIdBytes = AssociationId;

            //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
            InformationElements.CopyTo(Header, Header.Offset + AssociationResponseFields.InformationElement1Position);

            Header.Length = FrameSize;
        }
    }
}