using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LSA
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
        public static readonly LSAType LSAType = LSAType.Network;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NetworkLSA()
        {
            var b = new Byte[NetworkLSAFields.AttachedRouterPosition];
            Header = new ByteArraySegment(b);
            LSType = LSAType;
            Length = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Network LSA with a list of attached routers
        /// </summary>
        public NetworkLSA(List<IPAddress> routers)
        {
            var length = NetworkLSAFields.AttachedRouterPosition + routers.Count * IPv4BytesCount;
            var offset = NetworkLSAFields.AttachedRouterPosition;

            var b = new Byte[length];
            foreach (var ip in routers)
            {
                Array.Copy(ip.GetAddressBytes(), 0, b, offset, IPv4BytesCount);
                offset += IPv4BytesCount;
            }

            Header = new ByteArraySegment(b);
            LSType = LSAType;
            Length = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        /// A <see cref="System.Int32" />
        /// </param>
        public NetworkLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        { }

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
                var ret = new List<IPAddress>();
                var routerCount = Length - NetworkMaskLength - OSPFv2Fields.LSAHeaderLength;
                if (routerCount % IPv4BytesCount != 0)
                {
                    throw new Exception("Mallformed NetworkLSA - routerCount should be aligned to 4");
                }

                routerCount /= IPv4BytesCount;

                for (var i = 0; i < routerCount; i++)
                {
                    var adr = new Byte[IPv4BytesCount];
                    Array.Copy(Header.Bytes, Header.Offset + NetworkLSAFields.AttachedRouterPosition + i * IPv4BytesCount, adr, 0, IPv4BytesCount);
                    var ip = new IPAddress(adr);
                    ret.Add(ip);
                }

                return ret;
            }
        }

        /// <summary>
        /// The IP address mask for the network.  For example, a class A
        /// network would have the mask 0xff000000 (255.0.0.0).
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + NetworkLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + NetworkLSAFields.NetworkMaskPosition,
                           address.Length);
            }
        }
    }
}