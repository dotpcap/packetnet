/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2011 Georgi Baychev <georgi.baychev@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// OSPFv2 packet.
    /// </summary>
    [Serializable]
    public abstract class OSPFv2Packet : OSPFPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive Log;
#pragma warning restore 0169
#endif
        /// <value>
        /// Version number of this OSPF protocol
        /// </value>
        public static OSPFVersion ospfVersion = OSPFVersion.OSPFv2;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OSPFv2Packet()
        {
            Log.Debug("");

            // allocate memory for this packet
            Int32 offset = 0;
            Int32 length = OSPFv2Fields.HeaderLength;
            var headerBytes = new Byte[length];
            Header = new ByteArraySegment(headerBytes, offset, length);

            Version = ospfVersion;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        public OSPFv2Packet(Byte[] Bytes, Int32 Offset)
        {
            Log.Debug("");
            Header = new ByteArraySegment(Bytes, Offset, OSPFv2Fields.HeaderLength);
            Version = ospfVersion;
        }

        /// <summary>
        /// The OSPF version number.
        /// </summary>
        public OSPFVersion Version
        {
            get => (OSPFVersion) Header.Bytes[Header.Offset + OSPFv2Fields.VersionPosition];

            set => Header.Bytes[Header.Offset + OSPFv2Fields.VersionPosition] = (Byte) value;
        }

        /// <summary>
        /// The OSPF packet types - see http://www.ietf.org/rfc/rfc2328.txt for details
        /// </summary>
        public virtual OSPFPacketType Type
        {
            get => (OSPFPacketType) Header.Bytes[Header.Offset + OSPFv2Fields.TypePosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.TypePosition] = (Byte) value;
        }

        /// <summary>
        /// The length of the OSPF protocol packet in bytes.
        /// </summary>
        public virtual UInt16 PacketLength
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.PacketLengthPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.PacketLengthPosition);
        }

        /// <summary>
        /// The Router ID of the packet's source.
        /// </summary>
        public virtual IPAddress RouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.RouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OSPFv2Fields.RouterIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Identifies the area that this packet belongs to. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual IPAddress AreaID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.AreaIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + OSPFv2Fields.AreaIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The standard IP checksum of the entire contents of the packet,
        /// except the 64-bit authentication field
        /// </summary>
        public virtual UInt16 Checksum
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.ChecksumPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.ChecksumPosition);
        }

        /// <summary>
        /// Authentication procedure. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual UInt16 AuType
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.AuTypePosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.AuTypePosition);
        }

        /// <summary>
        /// A 64-bit field for use by the authentication scheme
        /// </summary>
        public virtual UInt64 Authentication
        {
            get => EndianBitConverter.Big.ToUInt64(Header.Bytes, Header.Offset + OSPFv2Fields.AuthorizationPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.AuthorizationPosition);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2Packet" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2Packet" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.AppendFormat("OSPFv2 packet, type {0} ", Type);
            packet.AppendFormat("length: {0}, ", PacketLength);
            packet.AppendFormat("Checksum: {0:X8}, ", Checksum);
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
        /// <param name="NetworkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="HelloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="RouterDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket
        (
            IPAddress NetworkMask,
            UInt16 HelloInterval,
            UInt16 RouterDeadInterval)
        {
            Byte[] b = new Byte[OSPFv2Fields.NeighborIDStart];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.NeighborIDStart);
            Type = packetType;

            this.NetworkMask = NetworkMask;
            this.HelloInterval = HelloInterval;
            this.RouterDeadInterval = RouterDeadInterval;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from NetworkMask, HelloInterval and
        /// RouterDeadInterval and a list of neighbor routers.
        /// </summary>
        /// <param name="NetworkMask">The Network mask - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="HelloInterval">The Hello interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="RouterDeadInterval">The Router dead interval - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        /// <param name="Neighbors">List of router neighbors - see http://www.ietf.org/rfc/rfc2328.txt for details.</param>
        public OSPFv2HelloPacket
        (
            IPAddress NetworkMask,
            UInt16 HelloInterval,
            UInt16 RouterDeadInterval,
            List<IPAddress> Neighbors)
            : this(NetworkMask, HelloInterval, RouterDeadInterval)
        {
            Int32 length = Neighbors.Count * 4;
            Int32 offset = OSPFv2Fields.NeighborIDStart;
            Byte[] bytes = new Byte[length + OSPFv2Fields.NeighborIDStart];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            for (Int32 i = 0; i < Neighbors.Count; i++)
            {
                Array.Copy(Neighbors[i].GetAddressBytes(), 0, bytes, offset, 4); //4 bytes per address
                offset += 4;
            }

            Header = new ByteArraySegment(bytes);
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Hello packet from given bytes and offset.
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32" />
        /// </param>
        public OSPFv2HelloPacket(Byte[] Bytes, Int32 Offset) :
            base(Bytes, Offset)
        {
            Type = packetType;
        }


        /// <summary>
        /// The identity of the Backup Designated Router for this network,
        /// in the view of the sending router.
        /// </summary>
        public virtual IPAddress BackupRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.BackupRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
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
        public virtual IPAddress DesignatedRouterID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.DesignatedRouterIDPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
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
        public virtual UInt16 HelloInterval
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + OSPFv2Fields.HelloIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.HelloIntervalPosition);
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual Byte HelloOptions
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.HelloOptionsPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.HelloOptionsPosition] = value;
        }

        /// <summary>
        /// List of the Router IDs of each router from whom valid Hello packets have
        /// been seen recently on the network.  Recently means in the last
        /// RouterDeadInterval seconds. Can be zero or more.
        /// </summary>
        public virtual List<IPAddress> NeighborID
        {
            get
            {
                List<IPAddress> ret = new List<IPAddress>();
                Int32 bytesAvailable = PacketLength - OSPFv2Fields.NeighborIDStart;

                if (bytesAvailable % 4 != 0)
                {
                    throw new Exception("malformed OSPFv2Hello Packet - bad NeighborID size");
                }

                Int32 offset = OSPFv2Fields.NeighborIDStart;
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
        public virtual IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.NetworkMaskPositon);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
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
        public virtual UInt32 RouterDeadInterval
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.RouterDeadIntervalPosition);
        }

        /// <summary>
        /// This router's Router Priority.
        /// </summary>
        public virtual Byte RtrPriority
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.RtrPriorityPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.RtrPriorityPosition] = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2HelloPacket" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2HelloPacket" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
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
            Byte[] b = new Byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            Type = packetType;

            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 DD packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2DDPacket(List<LSA> lsas)
        {
            Int32 length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            Int32 offset = OSPFv2Fields.LSAHeaderPosition;
            Byte[] bytes = new Byte[length + OSPFv2Fields.LSAHeaderPosition];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, 20); //20 bytes per header
                offset += 20;
            }

            Header = new ByteArraySegment(bytes);
            Type = packetType;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32" />
        /// </param>
        public OSPFv2DDPacket(Byte[] Bytes, Int32 Offset) :
            base(Bytes, Offset)
        {
            Type = packetType;
        }

        /// <summary>
        /// DD Packet bits - See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual Byte DBDescriptionBits
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.BitsPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.BitsPosition] = value;
        }

        /// <summary>
        /// The optional capabilities supported by the router. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public virtual Byte DBDescriptionOptions
        {
            get => Header.Bytes[Header.Offset + OSPFv2Fields.DBDescriptionOptionsPosition];
            set => Header.Bytes[Header.Offset + OSPFv2Fields.DBDescriptionOptionsPosition] = value;
        }

        /// <summary>
        /// Used to sequence the collection of Database Description Packets.
        /// </summary>
        public virtual UInt32 DDSequence
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.DDSequencePosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.DDSequencePosition);
        }


        /// <summary>
        /// The size in bytes of the largest IP datagram that can be sent
        /// out the associated interface, without fragmentation.
        /// </summary>
        public virtual UInt16 InterfaceMTU
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
        public virtual List<LSA> LSAHeader
        {
            get
            {
                List<LSA> ret = new List<LSA>();
                Int32 bytesNeeded = PacketLength - OSPFv2Fields.LSAHeaderPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 DD Packet - Invalid LSA headers count");
                }

                Int32 offset = Header.Offset + OSPFv2Fields.LSAHeaderPosition;
                Int32 headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (Int32 i = 0; i < headerCount; i++)
                {
                    LSA l = new LSA(Header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    offset += OSPFv2Fields.LSAHeaderLength;
                    ret.Add(l);
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2DDPacket" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2DDPacket" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
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

    /// <summary>
    /// Link State Request packets are OSPF packet type 3.
    /// The Link State Request packet is used to request the pieces of the
    /// neighbor's database that are more up-to-date.
    /// See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2LSRequestPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType packetType = OSPFPacketType.LinkStateRequest;

        /// <summary>
        /// Constructs an OSPFv2 LSR packet
        /// </summary>
        public OSPFv2LSRequestPacket()
        {
            Type = packetType;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSR packet with link state requests
        /// </summary>
        /// <param name="lsrs">List of the link state requests</param>
        public OSPFv2LSRequestPacket(List<LinkStateRequest> lsrs)
        {
            Int32 length = lsrs.Count * LinkStateRequest.Length;
            Int32 offset = OSPFv2Fields.HeaderLength;
            Byte[] bytes = new Byte[length + OSPFv2Fields.HeaderLength];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            for (Int32 i = 0; i < lsrs.Count; i++)
            {
                Array.Copy(lsrs[i].Bytes, 0, bytes, offset, LinkStateRequest.Length);
                offset += LinkStateRequest.Length;
            }

            Header = new ByteArraySegment(bytes);
            Type = packetType;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 LSR packet from ByteArraySegment
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public OSPFv2LSRequestPacket(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas.Bytes);
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32" />
        /// </param>
        public OSPFv2LSRequestPacket(Byte[] Bytes, Int32 Offset) :
            base(Bytes, Offset)
        {
            Type = packetType;
        }

        /// <summary>
        /// A list of link state requests, contained in this packet
        /// </summary>
        /// See
        /// <see cref="PacketDotNet.LinkStateRequest" />
        public virtual List<LinkStateRequest> LinkStateRequests
        {
            get
            {
                Int32 bytesNeeded = PacketLength - OSPFv2Fields.LSRStart;
                if (bytesNeeded % LinkStateRequest.Length != 0)
                {
                    throw new Exception("Malformed LSR packet - bad size for the LS requests");
                }

                List<LinkStateRequest> ret = new List<LinkStateRequest>();
                Int32 offset = Header.Offset + OSPFv2Fields.LSRStart;
                Int32 lsrCount = bytesNeeded / LinkStateRequest.Length;

                for (Int32 i = 0; i < lsrCount; i++)
                {
                    LinkStateRequest request = new LinkStateRequest(Header.Bytes, offset, LinkStateRequest.Length);
                    ret.Add(request);
                    offset += LinkStateRequest.Length;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2LSRequestPacket" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2LSRequestPacket" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
            packet.Append(base.ToString());
            packet.Append(" ");
            packet.AppendFormat("LSR count: {0} ", LinkStateRequests.Count);
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

    /// <summary>
    /// Link State Update packets are OSPF packet type 4.  These packets
    /// implement the flooding of LSAs. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2LSUpdatePacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType packetType = OSPFPacketType.LinkStateUpdate;

        /// <summary>
        /// Constructs an OSPFv2 Link state update packet
        /// </summary>
        public OSPFv2LSUpdatePacket()
        {
            Byte[] b = new Byte[OSPFv2Fields.HeaderLength + 4];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.LSRStart);
            Type = packetType;

            PacketLength = (UInt16) Header.Bytes.Length;
            LSANumber = 0;
        }

        /// <summary>
        /// Constructs an OSPFv2 link state update with LSAs
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSUpdatePacket(List<LSA> lsas)
        {
            Int32 length = 0;
            Int32 offset = OSPFv2Fields.HeaderLength + OSPFv2Fields.LSANumberLength;

            //calculate the length for the LSAs
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                length += lsas[i].Bytes.Length;
            }

            Byte[] bytes = new Byte[length + offset];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, lsas[i].Bytes.Length);
                offset += lsas[i].Bytes.Length;
            }

            Header = new ByteArraySegment(bytes);
            Type = packetType;
            PacketLength = (UInt16) Header.Bytes.Length;
            LSANumber = (UInt32) lsas.Count;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32" />
        /// </param>
        public OSPFv2LSUpdatePacket(Byte[] Bytes, Int32 Offset) :
            base(Bytes, Offset)
        {
            Type = packetType;
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
        public virtual UInt32 LSANumber
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + OSPFv2Fields.LSANumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + OSPFv2Fields.LSANumberPosition);
        }

        /// <summary>
        /// A list of LSA, contained in this packet
        /// </summary>
        /// See
        /// <see cref="PacketDotNet.LSA" />
        public virtual List<LSA> LSAUpdates
        {
            get
            {
                List<LSA> ret = new List<LSA>();

                Int32 offset = Header.Offset + OSPFv2Fields.LSAUpdatesPositon;
                for (Int32 i = 0; i < LSANumber; i++)
                {
                    LSA l = new LSA(Header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
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
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2LSUpdatePacket" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2LSUpdatePacket" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
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

    /// <summary>
    /// Link State Acknowledgment Packets are OSPF packet type 5.  To make
    /// the flooding of LSAs reliable, flooded LSAs are explicitly
    /// acknowledged. See http://www.ietf.org/rfc/rfc2328.txt for details.
    /// </summary>
    public class OSPFv2LSAPacket : OSPFv2Packet
    {
        /// <value>
        /// The packet type
        /// </value>
        public static OSPFPacketType packetType = OSPFPacketType.LinkStateAcknowledgment;

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
            Byte[] b = new Byte[OSPFv2Fields.LSAHeaderPosition];
            Array.Copy(Header.Bytes, b, Header.Bytes.Length);
            Header = new ByteArraySegment(b, 0, OSPFv2Fields.LSAHeaderPosition);
            Type = packetType;

            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an OSPFv2 Link State Acknowledge packet with LSA headers
        /// </summary>
        /// <param name="lsas">List of the LSA headers</param>
        public OSPFv2LSAPacket(List<LSA> lsas)
        {
            Int32 length = lsas.Count * OSPFv2Fields.LSAHeaderLength;
            Int32 offset = OSPFv2Fields.HeaderLength;
            Byte[] bytes = new Byte[length + OSPFv2Fields.HeaderLength];

            Array.Copy(Header.Bytes, bytes, Header.Length);
            for (Int32 i = 0; i < lsas.Count; i++)
            {
                Array.Copy(lsas[i].Bytes, 0, bytes, offset, OSPFv2Fields.LSAHeaderLength);
                offset += 20;
            }

            Header = new ByteArraySegment(bytes);
            Type = packetType;
            PacketLength = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32" />
        /// </param>
        public OSPFv2LSAPacket(Byte[] Bytes, Int32 Offset) :
            base(Bytes, Offset)
        {
            Type = packetType;
        }

        /// <summary>
        /// List of LSA acknowledgements.
        /// </summary>
        public virtual List<LSA> LSAAcknowledge
        {
            get
            {
                List<LSA> ret = new List<LSA>();
                Int32 bytesNeeded = PacketLength - OSPFv2Fields.LSAAckPosition;

                if (bytesNeeded % OSPFv2Fields.LSAHeaderLength != 0)
                {
                    throw new Exception("OSPFv2 LSA Packet - Invalid LSA headers count");
                }

                Int32 offset = Header.Offset + OSPFv2Fields.LSAAckPosition;
                Int32 headerCount = bytesNeeded / OSPFv2Fields.LSAHeaderLength;

                for (Int32 i = 0; i < headerCount; i++)
                {
                    LSA l = new LSA(Header.Bytes, offset, OSPFv2Fields.LSAHeaderLength);
                    ret.Add(l);
                    offset += OSPFv2Fields.LSAHeaderLength;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2LSAPacket" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="PacketDotNet.OSPFv2LSAPacket" />.</returns>
        public override String ToString()
        {
            StringBuilder packet = new StringBuilder();
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