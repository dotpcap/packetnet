using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa
{
    /// <summary>
    /// AS-external-LSAs are the Type 5 LSAs.  These LSAs are originated by
    /// AS boundary routers, and describe destinations external to the AS.
    /// </summary>
    public class ASExternalLinkAdvertisement : LinkStateAdvertisement
    {
        private const int ASExternalLinkLength = 12;

        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LinkStateAdvertisementType LsaType = LinkStateAdvertisementType.ASExternal;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLinkAdvertisement()
        {
            var b = new byte[ASExternalLinkAdvertisementFields.MetricPosition];
            Header = new ByteArraySegment(b);
            Type = LsaType;
            Length = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an ASExternalLinkAdvertisement with a list of ASExternalLinks
        /// </summary>
        public ASExternalLinkAdvertisement(List<ASExternalLink> links)
        {
            var length = ASExternalLinkAdvertisementFields.MetricPosition + ASExternalLink.Length * links.Count;
            var offset = ASExternalLinkAdvertisementFields.MetricPosition;
            var b = new byte[length];

            foreach (var l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, ASExternalLink.Length);
                offset += ASExternalLink.Length;
            }

            Header = new ByteArraySegment(b);
            Type = LsaType;
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
        public ASExternalLinkAdvertisement(byte[] packet, int offset, int length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// The list of the AS-External-Links contained in this OSPF packet.
        /// </summary>
        public List<ASExternalLink> ASExternalLinks
        {
            get
            {
                var linkCnt = (Length - NetworkMaskLength - OspfV2Fields.LSAHeaderLength) / ASExternalLinkLength;
                var ret = new List<ASExternalLink>(linkCnt);
                for (var i = 0; i < linkCnt; i++)
                {
                    var l = new ASExternalLink(Header.Bytes,
                                               Header.Offset + ASExternalLinkAdvertisementFields.MetricPosition + i * ASExternalLink.Length,
                                               ASExternalLink.Length);

                    ret.Add(l);
                }

                return ret;
            }
        }

        /// <summary>
        /// The IP address mask for the advertised destination.
        /// E.g. class A network - 255.0.0.0
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + ASExternalLinkAdvertisementFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + ASExternalLinkAdvertisementFields.NetworkMaskPosition,
                           address.Length);
            }
        }
    }
}