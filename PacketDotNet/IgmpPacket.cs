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
        /// Constructs the right version and type of an OspfPacket
        /// </summary>
        /// <param name="payload">A ByteArraySegment</param>
        /// <param name="parentPacket">The parent packet</param>
        /// <returns>an IGMP packet</returns>
        public static IgmpPacket ConstructIgmpPacket(ByteArraySegment payload, Packet parentPacket)
        {
            var type = (IgmpMessageType)payload.Bytes[payload.Offset + IgmpV2Fields.TypePosition];

            switch (type)
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
