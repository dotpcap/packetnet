/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;

namespace PacketDotNet
{
    /// <summary>
    /// Represents an OSPF packet. OSPF is a dynamic routing IGP protocol
    /// OSPF Version 2 (for IPv4) is defined in RFC 2328.
    /// OSPF Version 3 (for IPv6) is defined in RFC 5340.
    /// </summary>
    public abstract class OspfPacket : Packet
    {
        /// <summary>
        /// Constructs the right version and type of an OspfPacket
        /// </summary>
        /// <param name="payload">The bytes from which the packet is constructed</param>
        /// <param name="offset">The offset of this packet from the parent packet</param>
        /// <returns>an OSPF packet</returns>
        public static OspfPacket ConstructOspfPacket(byte[] payload, int offset)
        {
            var v = (OspfVersion) payload[offset + OspfV2Fields.VersionPosition];

            switch (v)
            {
                case OspfVersion.OspfV2:
                {
                    return ConstructV2Packet(payload, offset);
                }
                case OspfVersion.OspfV3:
                {
                    return ConstructV3Packet();
                }
                default:
                {
                    throw new InvalidOperationException("No such OSPF version: " + v);
                }
            }
        }

        private static OspfV2Packet ConstructV2Packet(byte[] payload, int offset)
        {
            OspfV2Packet p;
            var type = (OspfPacketType) payload[offset + OspfV2Fields.TypePosition];

            switch (type)
            {
                case OspfPacketType.Hello:
                {
                    p = new OspfV2HelloPacket(payload, offset);
                    break;
                }
                case OspfPacketType.DatabaseDescription:
                {
                    p = new OspfV2DatabaseDescriptorPacket(payload, offset);
                    break;
                }
                case OspfPacketType.LinkStateAcknowledgment:
                {
                    p = new OspfV2LinkStateAcknowledgmentPacket(payload, offset);
                    break;
                }
                case OspfPacketType.LinkStateRequest:
                {
                    p = new OspfV2LinkStateRequestPacket(payload, offset);
                    break;
                }
                case OspfPacketType.LinkStateUpdate:
                {
                    p = new OspfV2LinkStateUpdatePacket(payload, offset);
                    break;
                }
                default:
                {
                    throw new Exception("Malformed OSPF packet");
                }
            }

            return p;
        }

        private static OspfPacket ConstructV3Packet()
        {
            throw new NotImplementedException("OSPFv3 is not supported yet");
        }
    }
}