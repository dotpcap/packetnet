using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Link State Acknowledgment Packets are OSPF packet type 5.  To make
    /// the flooding of LSAs reliable, flooded LSAs are explicitly
    /// acknowledged. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public sealed class OSPFv2LSAPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType PacketType = OSPFPacketType.LinkStateAcknowledgment;

        /// <summary>
        /// Constructs an OSPFv2 Link State Acknowledge packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2LSAPacket(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs an Link OSPFv2 State Acknowledge packet
        /// </summary>
        public OSPFv2LSAPacket()
        {
            var b = new Byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            Type = PacketType;

            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Link State Acknowledge packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSAPacket(List<LSA.LSA> lsas)
        {
            var length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            var offset = OSPFv2Fields.HeaderLength;
            var bytes = new Byte[length + OSPFv2Fields.HeaderLength];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in lsas)
            {
                Array.Copy(t.Bytes, 0, bytes, offset, OSPFv2Fields.LSAHeaderLength);
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
        public OSPFv2LSAPacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Type = PacketType;
        }

        /// <summary>
        /// List of LSA acknowledgements.
        /// </summary>
        public List<LSA.LSA> LSAAcknowledge
        {
            get
            {
                var ret = new List<LSA.LSA>();
                var bytesNeeded = PacketLength - OSPFv2Fields.LSAAckPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 LSA Packet - Invalid LSA headers count");
                }

                var offset = Header.Offset + OSPFv2Fields.LSAAckPosition;
                var headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (var i = 0; i < headerCount; i++)
                {
                    var l = new LSA.LSA(Header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    ret.Add(l);
                    offset += OSPFv2Fields.LSAHeaderLength;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2LSAPacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2LSAPacket" />.</returns>
        public override String ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.AppendFormat("#LSA{0} ", LSAAcknowledge.Count);
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