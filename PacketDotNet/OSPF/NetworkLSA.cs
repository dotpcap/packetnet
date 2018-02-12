using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Network-LSAs are the Type 2 LSAs. The LSA describes all routers
    /// attached to the network, including the Designated Router itself.
    /// </summary>
    public class NetworkLSA : LSA
    {
        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType lsaType = LSAType.Network;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NetworkLSA()
        {
            byte[] b = new byte[NetworkLSAFields.AttachedRouterPosition];
            this.header = new ByteArraySegment(b);
            this.LSType = lsaType;
            this.Length = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Network LSA with a list of attached routers
        /// </summary>
        public NetworkLSA(List<IPAddress> routers)
        {
            int length = NetworkLSAFields.AttachedRouterPosition + routers.Count * IPv4BytesCount;
            int offset = NetworkLSAFields.AttachedRouterPosition;

            byte[] b = new byte[length];
            foreach (IPAddress ip in routers)
            {
                Array.Copy((Array) ip.GetAddressBytes(), (int) 0, (Array) b, offset, (int) IPv4BytesCount);
                offset += IPv4BytesCount;
            }

            this.header = new ByteArraySegment(b);
            this.LSType = lsaType;
            this.Length = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32"/>
        /// </param>
        public NetworkLSA(byte[] packet, int offset, int length) :
            base(packet, offset, length)
        {

        }

        /// <summary>
        /// The IP address mask for the network.  For example, a class A
        /// network would have the mask 0xff000000 (255.0.0.0).
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {

                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + NetworkLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + NetworkLSAFields.NetworkMaskPosition),
                    address.Length);
            }
        }

        /// <summary>
        /// The Router IDs of each of the routers attached to the network.
        /// Actually, only those routers that are fully adjacent to the
        /// Designated Router are listed.  The Designated Router includes
        /// itself in this list.  The number of routers included can be
        /// deduced from the LSA header's length field.
        /// </summary>
        public List<IPAddress> AttachedRouters
        {
            get
            {
                List<IPAddress> ret = new List<IPAddress>();
                int routerCount = this.Length - NetworkMaskLength - OSPFv2Fields.LSAHeaderLength;
                if (routerCount % IPv4BytesCount != 0)
                {
                    throw new Exception("Mallformed NetworkLSA - routerCount should be aligned to 4");
                }

                routerCount /= IPv4BytesCount;

                for (int i = 0; i < routerCount; i++)
                {
                    byte[] adr = new byte[IPv4BytesCount];
                    Array.Copy((Array) this.header.Bytes, this.header.Offset + NetworkLSAFields.AttachedRouterPosition + i * IPv4BytesCount, (Array) adr, (int) 0, (int) IPv4BytesCount);
                    IPAddress ip = new IPAddress(adr);
                    ret.Add(ip);
                }

                return ret;
            }
        }
    }
}