using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa
{
    /// <summary>
    /// Network-LSAs are the Type 2 LSAs. The LSA describes all routers
    /// attached to the network, including the Designated Router itself.
    /// </summary>
    public class NetworkLinksAdvertisement : LinkStateAdvertisement
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NetworkLinksAdvertisement()
        {
            var b = new byte[NetworkLinksAdvertisementFields.AttachedRouterPosition];
            Header = new ByteArraySegment(b);
            Type = LinkStateAdvertisementType.Network;
            Length = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Network LSA with a list of attached routers
        /// </summary>
        public NetworkLinksAdvertisement(List<IPAddress> routers)
        {
            var length = NetworkLinksAdvertisementFields.AttachedRouterPosition + routers.Count * IPv4BytesCount;
            var offset = NetworkLinksAdvertisementFields.AttachedRouterPosition;

            var b = new byte[length];
            foreach (var ip in routers)
            {
                Array.Copy(ip.GetAddressBytes(), 0, b, offset, IPv4BytesCount);
                offset += IPv4BytesCount;
            }

            Header = new ByteArraySegment(b);
            Type = LinkStateAdvertisementType.Network;
            Length = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        /// A <see cref="byte" />
        /// </param>
        /// <param name="offset">
        /// A <see cref="int" />
        /// </param>
        /// <param name="length">
        /// A <see cref="int" />
        /// </param>
        public NetworkLinksAdvertisement(byte[] packet, int offset, int length) :
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
                var routerCount = Length - NetworkMaskLength - OspfV2Fields.LSAHeaderLength;
                if (routerCount % IPv4BytesCount != 0)
                {
                    throw new Exception("Mallformed NetworkLinksAdvertisement - routerCount should be aligned to 4");
                }

                routerCount /= IPv4BytesCount;

                for (var i = 0; i < routerCount; i++)
                {
                    var adr = new byte[IPv4BytesCount];
                    Array.Copy(Header.Bytes, Header.Offset + NetworkLinksAdvertisementFields.AttachedRouterPosition + i * IPv4BytesCount, adr, 0, IPv4BytesCount);
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
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + NetworkLinksAdvertisementFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + NetworkLinksAdvertisementFields.NetworkMaskPosition,
                           address.Length);
            }
        }
    }
}