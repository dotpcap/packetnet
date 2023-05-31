using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Lsa;
using PacketDotNet.Utils;

namespace PacketDotNet;

    /// <summary>
    /// Link State Acknowledgment Packets are OSPF packet type 5.  To make
    /// the flooding of LSAs reliable, flooded LSAs are explicitly
    /// acknowledged. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public sealed class OspfV2LinkStateAcknowledgmentPacket : OspfV2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OspfPacketType PacketType = OspfPacketType.LinkStateAcknowledgment;

        /// <summary>
        /// Constructs an OSPFv2 Link State Acknowledge packet from ByteArraySegment
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OspfV2LinkStateAcknowledgmentPacket(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment.Bytes);
        }

        /// <summary>
        /// Constructs an Link OSPFv2 State Acknowledge packet
        /// </summary>
        public OspfV2LinkStateAcknowledgmentPacket()
        {
            var b = new byte[OspfV2Fields.LSAHeaderPosition];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OspfV2Fields.LSAHeaderPosition);
            Type = PacketType;

            PacketLength = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Link State Acknowledge packet with LSA headers
        /// </summary>
        /// <param name="linkStates">List of the LSA headers</param>
        public OspfV2LinkStateAcknowledgmentPacket(List<LinkStateAdvertisement> linkStates)
        {
            var length = linkStates.Count * OspfV2Fields.LSAHeaderLength;
            var offset = OspfV2Fields.HeaderLength;
            var bytes = new byte[length + OspfV2Fields.HeaderLength];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in linkStates)
            {
                Array.Copy(t.Bytes, 0, bytes, offset, OspfV2Fields.LSAHeaderLength);
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
        public OspfV2LinkStateAcknowledgmentPacket(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Type = PacketType;
        }

        /// <summary>
        /// List of LSA acknowledgments.
        /// </summary>
        public List<LinkStateAdvertisement> Acknowledgments
        {
            get
            {
                var ret = new List<LinkStateAdvertisement>();
                var bytesNeeded = PacketLength - OspfV2Fields.LSAAckPosition;

                if (bytesNeeded % OspfV2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 LSA Packet - Invalid LSA headers count");
                }

                var offset = Header.Offset + OspfV2Fields.LSAAckPosition;
                var headerCount = bytesNeeded / OspfV2Fields.LSAHeaderLength;

                for (var i = 0; i < headerCount; i++)
                {
                    var l = new LinkStateAdvertisement(Header.Bytes, offset, OspfV2Fields.LSAHeaderLength);
                    ret.Add(l);
                    offset += OspfV2Fields.LSAHeaderLength;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="OspfV2LinkStateAcknowledgmentPacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="OspfV2LinkStateAcknowledgmentPacket" />.</returns>
        public override string ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.AppendFormat("#LSA{0} ", Acknowledgments.Count);
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