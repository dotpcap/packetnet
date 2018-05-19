using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LSA
{
    /// <summary>
    /// AS-external-LSAs are the Type 5 LSAs.  These LSAs are originated by
    /// AS boundary routers, and describe destinations external to the AS.
    /// </summary>
    public class ASExternalLSA : LSA
    {
        const Int32 ASExternalLinkLength = 12;

        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType LSAType = LSAType.ASExternal;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLSA()
        {
            var b = new Byte[ASExternalLSAFields.MetricPosition];
            Header = new ByteArraySegment(b);
            LSType = LSAType;
            Length = (UInt16) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an ASExternalLSA with a list of ASExternalLinks
        /// </summary>
        public ASExternalLSA(List<ASExternalLink> links)
        {
            var length = ASExternalLSAFields.MetricPosition + ASExternalLink.Length * links.Count;
            var offset = ASExternalLSAFields.MetricPosition;
            var b = new Byte[length];

            foreach (var l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, ASExternalLink.Length);
                offset += ASExternalLink.Length;
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
        public ASExternalLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// The list of the AS-External-Links contained in this OSPF packet.
        /// </summary>
        public List<ASExternalLink> ASExternalLinks
        {
            get
            {
                var linkCnt = (Length - NetworkMaskLength - OSPFv2Fields.LSAHeaderLength) / ASExternalLinkLength;
                var ret = new List<ASExternalLink>(linkCnt);
                for (var i = 0; i < linkCnt; i++)
                {
                    var l = new ASExternalLink(Header.Bytes,
                                               Header.Offset + ASExternalLSAFields.MetricPosition + i * ASExternalLink.Length,
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
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + ASExternalLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + ASExternalLSAFields.NetworkMaskPosition,
                           address.Length);
            }
        }
    }
}