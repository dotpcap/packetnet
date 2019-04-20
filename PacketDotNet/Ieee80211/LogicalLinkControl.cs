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
 *  Copyright 2017 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Threading;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Logical link control.
    /// See https://en.wikipedia.org/wiki/IEEE_802.2 and
    /// https://en.wikipedia.org/wiki/Logical_link_control for additional information.
    /// </summary>
    public class LogicalLinkControl : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PacketDotNet.Ieee80211.Ieee80211.LogicalLinkControl" /> class.
        /// </summary>
        /// <param name="byteArraySegment">byteArraySegment.</param>
        public LogicalLinkControl(ByteArraySegment byteArraySegment)
        {
            // set the header field, header field values are retrieved from this byte array
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = LogicalLinkControlFields.HeaderLength
            };

            // parse the payload via an EthernetPacket method
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => EthernetPacket.ParseNextSegment(Header,
                                                                                                           Type),
                                                                     LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets or sets the control.
        /// </summary>
        /// <value>The control.</value>
        public byte Control
        {
            get => (byte) ((ControlOrganizationCode >> 24) & 0xFF);
            set => throw new NotImplementedException("Control setter not implemented");
        }

        /// <summary>
        /// Gets or sets the destination service access point.
        /// </summary>
        /// <value>The dsap.</value>
        public byte Dsap
        {
            get => Header.Bytes[Header.Offset + LogicalLinkControlFields.DsapPosition];
            set => Header.Bytes[Header.Offset + LogicalLinkControlFields.DsapPosition] = value;
        }

        /// <summary>
        /// Gets or sets the organization code.
        /// </summary>
        /// <value>The organization code.</value>
        public uint OrganizationCode
        {
            get => (byte) (ControlOrganizationCode & 0x00FFFFFF);
            set => throw new NotImplementedException("OrganizationCode setter not implemented");
        }

        /// <summary>
        /// Gets or sets the source service access point.
        /// </summary>
        /// <value>The ssap.</value>
        public byte Ssap
        {
            get => Header.Bytes[Header.Offset + LogicalLinkControlFields.SsapPosition];
            set => Header.Bytes[Header.Offset + LogicalLinkControlFields.SsapPosition] = value;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public EthernetType Type
        {
            get => (EthernetType) EndianBitConverter.Big.ToInt16(Header.Bytes,
                                                                 Header.Offset + LogicalLinkControlFields.TypePosition);
            set
            {
                var val = (short) value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + LogicalLinkControlFields.TypePosition);
            }
        }

        /// <summary>
        /// Gets or sets the control organization code.
        /// </summary>
        /// <value>The control organization code.</value>
        protected uint ControlOrganizationCode
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes,
                                                   Header.Offset + LogicalLinkControlFields.ControlOrganizationPosition);
            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 Header.Bytes,
                                                 Header.Offset + LogicalLinkControlFields.ControlOrganizationPosition);
            }
        }
    }
}