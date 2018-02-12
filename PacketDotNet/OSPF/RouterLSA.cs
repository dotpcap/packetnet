using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    ///     Router-LSAs are the Type 1 LSAs. The LSA describes the state and cost of
    ///     the router's links (i.e., interfaces) to the area.
    /// </summary>
    public class RouterLSA : LSA
    {
        /// <summary>
        ///     The type of the lsa.
        /// </summary>
        public static readonly LSAType LSAType = LSAType.Router;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public RouterLSA()
        {
            Byte[] b = new Byte[RouterLSAFields.RouterLinksStart];
            this.Header = new ByteArraySegment(b);
            this.LSType = LSAType;
            this.LinkNumber = 0;
            this.Length = (UInt16) this.Header.Bytes.Length;
        }

        /// <summary>
        ///     Constructs a Router LSA with a list of router links
        /// </summary>
        public RouterLSA(List<RouterLink> links)
        {
            Int32 length = 0;
            Int32 offset = RouterLSAFields.RouterLinksStart;
            foreach (RouterLink l in links)
            {
                length += l.Bytes.Length;
            }

            length += RouterLSAFields.RouterLinksStart;

            Byte[] b = new Byte[length];
            this.Header = new ByteArraySegment(b);
            foreach (RouterLink l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, l.Bytes.Length);
                offset += l.Bytes.Length;
            }

            this.LSType = LSAType;
            this.LinkNumber = (UInt16) links.Count;
            this.Length = (UInt16) this.Header.Bytes.Length;
        }

        /// <summary>
        ///     Constructs a packet from bytes and offset and length
        /// </summary>
        /// <param name="packet">
        ///     A <see cref="System.Byte" />
        /// </param>
        /// <param name="offset">
        ///     A <see cref="System.Int32" />
        /// </param>
        /// <param name="length">
        ///     A <see cref="System.Int32" />
        /// </param>
        public RouterLSA(Byte[] packet, Int32 offset, Int32 length) :
            base(packet, offset, length)
        {
        }

        /// <summary>
        ///     When set, the router is an area border router
        /// </summary>
        public Int32 BBit
        {
            get
            {
                Byte flags = (Byte) (this.Header.Bytes[this.Header.Offset + RouterLSAFields.RouterOptionsPosition] & 1);
                return flags;
            }
            set => this.Header.Bytes[this.Header.Offset + RouterLSAFields.RouterOptionsPosition] |= (Byte) (value & 1);
        }

        /// <summary>
        ///     When set, the router is an AS boundary router
        /// </summary>
        public Int32 EBit
        {
            get
            {
                Byte flags =
                    (Byte) ((this.Header.Bytes[this.Header.Offset + RouterLSAFields.RouterOptionsPosition] >> 1) & 1);
                return flags;
            }
            set => this.Header.Bytes[this.Header.Offset + RouterLSAFields.RouterOptionsPosition] |=
                (Byte) ((value & 1) << 1);
        }

        /// <summary>
        ///     The number of the contained links in this RouterLSA
        /// </summary>
        public UInt16 LinkNumber
        {
            get => EndianBitConverter.Big.ToUInt16(this.Header.Bytes,
                this.Header.Offset + RouterLSAFields.LinkNumberPosition);

            set => EndianBitConverter.Big.CopyBytes(value, this.Header.Bytes,
                this.Header.Offset + RouterLSAFields.LinkNumberPosition);
        }

        /// <summary>
        ///     The contained router links in this RouterLSA
        /// </summary>
        /// <see cref="RouterLink" />
        public List<RouterLink> RouterLinks
        {
            get
            {
                List<RouterLink> ret = new List<RouterLink>();

                Int32 offset = this.Header.Offset + RouterLSAFields.RouterLinksStart;

                for (Int32 i = 0; i < this.LinkNumber; i++)
                {
                    RouterLink l = new RouterLink(this.Header.Bytes, offset, RouterLink.RouterLinkLength);
                    ret.Add(l);
                    offset += RouterLink.RouterLinkLength;
                }

                return ret;
            }
        }

        /// <summary>
        ///     When set, the router is an endpoint of one or more fully
        ///     adjacent virtual links having the described area as Transit area
        /// </summary>
        public Int32 VBit
        {
            get
            {
                Byte flags =
                    (Byte) ((this.Header.Bytes[this.Header.Offset + RouterLSAFields.RouterOptionsPosition] >> 2) & 1);
                return flags;
            }
            set => this.Header.Bytes[this.Header.Offset + RouterLSAFields.RouterOptionsPosition] |=
                (Byte) ((value & 1) << 2);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents the current <see cref="RouterLSA" />.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current <see cref="RouterLSA" />.</returns>
        public override String ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append("Router LSA, ");
            ret.AppendFormat("V: {0}, E: {1}, B: {2}, ", this.VBit, this.EBit, this.BBit);
            ret.AppendFormat("links#: {0}", this.LinkNumber);
            return ret.ToString();
        }
    }
}