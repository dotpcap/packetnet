using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Hello packets are OSPF packet type 1.  These packets are sent
    /// periodically on all interfaces (including virtual links) in order to
    /// establish and maintain neighbor relationships.
    /// See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2HelloPacket : OSPFv2Packet
    {

        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType packetType = OSPFPacketType.Hello;

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public OSPFv2HelloPacket(ByteArraySegment bas)
        {
            this.header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from NetworkMask, HelloInterval and
        /// RouterDeadInterval.
        /// </summary>
        /// <param name="NetworkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="HelloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="RouterDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket(System.Net.IPAddress NetworkMask, ushort HelloInterval,
            ushort RouterDeadInterval)
        {
            byte[] b = new byte[OSPFv2Fields.NeighborIDStart];
            Array.Copy((Array) this.header.Bytes, (Array) b, (int) this.header.Bytes.Length);
            this.header = new ByteArraySegment(b, 0, OSPFv2Fields.NeighborIDStart);
            this.Type = packetType;

            this.NetworkMask = NetworkMask;
            this.HelloInterval = HelloInterval;
            this.RouterDeadInterval = RouterDeadInterval;
            this.PacketLength = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from NetworkMask, HelloInterval and
        /// RouterDeadInterval and a list of neighbor routers.
        /// </summary>
        /// <param name="NetworkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="HelloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="RouterDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="Neighbors">List of router neighbors - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket(System.Net.IPAddress NetworkMask, ushort HelloInterval,
            ushort RouterDeadInterval, List<System.Net.IPAddress> Neighbors)
            : this(NetworkMask, HelloInterval, RouterDeadInterval)
        {
            int length = Neighbors.Count * 4;
            int offset = OSPFv2Fields.NeighborIDStart;
            byte[] bytes = new byte[length + OSPFv2Fields.NeighborIDStart];

            Array.Copy((Array) this.header.Bytes, (Array) bytes, (int) this.header.Length);
            for (int i = 0; i < Neighbors.Count; i++)
            {
                Array.Copy(Neighbors[i].GetAddressBytes(), 0, bytes, offset, 4); //4 bytes per address
                offset += 4;
            }

            this.header = new ByteArraySegment(bytes);
            this.PacketLength = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from given bytes and offset.
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public OSPFv2HelloPacket(byte[] Bytes, int Offset) :
            base(Bytes, Offset)
        {
            this.Type = packetType;
        }

        /// <summary>
        /// The network mask associated with this interface.
        /// </summary>
        public virtual System.Net.IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + OSPFv2Fields.NetworkMaskPositon);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + OSPFv2Fields.NetworkMaskPositon),
                    address.Length);
            }
        }

        /// <summary>
        /// The number of seconds between this router's Hello packets.
        /// </summary>
        public virtual ushort HelloInterval
        {
            get => EndianBitConverter.Big.ToUInt16(this.header.Bytes, this.header.Offset + OSPFv2Fields.HelloIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + OSPFv2Fields.HelloIntervalPosition);
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual byte HelloOptions
        {
            get => this.header.Bytes[this.header.Offset + OSPFv2Fields.HelloOptionsPosition];
            set => this.header.Bytes[this.header.Offset + OSPFv2Fields.HelloOptionsPosition] = value;
        }

        /// <summary>
        /// This router's Router Priority.
        /// </summary>
        public virtual byte RtrPriority
        {
            get => this.header.Bytes[this.header.Offset + OSPFv2Fields.RtrPriorityPosition];
            set => this.header.Bytes[this.header.Offset + OSPFv2Fields.RtrPriorityPosition] = value;
        }


        /// <summary>
        /// The number of seconds before declaring a silent router down.
        /// </summary>
        public virtual uint RouterDeadInterval
        {
            get => EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
        }

        /// <summary>
        /// The identity of the Designated Router for this network, in the
        /// view of the sending router.
        /// </summary>
        public virtual System.Net.IPAddress DesignatedRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + OSPFv2Fields.DesignatedRouterIDPosition);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + OSPFv2Fields.DesignatedRouterIDPosition),
                    address.Length);
            }
        }


        /// <summary>
        /// The identity of the Backup Designated Router for this network,
        /// in the view of the sending router.
        /// </summary>
        public virtual System.Net.IPAddress BackupRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + OSPFv2Fields.BackupRouterIDPosition);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + OSPFv2Fields.BackupRouterIDPosition),
                    address.Length);
            }
        }

        /// <summary>
        /// List of the Router IDs of each router from whom valid Hello packets have
        /// been seen recently on the network.  Recently means in the last
        /// RouterDeadInterval seconds. Can be zero or more.
        /// </summary>
        public virtual List<System.Net.IPAddress> NeighborID
        {
            get
            {
                List<System.Net.IPAddress> ret = new List<System.Net.IPAddress>();
                int bytesAvailable = this.PacketLength - OSPFv2Fields.NeighborIDStart;

                if (bytesAvailable % 4 != 0)
                {
                    throw new Exception("malformed OSPFv2Hello Packet - bad NeighborID size");
                }

                int offset = OSPFv2Fields.NeighborIDStart;
                while (offset < this.PacketLength)
                {
                    long address = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + offset);
                    ret.Add(new System.Net.IPAddress(address));
                    offset += 4;
                }
                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OSPFv2HelloPacket"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OSPFv2HelloPacket"/>.</returns>
        public override string ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append((string) base.ToString());
            packet.Append(" ");
            packet.AppendFormat((string) "HelloOptions: {0} ", (object) this.HelloOptions);
            packet.AppendFormat((string) "RouterID: {0} ", (object) this.RouterID);
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