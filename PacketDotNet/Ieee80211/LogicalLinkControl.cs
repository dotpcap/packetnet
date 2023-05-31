/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Ieee80211;

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
            PayloadPacketOrData = new LazySlim<PacketOrByteArraySegment>(() => EthernetPacket.ParseNextSegment(Header,
                                                                                                           Type));
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