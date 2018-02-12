using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet.OSPF
{
    /// <summary>
    ///     Link State Acknowledgment Packets are OSPF packet type 5.  To make
    ///     the flooding of LSAs reliable, flooded LSAs are explicitly
    ///     acknowledged. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2LSAPacket : OSPFv2Packet
    {
        /// <value>
        ///     The packet type
        /// </value>
        public static OSPFPacketType PacketType = OSPFPacketType.LinkStateAcknowledgment;

        /// <summary>
        ///     Constructs an OSPFv2 Link State Acknowledge packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2LSAPacket(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        ///     Constructs an Link OSPFv2 State Acknowledge packet
        /// </summary>
        public OSPFv2LSAPacket()
        {
            Byte[] b = new Byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(this.HeaderByteArraySegment.Bytes, b, this.HeaderByteArraySegment.Bytes.Length);
            this.HeaderByteArraySegment = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            this.Type = PacketType;

            this.PacketLength = (UInt16) this.HeaderByteArraySegment.Bytes.Length;
        }

        /// <summary>
        ///     Constructs an OSPFv2 Link State Acknowledge packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSAPacket(List<LSA> lsas)
        {
            Int32 length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            Int32 offset = OSPFv2Fields.HeaderLength;
            Byte[] bytes = new Byte[length + OSPFv2Fields.HeaderLength];

            Array.Copy(this.HeaderByteArraySegment.Bytes, bytes, this.HeaderByteArraySegment.Length);
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, OSPFv2Fields.LSAHeaderLength);
                offset += 20;
            }

            this.HeaderByteArraySegment = new ByteArraySegment(bytes);
            this.Type = PacketType;
            this.PacketLength = (UInt16) this.HeaderByteArraySegment.Bytes.Length;
        }

        /// <summary>
        ///     Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="bytes">
        ///     A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        ///     A <see cref="System.Int32" />
        /// </param>
        public OSPFv2LSAPacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            this.Type = PacketType;
        }

        /// <summary>
        ///     List of LSA acknowledgements.
        /// </summary>
        public virtual List<LSA> LSAAcknowledge
        {
            get
            {
                List<LSA> ret = new List<LSA>();
                Int32 bytesNeeded = this.PacketLength - OSPFv2Fields.LSAAckPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 LSA Packet - Invalid LSA headers count");
                }

                Int32 offset = this.HeaderByteArraySegment.Offset + OSPFv2Fields.LSAAckPosition;
                Int32 headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (Int32 i = 0; i < headerCount; i++)
                {
                    LSA l = new LSA(this.HeaderByteArraySegment.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    ret.Add(l);
                    offset += OSPFv2Fields.LSAHeaderLength;
                }

                return ret;
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents the current <see cref="OSPFv2LSAPacket" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="OSPFv2LSAPacket" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.AppendFormat("#LSA{0} ", this.LSAAcknowledge.Count);
            return packet.ToString();
        }

        /// <summary cref="Packet.ToString()">
        ///     Output the packet information in the specified format
        ///     Normal - outputs the packet info to a single line
        ///     Colored - outputs the packet info to a single line with coloring
        ///     Verbose - outputs detailed info about the packet
        ///     VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="outputFormat">Output format.</param>
        public override String ToString(StringOutputType outputFormat)
        {
            return this.ToString();
        }
    }
}