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
using System.Net.NetworkInformation;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using System.IO;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Logical link control.
        /// See https://en.wikipedia.org/wiki/IEEE_802.2 and
        /// https://en.wikipedia.org/wiki/Logical_link_control for additional information.
        /// </summary>
        public class LogicalLinkControl : Packet
        {
#if DEBUG
            private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

            /// <summary>
            /// Gets or sets the destination service access point.
            /// </summary>
            /// <value>The dsap.</value>
            public Byte DSAP
            {
                get => this.header.Bytes[this.header.Offset + LogicalLinkControlFields.DsapPosition];

                set => this.header.Bytes[this.header.Offset + LogicalLinkControlFields.DsapPosition] = value;
            }

            /// <summary>
            /// Gets or sets the source service access point.
            /// </summary>
            /// <value>The ssap.</value>
            public Byte SSAP
            {
                get => this.header.Bytes[this.header.Offset + LogicalLinkControlFields.SsapPosition];

                set => this.header.Bytes[this.header.Offset + LogicalLinkControlFields.SsapPosition] = value;
            }

            /// <summary>
            /// Gets or sets the control organization code.
            /// </summary>
            /// <value>The control organization code.</value>
            protected UInt32 ControlOrganizationCode
            {
                get => EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + LogicalLinkControlFields.ControlOrganizationPosition);

                set
                {
                    var val = (UInt32)value;
                    EndianBitConverter.Big.CopyBytes(val, this.header.Bytes, this.header.Offset + LogicalLinkControlFields.ControlOrganizationPosition);
                }
            }

            /// <summary>
            /// Gets or sets the control.
            /// </summary>
            /// <value>The control.</value>
            public Byte Control
            {
                get => (Byte)((this.ControlOrganizationCode >> 24) & 0xFF);

                set => throw new NotImplementedException("Control setter not implemented");
            }

            /// <summary>
            /// Gets or sets the organization code.
            /// </summary>
            /// <value>The organization code.</value>
            public UInt32 OrganizationCode
            {
                get => (Byte)((this.ControlOrganizationCode & 0x00FFFFFF));

                set => throw new NotImplementedException("OrganizationCode setter not implemented");
            }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            public EthernetPacketType Type
            {
                get => (EthernetPacketType)EndianBitConverter.Big.ToInt16(this.header.Bytes, this.header.Offset + LogicalLinkControlFields.TypePosition);

                set
                {
                    Int16 val = (Int16)value;
                    EndianBitConverter.Big.CopyBytes(val, this.header.Bytes, this.header.Offset + LogicalLinkControlFields.TypePosition);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:PacketDotNet.Ieee80211.LogicalLinkControl"/> class.
            /// </summary>
            /// <param name="bas">Bas.</param>
            public LogicalLinkControl(ByteArraySegment bas)
            {
                // set the header field, header field values are retrieved from this byte array
                this.header = new ByteArraySegment(bas);
                this.header.Length = LogicalLinkControlFields.HeaderLength;

                // parse the payload via an EthernetPacket method
                this.payloadPacketOrData = EthernetPacket.ParseEncapsulatedBytes(this.header, this.Type);
            }
        }
    }
}
