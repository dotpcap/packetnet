using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// AS-external-LSAs are the Type 5 LSAs.  These LSAs are originated by
    /// AS boundary routers, and describe destinations external to the AS.
    /// </summary>
    public class ASExternalLSA : LSA
    {
        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType LSAType = LSAType.ASExternal;

        private const Int32 ASExternalLinkLength = 12;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLSA()
        {
            Byte[] b = new Byte[ASExternalLSAFields.MetricPosition];
            this.Header = new ByteArraySegment(b);
            this.LSType = LSAType;
            this.Length = (UInt16)this.Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an ASExternalLSA with a list of ASExternalLinks
        /// </summary>
        public ASExternalLSA(List<ASExternalLink> links)
        {
            Int32 length = ASExternalLSAFields.MetricPosition + ASExternalLink.Length * links.Count;
            Int32 offset = ASExternalLSAFields.MetricPosition;
            Byte[] b = new Byte[length];

            foreach (ASExternalLink l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, ASExternalLink.Length);
                offset += ASExternalLink.Length;
            }

            this.Header = new ByteArraySegment(b);
            this.LSType = LSAType;
            this.Length = (UInt16)this.Header.Bytes.Length;
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
        public ASExternalLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        {

        }

        /// <summary>
        /// The IP address mask for the advertised destination.
        /// E.g. class A network - 255.0.0.0
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.Header.Bytes, this.Header.Offset + ASExternalLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.Header.Bytes, this.Header.Offset + ASExternalLSAFields.NetworkMaskPosition,
                    address.Length);
            }
        }

        /// <summary>
        /// The list of the AS-External-Links contained in this OSPF packet.
        /// </summary>
        public List<ASExternalLink> ASExternalLinks
        {
            get
            {
                Int32 linkCnt = (this.Length - NetworkMaskLength - OSPFv2Fields.LSAHeaderLength) / ASExternalLinkLength;
                List<ASExternalLink> ret = new List<ASExternalLink>(linkCnt);
                for(Int32 i = 0; i < linkCnt; i++)
                {
                    ASExternalLink l = new ASExternalLink(this.Header.Bytes,
                        this.Header.Offset + ASExternalLSAFields.MetricPosition + i * ASExternalLink.Length,
                        ASExternalLink.Length);
                    ret.Add(l);
                }

                return ret;
            }
        }
    }
}