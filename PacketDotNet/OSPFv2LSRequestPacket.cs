using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.LSA;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Link State Request packets are OSPF packet type 3.
    /// The Link State Request packet is used to request the pieces of the
    /// neighbor's database that are more up-to-date.
    /// See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public sealed class OSPFv2LSRequestPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType PacketType = OSPFPacketType.LinkStateRequest;

        /// <summary>
        /// Constructs an OSPFv2 LSR packet
        /// </summary>
        public OSPFv2LSRequestPacket()
        {
            Type = PacketType;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSR packet with link state requests
        /// </summary>
        /// <param name="lsrs">List of the link state requests</param>
        public OSPFv2LSRequestPacket(List<LinkStateRequest> lsrs)
        {
            var length = lsrs.Count * LinkStateRequest.Length;
            var offset = OSPFv2Fields.HeaderLength;
            var bytes = new Byte[length + OSPFv2Fields.HeaderLength];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in lsrs)
            {
                Array.Copy(t.Bytes, 0, bytes, offset, LinkStateRequest.Length);
                offset += LinkStateRequest.Length;
            }

            Header = new ByteArraySegment(bytes);
            Type = PacketType;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSR packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2LSRequestPacket(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        public OSPFv2LSRequestPacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Type = PacketType;
        }

        /// <summary>
        /// A list of link state requests, contained in this packet
        /// </summary>
        /// See
        /// <see cref="LinkStateRequest" />
        public List<LinkStateRequest> LinkStateRequests
        {
            get
            {
                var bytesNeeded = PacketLength - OSPFv2Fields.LSRStart;
                if (bytesNeeded % LinkStateRequest.Length != 0)
                {
                    throw new Exception("Malformed LSR packet - bad size for the LS requests");
                }

                var ret = new List<LinkStateRequest>();
                var offset = Header.Offset + OSPFv2Fields.LSRStart;
                var lsrCount = bytesNeeded / LinkStateRequest.Length;

                for (var i = 0; i < lsrCount; i++)
                {
                    var request = new LinkStateRequest(Header.Bytes, offset, LinkStateRequest.Length);
                    ret.Add(request);
                    offset += LinkStateRequest.Length;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2LSRequestPacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2LSRequestPacket" />.</returns>
        public override String ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("LSR count: {0} ", LinkStateRequests.Count);
            return packet.ToString();
        }

        /// <summary cref="Packet.ToString()">
        /// Output the packet information in the specified format
        /// Normal - outputs the packet info to a single line
        /// Colored - outputs the packet info to a single line with coloring
        /// Verbose - outputs detailed info about the packet
        /// VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="outputFormat">Output format.</param>
        public override String ToString(StringOutputType outputFormat)
        {
            return ToString();
        }
    }
}