using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Link State Acknowledgment Packets are OSPF packet type 5.  To make
    /// the flooding of LSAs reliable, flooded LSAs are explicitly
    /// acknowledged. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2LSAPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType packetType = OSPFPacketType.LinkStateAcknowledgment;

        /// <summary>
        /// Constructs an OSPFv2 Link State Acknowledge packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public OSPFv2LSAPacket(ByteArraySegment bas)
        {
            this.header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs an Link OSPFv2 State Acknowledge packet
        /// </summary>
        public OSPFv2LSAPacket()
        {
            byte[] b = new byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(this.header.Bytes, b, this.header.Bytes.Length);
            this.header = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            this.Type = packetType;

            this.PacketLength = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Link State Acknowledge packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSAPacket(List<LSA> lsas)
        {
            int length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            int offset = OSPFv2Fields.HeaderLength;
            byte[] bytes = new byte[length + OSPFv2Fields.HeaderLength];

            Array.Copy(this.header.Bytes, bytes, this.header.Length);
            for (int i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, OSPFv2Fields.LSAHeaderLength); 
                offset += 20;
            }

            this.header = new ByteArraySegment(bytes);
            this.Type = packetType;
            this.PacketLength = (ushort)this.header.Bytes.Length;
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
        public OSPFv2LSAPacket(byte[] Bytes, int Offset) :
            base(Bytes, Offset)
        {
            this.Type = packetType;
        }

        /// <summary>
        /// List of LSA acknowledgements.
        /// </summary>
        public virtual List<LSA> LSAAcknowledge
        {
            get
            {
                List<LSA> ret = new List<LSA>();
                int bytesNeeded = this.PacketLength - OSPFv2Fields.LSAAckPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 LSA Packet - Invalid LSA headers count");
                }

                int offset = this.header.Offset + OSPFv2Fields.LSAAckPosition;
                int headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (int i = 0; i < headerCount; i++)
                {
                    LSA l = new LSA(this.header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    ret.Add(l);
                    offset += OSPFv2Fields.LSAHeaderLength;
                }
                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="PacketDotNet.OSPFv2LSAPacket"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="PacketDotNet.OSPFv2LSAPacket"/>.</returns>
        public override string ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.AppendFormat("#LSA{0} ", this.LSAAcknowledge.Count);
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