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
    public abstract class OSPFPacket : Packet
    {
        /// <summary>
        /// Constructs the right version and type of an OSPFPacket
        /// </summary>
        /// <param name="payload">The bytes from which the packet is conctructed</param>
        /// <param name="offset">The offset of this packet from the parent packet</param>
        /// <returns>an OSPF packet</returns>
        public static OSPFPacket ConstructOSPFPacket(Byte[] payload, Int32 offset)
        {
            var v = (OSPFVersion) payload[offset + OSPFv2Fields.VersionPosition];

            switch (v)
            {
                case OSPFVersion.OSPFv2:
                    return ConstructV2Packet(payload, offset);
                case OSPFVersion.OSPFv3:
                    return ConstructV3Packet(payload, offset);
                default:
                    throw new InvalidOperationException("No such OSPF version: " + v);
            }
        }

        private static OSPFv2Packet ConstructV2Packet(Byte[] payload, Int32 offset)
        {
            OSPFv2Packet p;
            var type = (OSPFPacketType) payload[offset + OSPFv2Fields.TypePosition];

            switch (type)
            {
                case OSPFPacketType.Hello:
                    p = new OSPFv2HelloPacket(payload, offset);
                    break;
                case OSPFPacketType.DatabaseDescription:
                    p = new OSPFv2DDPacket(payload, offset);
                    break;
                case OSPFPacketType.LinkStateAcknowledgment:
                    p = new OSPFv2LSAPacket(payload, offset);
                    break;
                case OSPFPacketType.LinkStateRequest:
                    p = new OSPFv2LSRequestPacket(payload, offset);
                    break;
                case OSPFPacketType.LinkStateUpdate:
                    p = new OSPFv2LSUpdatePacket(payload, offset);
                    break;
                default:
                    throw new Exception("Malformed OSPF packet");
            }

            return p;
        }

        private static OSPFPacket ConstructV3Packet(Byte[] payload, Int32 offset)
        {
            throw new NotImplementedException("OSPFv3 is not supported yet");
        }
    }
}