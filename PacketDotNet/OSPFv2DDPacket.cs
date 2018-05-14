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
    public sealed class OSPFv2DDPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType PacketType = OSPFPacketType.DatabaseDescription;

        /// <summary>
        /// Constructs an OSPFv2 DD packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2DDPacket(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet
        /// </summary>
        public OSPFv2DDPacket()
        {
            var b = new Byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            Type = PacketType;

            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2DDPacket(List<LSA.LSA> lsas)
        {
            var length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            var offset = OSPFv2Fields.LSAHeaderPosition;
            var bytes = new Byte[length + OSPFv2Fields.LSAHeaderPosition];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in lsas)
            {
                Array.Copy(t.Bytes, 0, bytes, offset, 20); //20 bytes per header
                offset += 20;
            }

            Header = new ByteArraySegment(bytes);
            Type = PacketType;
            PacketLength = (UInt16) Header.Bytes.Length;
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
        public OSPFv2DDPacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Type = PacketType;
        }

        /// <summary>
        /// DD Packet bits - See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public Byte DBDescriptionBits
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.BitsPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.BitsPosition] = value;
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public Byte DBDescriptionOptions
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.DBDescriptionOptionsPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.DBDescriptionOptionsPosition] = value;
        }

        /// <summary>
        /// Used to sequence the collection of Database Description Packets.
        /// </summary>
        public UInt32 DDSequence
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.DDSequencePosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.DDSequencePosition);
        }


        /// <summary>
        /// The size in bytes of the largest IP datagram that can be sent
        /// out the associated interface, without fragmentation.
        /// </summary>
        public UInt16 InterfaceMTU
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.InterfaceMTUPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.InterfaceMTUPosition);
        }

        /// <summary>
        /// A (possibly partial) list of the link-state database's pieces.
        /// Each LSA in the database is described by its LSA header.
        /// See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        /// See
        /// <see cref="PacketDotNet.LSA" />
        public List<LSA.LSA> LSAHeader
        {
            get
            {
                var ret = new List<LSA.LSA>();
                var bytesNeeded = PacketLength - OSPFv2Fields.LSAHeaderPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 DD Packet - Invalid LSA headers count");
                }

                var offset = Header.Offset + OSPFv2Fields.LSAHeaderPosition;
                var headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (var i = 0; i < headerCount; i++)
                {
                    var l = new LSA.LSA(Header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    offset += OSPFv2Fields.LSAHeaderLength;
                    ret.Add(l);
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2DDPacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2DDPacket" />.</returns>
        public override String ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("DDSequence: 0x{0:X8} ", DDSequence);
            packet.AppendFormat("#LSA headers: {0} ", LSAHeader.Count);
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