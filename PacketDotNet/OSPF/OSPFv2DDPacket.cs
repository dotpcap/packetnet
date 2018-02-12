using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Database Description packets are OSPF packet type 2.  These packets
    /// are exchanged when an adjacency is being initialized.
    /// See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2DDPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType PacketType = OSPFPacketType.DatabaseDescription;

        /// <summary>
        /// Constructs an OSPFv2 DD packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public OSPFv2DDPacket(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet
        /// </summary>
        public OSPFv2DDPacket()
        {
            Byte[] b = new Byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(this.HeaderByteArraySegment.Bytes, b, this.HeaderByteArraySegment.Bytes.Length);
            this.HeaderByteArraySegment = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            this.Type = PacketType;

            this.PacketLength = (UInt16)this.HeaderByteArraySegment.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2DDPacket(List<LSA> lsas)
        {
            Int32 length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            Int32 offset = OSPFv2Fields.LSAHeaderPosition;
            Byte[] bytes = new Byte[length + OSPFv2Fields.LSAHeaderPosition];

            Array.Copy(this.HeaderByteArraySegment.Bytes, bytes, this.HeaderByteArraySegment.Length);
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, 20); //20 bytes per header
                offset += 20;
            }

            this.HeaderByteArraySegment = new ByteArraySegment(bytes);
            this.Type = PacketType;
            this.PacketLength = (UInt16)this.HeaderByteArraySegment.Bytes.Length;
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
        public OSPFv2DDPacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            this.Type = PacketType;
        }


        /// <summary>
        /// The size in bytes of the largest IP datagram that can be sent
        /// out the associated interface, without fragmentation.
        /// </summary>
        public virtual UInt16 InterfaceMTU
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + OSPFv2Fields.InterfaceMTUPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + OSPFv2Fields.InterfaceMTUPosition);
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual Byte DBDescriptionOptions
        {
            get => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + OSPFv2Fields.DBDescriptionOptionsPosition];
            set => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + OSPFv2Fields.DBDescriptionOptionsPosition] = value;
        }

        /// <summary>
        /// DD Packet bits - See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual Byte DBDescriptionBits
        {
            get => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + OSPFv2Fields.BitsPosition];
            set => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + OSPFv2Fields.BitsPosition] = value;
        }

        /// <summary>
        /// Used to sequence the collection of Database Description Packets.
        /// </summary>
        public virtual UInt32 DDSequence
        {
            get => EndianBitConverter.Big.ToUInt32(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + OSPFv2Fields.DDSequencePosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + OSPFv2Fields.DDSequencePosition);
        }

        ///<summary>
        /// A (possibly partial) list of the link-state database's pieces.
        /// Each LSA in the database is described by its LSA header.
        /// See http://www.ietf.org/rfc/rfc2328.txt for details.
        ///</summary>
        /// See <see cref="LSA"/>
        public virtual List<LSA> LSAHeader
        {
            get
            {
                List<LSA> ret = new List<LSA>();
                Int32 bytesNeeded = this.PacketLength - OSPFv2Fields.LSAHeaderPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 DD Packet - Invalid LSA headers count");
                }

                Int32 offset = this.HeaderByteArraySegment.Offset + OSPFv2Fields.LSAHeaderPosition;
                Int32 headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (Int32 i = 0; i < headerCount; i++)
                {
                    LSA l = new LSA(this.HeaderByteArraySegment.Bytes, offset , OSPFv2Fields.LSAHeaderLength);
                    offset += OSPFv2Fields.LSAHeaderLength;
                    ret.Add(l);
                }
                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OSPFv2DDPacket"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OSPFv2DDPacket"/>.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("DDSequence: 0x{0:X8} ", this.DDSequence);
            packet.AppendFormat("#LSA headers: {0} ", this.LSAHeader.Count);
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