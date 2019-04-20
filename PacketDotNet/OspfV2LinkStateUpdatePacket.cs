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
    public sealed class OspfV2LinkStateUpdatePacket : OspfV2Packet
    {
        /// <summary>
        /// Constructs an OSPFv2 Link state update packet
        /// </summary>
        public OspfV2LinkStateUpdatePacket()
        {
            var b = new byte[OspfV2Fields.HeaderLength + 4];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OspfV2Fields.LSRStart);
            Type = OspfPacketType.LinkStateUpdate;

            PacketLength = (ushort) Header.Bytes.Length;
            LsaNumber = 0;
        }

        /// <summary>
        /// Constructs an OSPFv2 link state update with LSAs
        /// </summary>
        /// <param name="linkStates">List of the LSA headers</param>
        public OspfV2LinkStateUpdatePacket(List<LSA.LSA> linkStates)
        {
            var length = 0;
            var offset = OspfV2Fields.HeaderLength + OspfV2Fields.LSANumberLength;

            //calculate the length for the LSAs
            foreach (var t in linkStates)
            {
                length += t.Bytes.Length;
            }

            var bytes = new byte[length + offset];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in linkStates)
            {
                Array.Copy(t.Bytes, 0, bytes, offset, t.Bytes.Length);
                offset += t.Bytes.Length;
            }

            Header = new ByteArraySegment(bytes);
            Type = OspfPacketType.LinkStateUpdate;
            PacketLength = (ushort) Header.Bytes.Length;
            LsaNumber = (uint) linkStates.Count;
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
        public OspfV2LinkStateUpdatePacket(byte[] bytes, int offset) : base(bytes, offset)
        {
            Type = OspfPacketType.LinkStateUpdate;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSU packet from ByteArraySegment
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OspfV2LinkStateUpdatePacket(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment.Bytes);
        }

        /// <summary>
        /// The number of LSAs included in this update.
        /// </summary>
        public uint LsaNumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OspfV2Fields.LSANumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OspfV2Fields.LSANumberPosition);
        }

        /// <summary>
        /// A list of LSA, contained in this packet
        /// </summary>
        /// See
        /// <see cref="LSA" />
        public List<LSA.LSA> Updates
        {
            get
            {
                var ret = new List<LSA.LSA>();

                var offset = Header.Offset + OspfV2Fields.LSAUpdatesPosition;
                for (var i = 0; i < LsaNumber; i++)
                {
                    var l = new LSA.LSA(Header.Bytes, offset, OspfV2Fields.LSAHeaderLength);
                    switch (l.LSType)
                    {
                        case LSAType.ASExternal:
                        {
                            ret.Add(new ASExternalLSA(Header.Bytes, offset, l.Length));
                            break;
                        }
                        case LSAType.Network:
                        {
                            ret.Add(new NetworkLSA(Header.Bytes, offset, l.Length));
                            break;
                        }
                        case LSAType.Router:
                        {
                            ret.Add(new RouterLSA(Header.Bytes, offset, l.Length));
                            break;
                        }
                        case LSAType.Summary:
                        case LSAType.SummaryASBR:
                        {
                            ret.Add(new SummaryLSA(Header.Bytes, offset, l.Length));
                            break;
                        }
                    }

                    offset += l.Length;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="OspfV2LinkStateUpdatePacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="OspfV2LinkStateUpdatePacket" />.</returns>
        public override string ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("LsaNumber: {0} ", LsaNumber);
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