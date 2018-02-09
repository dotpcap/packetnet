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
        public static readonly LSAType lsaType = LSAType.ASExternal;

        const int ASExternalLinkLength = 12;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLSA()
        {
            byte[] b = new byte[ASExternalLSAFields.MetricPosition];
            this.header = new ByteArraySegment(b);
            this.LSType = lsaType;
            this.Length = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs an ASExternalLSA with a list of ASExternalLinks
        /// </summary>
        public ASExternalLSA(List<ASExternalLink> links)
        {
            int length = ASExternalLSAFields.MetricPosition + ASExternalLink.Length * links.Count;
            int offset = ASExternalLSAFields.MetricPosition;
            byte[] b = new byte[length];

            foreach (ASExternalLink l in links)
            {
                Array.Copy((Array) l.Bytes, (int) 0, (Array) b, offset, (int) ASExternalLink.Length);
                offset += ASExternalLink.Length;
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
        public ASExternalLSA(byte[] packet, int offset, int length) :
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
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLSAFields.NetworkMaskPosition);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + ASExternalLSAFields.NetworkMaskPosition),
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
                int linkCnt = (this.Length - NetworkMaskLength - OSPFv2Fields.LSAHeaderLength) / ASExternalLinkLength;
                List<ASExternalLink> ret = new List<ASExternalLink>(linkCnt);
                for(int i = 0; i < linkCnt; i++)
                {
                    ASExternalLink l = new ASExternalLink(this.header.Bytes,
                        this.header.Offset + ASExternalLSAFields.MetricPosition + i * ASExternalLink.Length,
                        ASExternalLink.Length);
                    ret.Add(l);
                }

                return ret;
            }
        }
    }
}