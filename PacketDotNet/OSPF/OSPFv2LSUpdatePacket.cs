using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Link State Update packets are OSPF packet type 4.  These packets
    /// implement the flooding of LSAs. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2LSUpdatePacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType packetType = OSPFPacketType.LinkStateUpdate;

        /// <summary>
        /// Constructs an OSPFv2 Link state update packet
        /// </summary>
        public OSPFv2LSUpdatePacket()
        {
            byte[] b = new byte[OSPFv2Fields.HeaderLength + 4];
            Array.Copy((Array) this.header.Bytes, (Array) b, (int) this.header.Bytes.Length);
            this.header = new ByteArraySegment(b, 0, OSPFv2Fields.LSRStart);
            this.Type = packetType;

            this.PacketLength = (ushort)this.header.Bytes.Length;
            this.LSANumber = 0;
        }

        /// <summary>
        /// Constructs an OSPFv2 link state update with LSAs
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSUpdatePacket(List<LSA> lsas)
        {
            int length = 0;
            int offset = OSPFv2Fields.HeaderLength + OSPFv2Fields.LSANumberLength;

            //calculate the length for the LSAs
            for (int i = 0; i < lsas.Count; i++)
            {
                length += lsas[i].Bytes.Length;
            }

            byte[] bytes = new byte[length + offset];

            Array.Copy((Array) this.header.Bytes, (Array) bytes, (int) this.header.Length);
            for (int i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, lsas[i].Bytes.Length);
                offset += lsas[i].Bytes.Length;
            }

            this.header = new ByteArraySegment(bytes);
            this.Type = packetType;
            this.PacketLength = (ushort)this.header.Bytes.Length;
            this.LSANumber = (uint)lsas.Count;
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
        public OSPFv2LSUpdatePacket(byte[] Bytes, int Offset) :
            base(Bytes, Offset)
        {
            this.Type = packetType;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSU packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public OSPFv2LSUpdatePacket(ByteArraySegment bas)
        {
            this.header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// The number of LSAs included in this update.
        /// </summary>
        public virtual uint LSANumber
        {
            get
            {
                return EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + OSPFv2Fields.LSANumberPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + OSPFv2Fields.LSANumberPosition);
            }
        }

        /// <summary>
        /// A list of LSA, contained in this packet
        /// </summary>
        /// See <see cref="LSA"/>
        public virtual List<LSA> LSAUpdates
        {
            get
            {
                List<LSA> ret = new List<LSA>();

                int offset = this.header.Offset + OSPFv2Fields.LSAUpdatesPositon;
                for (int i = 0; i < this.LSANumber; i++)
                {
                    LSA l = new LSA(this.header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    switch (l.LSType)
                    {
                        case LSAType.ASExternal:
                            ret.Add(new ASExternalLSA(this.header.Bytes, offset, l.Length));
                            break;
                        case LSAType.Network:
                            ret.Add(new NetworkLSA(this.header.Bytes, offset, l.Length));
                            break;
                        case LSAType.Router:
                            ret.Add(new RouterLSA(this.header.Bytes, offset, l.Length));
                            break;
                        case LSAType.Summary:
                        case LSAType.SummaryASBR:
                            ret.Add(new SummaryLSA(this.header.Bytes, offset, l.Length));
                            break;
                    }
                    offset += l.Length;
                }
                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OSPFv2LSUpdatePacket"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OSPFv2LSUpdatePacket"/>.</returns>
        public override string ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append((string) base.ToString());
            packet.Append(" ");
            packet.AppendFormat((string) "LSANumber: {0} ", (object) this.LSANumber);
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