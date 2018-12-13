using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.LSA;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Link State Update packets are OSPF packet type 4.  These packets
    /// implement the flooding of LSAs. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public sealed class OSPFv2LSUpdatePacket : OSPFv2Packet
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
            var b = new Byte[OSPFv2Fields.HeaderLength + 4];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.LSRStart);
            Type = PacketType;

            PacketLength = (UInt16) Header.Bytes.Length;
            LSANumber = 0;
        }

        /// <summary>
        /// Constructs an OSPFv2 link state update with LSAs
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSUpdatePacket(List<LSA.LSA> lsas)
        {
            var length = 0;
            var offset = OSPFv2Fields.HeaderLength + OSPFv2Fields.LSANumberLength;

            //calculate the length for the LSAs
            foreach (var t in lsas)
            {
                length += t.Bytes.Length;
            }

            var bytes = new Byte[length + offset];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in lsas)
            {
                Array.Copy(t.Bytes, 0, bytes, offset, t.Bytes.Length);
                offset += t.Bytes.Length;
            }

            Header = new ByteArraySegment(bytes);
            Type = PacketType;
            PacketLength = (UInt16) Header.Bytes.Length;
            LSANumber = (UInt32) lsas.Count;
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
        public OSPFv2LSUpdatePacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Type = PacketType;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSU packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2LSUpdatePacket(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// The number of LSAs included in this update.
        /// </summary>
        public UInt32 LSANumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.LSANumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.LSANumberPosition);
        }

        /// <summary>
        /// A list of LSA, contained in this packet
        /// </summary>
        /// See
        /// <see cref="PacketDotNet.LSA" />
        public List<LSA.LSA> LSAUpdates
        {
            get
            {
                var ret = new List<LSA.LSA>();

                var offset = Header.Offset + OSPFv2Fields.LSAUpdatesPositon;
                for (var i = 0; i < LSANumber; i++)
                {
                    var l = new LSA.LSA(Header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    switch (l.LSType)
                    {
                        case LSAType.ASExternal:
                            ret.Add(new ASExternalLSA(Header.Bytes, offset, l.Length));
                            break;
                        case LSAType.Network:
                            ret.Add(new NetworkLSA(Header.Bytes, offset, l.Length));
                            break;
                        case LSAType.Router:
                            ret.Add(new RouterLSA(Header.Bytes, offset, l.Length));
                            break;
                        case LSAType.Summary:
                        case LSAType.SummaryASBR:
                            ret.Add(new SummaryLSA(Header.Bytes, offset, l.Length));
                            break;
                    }

                    offset += l.Length;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2LSUpdatePacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2LSUpdatePacket" />.</returns>
        public override String ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("LSANumber: {0} ", LSANumber);
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