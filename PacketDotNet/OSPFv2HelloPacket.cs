using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Hello packets are OSPF packet type 1.  These packets are sent
    /// periodically on all interfaces (including virtual links) in order to
    /// establish and maintain neighbor relationships.
    /// See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public sealed class OSPFv2HelloPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType PacketType = OSPFPacketType.Hello;

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2HelloPacket(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from NetworkMask, HelloInterval and
        /// RouterDeadInterval.
        /// </summary>
        /// <param name="networkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="helloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="routerDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket
        (
            IPAddress networkMask,
            UInt16 helloInterval,
            UInt16 routerDeadInterval)
        {
            var b = new Byte[OSPFv2Fields.NeighborIDStart];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.NeighborIDStart);
            Type = PacketType;

            NetworkMask = networkMask;
            HelloInterval = helloInterval;
            RouterDeadInterval = routerDeadInterval;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from NetworkMask, HelloInterval and
        /// RouterDeadInterval and a list of neighbor routers.
        /// </summary>
        /// <param name="networkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="helloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="routerDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="neighbors">List of router neighbors - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket
        (
            IPAddress networkMask,
            UInt16 helloInterval,
            UInt16 routerDeadInterval,
            IReadOnlyList<IPAddress> neighbors)
            : this(networkMask, helloInterval, routerDeadInterval)
        {
            var length = neighbors.Count * 4;
            var offset = OSPFv2Fields.NeighborIDStart;
            var bytes = new Byte[length + OSPFv2Fields.NeighborIDStart];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            foreach (var t in neighbors)
            {
                Array.Copy(t.GetAddressBytes(), 0, bytes, offset, 4); //4 bytes per address
                offset += 4;
            }

            Header = new ByteArraySegment(bytes);
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from given bytes and offset.
        /// </summary>
        /// <param name="bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        public OSPFv2HelloPacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            Type = PacketType;
        }


        /// <summary>
        /// The identity of the Backup Designated Router for this network,
        /// in the view of the sending router.
        /// </summary>
        public IPAddress BackupRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.BackupRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OSPFv2Fields.BackupRouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The identity of the Designated Router for this network, in the
        /// view of the sending router.
        /// </summary>
        public IPAddress DesignatedRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.DesignatedRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OSPFv2Fields.DesignatedRouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The number of seconds between this router's Hello packets.
        /// </summary>
        public UInt16 HelloInterval
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.HelloIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.HelloIntervalPosition);
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public Byte HelloOptions
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.HelloOptionsPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.HelloOptionsPosition] = value;
        }

        /// <summary>
        /// List of the Router IDs of each router from whom valid Hello packets have
        /// been seen recently on the network.  Recently means in the last
        /// RouterDeadInterval seconds. Can be zero or more.
        /// </summary>
        public List<IPAddress> NeighborID
        {
            get
            {
                var ret = new List<IPAddress>();
                var bytesAvailable = PacketLength - OSPFv2Fields.NeighborIDStart;

                if (bytesAvailable % 4 != 0)
                {
                    throw new Exception("malformed OSPFv2Hello Packet - bad NeighborID size");
                }

                var offset = OSPFv2Fields.NeighborIDStart;
                while (offset < PacketLength)
                {
                    Int64 address = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + offset);
                    ret.Add(new IPAddress(address));
                    offset += 4;
                }

                return ret;
            }
        }

        /// <summary>
        /// The network mask associated with this interface.
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.NetworkMaskPositon);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OSPFv2Fields.NetworkMaskPositon,
                           address.Length);
            }
        }


        /// <summary>
        /// The number of seconds before declaring a silent router down.
        /// </summary>
        public UInt32 RouterDeadInterval
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
        }

        /// <summary>
        /// This router's Router Priority.
        /// </summary>
        public Byte RtrPriority
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.RtrPriorityPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.RtrPriorityPosition] = value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2HelloPacket" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="PacketDotNet.OSPFv2HelloPacket" />.</returns>
        public override String ToString()
        {
            var packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("HelloOptions: {0} ", HelloOptions);
            packet.AppendFormat("RouterID: {0} ", RouterID);
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