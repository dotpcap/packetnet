using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Link State Request packets are OSPF packet type 3.
    /// The Link State Request packet is used to request the pieces of the
    /// neighbor's database that are more up-to-date.
    /// See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2LSRequestPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType packetType = OSPFPacketType.LinkStateRequest;

        /// <summary>
        /// Constructs an OSPFv2 LSR packet
        /// </summary>
        public OSPFv2LSRequestPacket()
        {
            this.Type = packetType;
            this.PacketLength = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSR packet with link state requests
        /// </summary>
        /// <param name="lsrs">List of the link state requests</param>
        public OSPFv2LSRequestPacket(List<LinkStateRequest> lsrs)
        {
            int length = lsrs.Count * LinkStateRequest.Length;
            int offset = OSPFv2Fields.HeaderLength;
            byte[] bytes = new byte[length + OSPFv2Fields.HeaderLength];

            Array.Copy((Array) this.header.Bytes, (Array) bytes, (int) this.header.Length);
            for (int i = 0; i < lsrs.Count; i++)
            {
                Array.Copy(lsrs[i].Bytes, 0, bytes, offset, LinkStateRequest.Length);
                offset += LinkStateRequest.Length;
            }

            this.header = new ByteArraySegment(bytes);
            this.Type = packetType;
            this.PacketLength = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSR packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public OSPFv2LSRequestPacket(ByteArraySegment bas)
        {
            this.header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public OSPFv2LSRequestPacket(byte[] Bytes, int Offset) :
            base(Bytes, Offset)
        {
            this.Type = packetType;
        }

        /// <summary>
        /// A list of link state requests, contained in this packet
        /// </summary>
        /// See <see cref="LinkStateRequest"/>
        public virtual List<LinkStateRequest> LinkStateRequests
        {
            get
            {
                int bytesNeeded = this.PacketLength - OSPFv2Fields.LSRStart;
                if (bytesNeeded % LinkStateRequest.Length != 0)
                {
                    throw new Exception("Malformed LSR packet - bad size for the LS requests");
                }

                List<LinkStateRequest> ret = new List<LinkStateRequest>();
                int offset = this.header.Offset + OSPFv2Fields.LSRStart;
                int lsrCount = bytesNeeded / LinkStateRequest.Length;

                for (int i = 0; i < lsrCount; i++)
                {
                    LinkStateRequest request = new LinkStateRequest(this.header.Bytes, offset, LinkStateRequest.Length);
                    ret.Add(request);
                    offset += LinkStateRequest.Length;
                }
                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OSPFv2LSRequestPacket"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OSPFv2LSRequestPacket"/>.</returns>
        public override string ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append((string) base.ToString());
            packet.Append(" ");
            packet.AppendFormat((string) "LSR count: {0} ", (object) this.LinkStateRequests.Count);
            return packet.ToString();
        }

        /// <summary cref="Packet.ToString()">
        /// Output the packet information in the specified format
        ///  Normal - outputs the packet info to a single line
        ///  Colored - outputs the packet info to a single line with coloring
        ///  Verbose - outputs detailed info about the packet
        ///  VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="outputFormat">Output format.</param>
        public override string ToString(StringOutputType outputFormat)
        {
            return this.ToString();
        }
    }
}