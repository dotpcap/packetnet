/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Represents an IGMP packet. IGMP is a network layer protocol used to set up multicasting on networks that use IPv4.
    /// IGMP Version 2 is defined in RFC 2236.
    /// IGMP Version 3 is defined in RFC 3376.
    /// </summary>
    public abstract class IgmpPacket : InternetPacket
    {
        /// <summary>
        /// Gets or sets the IGMP message type.
        /// </summary>
        public virtual IgmpMessageType Type { get; set; }

        /// <summary>
        /// Constructs an IGMP packet.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="parentPacket">The parent packet.</param>
        /// <returns><see cref="IgmpPacket" />.</returns>
        public static IgmpPacket ConstructIgmpPacket(ByteArraySegment payload, Packet parentPacket)
        {
            switch ((IgmpMessageType) payload.Bytes[payload.Offset + IgmpV2Fields.TypePosition])
            {
                case IgmpMessageType.MembershipQuery:
                    if (payload.Length >= 12)
                        return new IgmpV3MembershipQueryPacket(payload, parentPacket);
                    else
                        return new IgmpV2Packet(payload, parentPacket);
                case IgmpMessageType.MembershipReportIGMPv2:
                case IgmpMessageType.LeaveGroup:
                    return new IgmpV2Packet(payload, parentPacket);
                case IgmpMessageType.MembershipReportIGMPv3:
                    return new IgmpV3MembershipReportPacket(payload, parentPacket);
                default:
                    return null;
            }
        }
    }
}