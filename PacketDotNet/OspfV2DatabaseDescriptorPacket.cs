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
    public sealed class OspfV2DatabaseDescriptorPacket : OspfV2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OspfPacketType PacketType = OspfPacketType.DatabaseDescription;

        /// <summary>
        /// Constructs an OSPFv2 DD packet from ByteArraySegment
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OspfV2DatabaseDescriptorPacket(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment.Bytes);
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet
        /// </summary>
        public OspfV2DatabaseDescriptorPacket()
        {
            var b = new byte[OspfV2Fields.LSAHeaderPosition];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OspfV2Fields.LSAHeaderPosition);
            Type = PacketType;

            PacketLength = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet with LSA headers
        /// </summary>
        /// <param name="linkStates">List of the LSA headers</param>
        public OspfV2DatabaseDescriptorPacket(List<LSA.LSA> linkStates)
        {
            var length = linkStates.Count * OspfV2Fields.LSAHeaderLength;
            var offset = OspfV2Fields.LSAHeaderPosition;
            var bytes = new byte[length + OspfV2Fields.LSAHeaderPosition];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in linkStates)
            {
                Array.Copy(t.Bytes, 0, bytes, offset, 20); //20 bytes per header
                offset += 20;
            }

            Header = new ByteArraySegment(bytes);
            Type = PacketType;
            PacketLength = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        public OspfV2DatabaseDescriptorPacket(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Type = PacketType;
        }

        /// <summary>
        /// DD Packet bits - See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public byte DescriptionBits
        {
            get => Header.Bytes[Header.Offset + OspfV2Fields.BitsPosition];
            set => Header.Bytes[Header.Offset + OspfV2Fields.BitsPosition] = value;
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public byte DescriptionOptions
        {
            get => Header.Bytes[Header.Offset + OspfV2Fields.DescriptionOptionsPosition];
            set => Header.Bytes[Header.Offset + OspfV2Fields.DescriptionOptionsPosition] = value;
        }

        /// <summary>
        /// Used to sequence the collection of Database Description Packets.
        /// </summary>
        public uint DDSequence
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OspfV2Fields.DDSequencePosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OspfV2Fields.DDSequencePosition);
        }

        /// <summary>
        /// The size in bytes of the largest IP datagram that can be sent
        /// out the associated interface, without fragmentation.
        /// </summary>
        public ushort InterfaceMtu
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OspfV2Fields.InterfaceMTUPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OspfV2Fields.InterfaceMTUPosition);
        }

        /// <summary>
        /// A (possibly partial) list of the link-state database's pieces.
        /// Each LSA in the database is described by its LSA header.
        /// See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        /// See
        /// <see cref="LSA" />
        public List<LSA.LSA> Headers
        {
            get
            {
                var ret = new List<LSA.LSA>();
                var bytesNeeded = PacketLength - OspfV2Fields.LSAHeaderPosition;

                if (bytesNeeded % OspfV2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 DD Packet - Invalid LSA headers count");
                }

                var offset = Header.Offset + OspfV2Fields.LSAHeaderPosition;
                var headerCount = bytesNeeded / OspfV2Fields.LSAHeaderLength;

                for (var i = 0; i < headerCount; i++)
                {
                    var l = new LSA.LSA(Header.Bytes, offset, OspfV2Fields.LSAHeaderLength);
                    offset += OspfV2Fields.LSAHeaderLength;
                    ret.Add(l);
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="OspfV2DatabaseDescriptorPacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="OspfV2DatabaseDescriptorPacket" />.</returns>
        public override string ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("DDSequence: 0x{0:X8} ", DDSequence);
            packet.AppendFormat("#LSA headers: {0} ", Headers.Count);
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
        public override string ToString(StringOutputType outputFormat)
        {
            return ToString();
        }
    }
}