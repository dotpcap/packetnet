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

namespace PacketDotNet.Ieee80211
{
    #region Action Category Enums

    //The following enums define the category and type of action. At present these are 
    //not handled and parsed but they are left here for future reference as tracking them down 
    //was not that easy

    //enum ActionCategory
    //{
    //    SpectrumManagement = 0x0,
    //    Qos = 0x1,
    //    Dls = 0x2,
    //    BlockAck = 0x3,
    //    VendorSpecific = 0x127
    //}

    //enum SpectrumManagementAction
    //{
    //    MeasurementRequest = 0x0,
    //    MeasurementReport = 0x1,
    //    TpcRequest = 0x2,
    //    TpcReport = 0x3,
    //    ChannelSwitchAnnouncement = 0x4
    //}

    //enum QosAction
    //{
    //    TrafficSpecificationRequest = 0x0,
    //    TrafficSpecificationResponse = 0x1,
    //    TrafficSpecificationDelete = 0x2,
    //    Schedule = 0x3
    //}

    //enum DlsAction
    //{
    //    DlsRequest = 0x0,
    //    DlsResponse = 0x1,
    //    DlsTeardown = 0x2
    //}

    //enum BlockAcknowledgmentActions
    //{
    //    BlockAcknowledgmentRequest = 0x0,
    //    BlockAcknowledgmentResponse = 0x1,
    //    BlockAcknowledgmentDelete = 0x2
    //}

    #endregion


    /// <summary>
    /// Format of an 802.11 management action frame. These frames are used by the 802.11e (QoS) and 802.11n standards to request actions of stations.
    /// </summary>
    public sealed class ActionFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public ActionFrame(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);

            Header.Length = FrameSize;
            var availablePayloadLength = GetAvailablePayloadLength();
            if (availablePayloadLength > 0)
            {
                PayloadPacketOrData.Value.ByteArraySegment = Header.EncapsulatedBytes(availablePayloadLength);
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ActionFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        /// Source address.
        /// </param>
        /// <param name='destinationAddress'>
        /// Destination address.
        /// </param>
        /// <param name='bssId'>
        /// Bss identifier.
        /// </param>
        public ActionFrame
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

            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementAction;
        }

        /// <summary>
        /// Gets the size of the frame in bytes
        /// </summary>
        /// <value>
        /// The size of the frame.
        /// </value>
        public override Int32 FrameSize => MacFields.FrameControlLength +
                                           MacFields.DurationIDLength +
                                           (MacFields.AddressLength * 3) +
                                           MacFields.SequenceControlLength;

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
        }
    }
}