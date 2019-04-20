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
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
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
                case OspfVersion.OSPFv2:
                {
                    return ConstructV2Packet(payload, offset);
                }
                case OspfVersion.OSPFv3:
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