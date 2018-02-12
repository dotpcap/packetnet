using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    ///     Hello packets are OSPF packet type 1.  These packets are sent
    ///     periodically on all interfaces (including virtual links) in order to
    ///     establish and maintain neighbor relationships.
    ///     See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2HelloPacket : OSPFv2Packet
    {
        /// <value>
        ///     The packet type
        /// </value>
        public static OSPFPacketType PacketType = OSPFPacketType.Hello;

        /// <summary>
        ///     Constructs an OSPFv2 Hello packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2HelloPacket(ByteArraySegment bas)
        {
            this.HeaderByteArraySegment = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        ///     Constructs an OSPFv2 Hello packet from NetworkMask, HelloInterval and
        ///     RouterDeadInterval.
        /// </summary>
        /// <param name="networkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="helloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="routerDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket(IPAddress networkMask, UInt16 helloInterval, UInt16 routerDeadInterval)
        {
            Byte[] b = new Byte[OSPFv2Fields.NeighborIDStart];
            Array.Copy(this.HeaderByteArraySegment.Bytes, b, this.HeaderByteArraySegment.Bytes.Length);
            this.HeaderByteArraySegment = new ByteArraySegment(b, 0, OSPFv2Fields.NeighborIDStart);
            this.Type = PacketType;

            this.NetworkMask = networkMask;
            this.HelloInterval = helloInterval;
            this.RouterDeadInterval = routerDeadInterval;
            this.PacketLength = (UInt16) this.HeaderByteArraySegment.Bytes.Length;
        }

        /// <summary>
        ///     Constructs an OSPFv2 Hello packet from NetworkMask, HelloInterval and
        ///     RouterDeadInterval and a list of neighbor routers.
        /// </summary>
        /// <param name="networkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="helloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="routerDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="neighbors">List of router neighbors - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket(IPAddress networkMask, UInt16 helloInterval,
            UInt16 routerDeadInterval, List<IPAddress> neighbors)
            : this(networkMask, helloInterval, routerDeadInterval)
        {
            Int32 length = neighbors.Count * 4;
            Int32 offset = OSPFv2Fields.NeighborIDStart;
            Byte[] bytes = new Byte[length + OSPFv2Fields.NeighborIDStart];

            Array.Copy(this.HeaderByteArraySegment.Bytes, bytes, this.HeaderByteArraySegment.Length);
            for (Int32 i = 0; i < neighbors.Count; i++)
            {
                Array.Copy(neighbors[i].GetAddressBytes(), 0, bytes, offset, 4); //4 bytes per address
                offset += 4;
            }

            this.HeaderByteArraySegment = new ByteArraySegment(bytes);
            this.PacketLength = (UInt16) this.HeaderByteArraySegment.Bytes.Length;
        }

        /// <summary>
        ///     Constructs an OSPFv2 Hello packet from given bytes and offset.
        /// </summary>
        /// <param name="bytes">
        ///     A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        ///     A <see cref="System.Int32" />
        /// </param>
        public OSPFv2HelloPacket(Byte[] bytes, Int32 offset) :
            base(bytes, offset)
        {
            this.Type = PacketType;
        }


        /// <summary>
        ///     The identity of the Backup Designated Router for this network,
        ///     in the view of the sending router.
        /// </summary>
        public virtual IPAddress BackupRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.BackupRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.BackupRouterIDPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     The identity of the Designated Router for this network, in the
        ///     view of the sending router.
        /// </summary>
        public virtual IPAddress DesignatedRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.DesignatedRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.DesignatedRouterIDPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///     The number of seconds between this router's Hello packets.
        /// </summary>
        public virtual UInt16 HelloInterval
        {
            get => EndianBitConverter.Big.ToUInt16(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.HelloIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.HelloIntervalPosition);
        }

        /// <summary>
        ///     The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual Byte HelloOptions
        {
            get => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.HelloOptionsPosition];
            set => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.HelloOptionsPosition] = value;
        }

        /// <summary>
        ///     List of the Router IDs of each router from whom valid Hello packets have
        ///     been seen recently on the network.  Recently means in the last
        ///     RouterDeadInterval seconds. Can be zero or more.
        /// </summary>
        public virtual List<IPAddress> NeighborID
        {
            get
            {
                List<IPAddress> ret = new List<IPAddress>();
                Int32 bytesAvailable = this.PacketLength - OSPFv2Fields.NeighborIDStart;

                if (bytesAvailable % 4 != 0)
                {
                    throw new Exception("malformed OSPFv2Hello Packet - bad NeighborID size");
                }

                Int32 offset = OSPFv2Fields.NeighborIDStart;
                while (offset < this.PacketLength)
                {
                    Int64 address = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset + offset);
                    ret.Add(new IPAddress(address));
                    offset += 4;
                }

                return ret;
            }
        }

        /// <summary>
        ///     The network mask associated with this interface.
        /// </summary>
        public virtual IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.NetworkMaskPositon);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.HeaderByteArraySegment.Bytes,
                    this.HeaderByteArraySegment.Offset + OSPFv2Fields.NetworkMaskPositon,
                    address.Length);
            }
        }


        /// <summary>
        ///     The number of seconds before declaring a silent router down.
        /// </summary>
        public virtual UInt32 RouterDeadInterval
        {
            get => EndianBitConverter.Big.ToUInt32(this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
        }

        /// <summary>
        ///     This router's Router Priority.
        /// </summary>
        public virtual Byte RtrPriority
        {
            get => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.RtrPriorityPosition];
            set => this.HeaderByteArraySegment.Bytes[
                this.HeaderByteArraySegment.Offset + OSPFv2Fields.RtrPriorityPosition] = value;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents the current <see cref="OSPFv2HelloPacket" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="OSPFv2HelloPacket" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("HelloOptions: {0} ", this.HelloOptions);
            packet.AppendFormat("RouterID: {0} ", this.RouterID);
            return packet.ToString();
        }

        /// <summary cref="Packet.ToString()">
        ///     Output the packet information in the specified format
        ///     Normal - outputs the packet info to a single line
        ///     Colored - outputs the packet info to a single line with coloring
        ///     Verbose - outputs detailed info about the packet
        ///     VerboseColored - outputs detailed info about the packet with coloring
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="outputFormat">Output format.</param>
        public override String ToString(StringOutputType outputFormat)
        {
            return this.ToString();
        }
    }
}