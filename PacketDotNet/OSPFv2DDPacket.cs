using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
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
        public static OSPFPacketType packetType = OSPFPacketType.DatabaseDescription;

        /// <summary>
        /// Constructs an OSPFv2 DD packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public OSPFv2DDPacket(ByteArraySegment bas)
        {
            this.header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet
        /// </summary>
        public OSPFv2DDPacket()
        {
            byte[] b = new byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(this.header.Bytes, b, this.header.Bytes.Length);
            this.header = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            this.Type = packetType;

            this.PacketLength = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2DDPacket(List<LSA> lsas)
        {
            int length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            int offset = OSPFv2Fields.LSAHeaderPosition;
            byte[] bytes = new byte[length + OSPFv2Fields.LSAHeaderPosition];

            Array.Copy(this.header.Bytes, bytes, this.header.Length);
            for (int i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, 20); //20 bytes per header
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
        public OSPFv2DDPacket(byte[] Bytes, int Offset) :
            base(Bytes, Offset)
        {
            this.Type = packetType;
        }


        /// <summary>
        /// The size in bytes of the largest IP datagram that can be sent
        /// out the associated interface, without fragmentation.
        /// </summary>
        public virtual ushort InterfaceMTU
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(this.header.Bytes, this.header.Offset + OSPFv2Fields.InterfaceMTUPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + OSPFv2Fields.InterfaceMTUPosition);
            }
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual byte DBDescriptionOptions
        {
            get
            {
                return this.header.Bytes[this.header.Offset + OSPFv2Fields.DBDescriptionOptionsPosition];
            }
            set
            {
                this.header.Bytes[this.header.Offset + OSPFv2Fields.DBDescriptionOptionsPosition] = value;

            }
        }

        /// <summary>
        /// DD Packet bits - See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual byte DBDescriptionBits
        {
            get
            {
                return this.header.Bytes[this.header.Offset + OSPFv2Fields.BitsPosition];
            }
            set
            {
                this.header.Bytes[this.header.Offset + OSPFv2Fields.BitsPosition] = value;
            }
        }

        /// <summary>
        /// Used to sequence the collection of Database Description Packets.
        /// </summary>
        public virtual uint DDSequence
        {
            get
            {
                return EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + OSPFv2Fields.DDSequencePosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + OSPFv2Fields.DDSequencePosition);
            }
        }

        ///<summary>
        /// A (possibly partial) list of the link-state database's pieces.
        /// Each LSA in the database is described by its LSA header.
        /// See http://www.ietf.org/rfc/rfc2328.txt for details.
        ///</summary>
        /// See <see cref="PacketDotNet.LSA"/>
        public virtual List<LSA> LSAHeader
        {
            get
            {
                List<LSA> ret = new List<LSA>();
                int bytesNeeded = this.PacketLength - OSPFv2Fields.LSAHeaderPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 DD Packet - Invalid LSA headers count");
                }

                int offset = this.header.Offset + OSPFv2Fields.LSAHeaderPosition;
                int headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (int i = 0; i < headerCount; i++)
                {
                    LSA l = new LSA(this.header.Bytes, offset , OSPFv2Fields.LSAHeaderLength);
                    offset += OSPFv2Fields.LSAHeaderLength;
                    ret.Add(l);
                }
                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="PacketDotNet.OSPFv2DDPacket"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="PacketDotNet.OSPFv2DDPacket"/>.</returns>
        public override string ToString()
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
        public override string ToString(StringOutputType outputFormat)
        {
            return this.ToString();
        }
    }
}