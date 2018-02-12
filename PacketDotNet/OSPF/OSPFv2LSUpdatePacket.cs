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
        public static OSPFPacketType PacketType = OSPFPacketType.LinkStateUpdate;

        /// <summary>
        /// Constructs an OSPFv2 Link state update packet
        /// </summary>
        public OSPFv2LSUpdatePacket()
        {
            Byte[] b = new Byte[OSPFv2Fields.HeaderLength + 4];
            Array.Copy(this.HeaderByteArraySegment.Bytes, b, this.HeaderByteArraySegment.Bytes.Length);
            this.HeaderByteArraySegment = new ByteArraySegment(b, 0, OSPFv2Fields.LSRStart);
            this.Type = PacketType;

            this.PacketLength = (UInt16)this.HeaderByteArraySegment.Bytes.Length;
            this.LSANumber = 0;
        }

        /// <summary>
        /// Constructs an OSPFv2 link state update with LSAs
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSUpdatePacket(List<LSA> lsas)
        {
            Int32 length = 0;
            Int32 offset = OSPFv2Fields.HeaderLength + OSPFv2Fields.LSANumberLength;

            //calculate the length for the LSAs
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                length += lsas[i].Bytes.Length;
            }

            Byte[] bytes = new Byte[length + offset];

            Array.Copy(this.HeaderByteArraySegment.Bytes, bytes, this.HeaderByteArraySegment.Length);
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, lsas[i].Bytes.Length);
                offset += lsas[i].Bytes.Length;
            }

            this.HeaderByteArraySegment = new ByteArraySegment(bytes);
            this.Type = PacketType;
            this.PacketLength = (UInt16)this.HeaderByteArraySegment.Bytes.Length;
            this.LSANumber = (UInt32)lsas.Count;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public OSPFv2LSUpdatePacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            this.Type = PacketType;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSU packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public OSPFv2LSUpdatePacket(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// The number of LSAs included in this update.
        /// </summary>
        public virtual UInt32 LSANumber
        {
            get => EndianBitConverter.Big.ToUInt32(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + OSPFv2Fields.LSANumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + OSPFv2Fields.LSANumberPosition);
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

                Int32 offset = this.HeaderByteArraySegment.Offset + OSPFv2Fields.LSAUpdatesPositon;
                for (Int32 i = 0; i < this.LSANumber; i++)
                {
                    LSA l = new LSA(this.HeaderByteArraySegment.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    switch (l.LSType)
                    {
                        case LSAType.ASExternal:
                            ret.Add(new ASExternalLSA(this.HeaderByteArraySegment.Bytes, offset, l.Length));
                            break;
                        case LSAType.Network:
                            ret.Add(new NetworkLSA(this.HeaderByteArraySegment.Bytes, offset, l.Length));
                            break;
                        case LSAType.Router:
                            ret.Add(new RouterLSA(this.HeaderByteArraySegment.Bytes, offset, l.Length));
                            break;
                        case LSAType.Summary:
                        case LSAType.SummaryASBR:
                            ret.Add(new SummaryLSA(this.HeaderByteArraySegment.Bytes, offset, l.Length));
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
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat($"LSANumber: {this.LSANumber} ");
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
        public override String ToString(StringOutputType outputFormat)
        {
            return this.ToString();
        }
    }
}