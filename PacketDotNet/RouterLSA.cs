using System;
using System.Collections.Generic;
using System.Text;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Router-LSAs are the Type 1 LSAs. The LSA describes the state and cost of
    /// the router's links (i.e., interfaces) to the area.
    /// </summary>
    public class RouterLSA : LSA
    {
        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType lsaType = LSAType.Router;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RouterLSA()
        {
            byte[] b = new byte[RouterLSAFields.RouterLinksStart];
            this.header = new ByteArraySegment(b);
            this.LSType = lsaType;
            this.LinkNumber = 0;
            this.Length = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Router LSA with a list of router links
        /// </summary>
        public RouterLSA(List<RouterLink> links)
        {
            int length = 0;
            int offset = RouterLSAFields.RouterLinksStart;
            foreach (RouterLink l in links)
            {
                length += l.Bytes.Length;
            }
            length += RouterLSAFields.RouterLinksStart;

            byte[] b = new byte[length];
            this.header = new ByteArraySegment(b);
            foreach (RouterLink l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, l.Bytes.Length);
                offset += l.Bytes.Length;
            }

            this.LSType = lsaType;
            this.LinkNumber = (ushort)links.Count;
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
        public RouterLSA(byte[] packet, int offset, int length) :
            base(packet, offset, length)
        {

        }

        /// <summary>
        /// When set, the router is an endpoint of one or more fully
        /// adjacent virtual links having the described area as Transit area
        /// </summary>
        public int vBit
        {
            get
            {
                byte flags = (byte)((this.header.Bytes[this.header.Offset + RouterLSAFields.RouterOptionsPosition] >> 2) & 1);
                return flags;
            }
            set
            {
                this.header.Bytes[this.header.Offset + RouterLSAFields.RouterOptionsPosition] |= (byte)((value & 1) << 2);
            }
        }

        /// <summary>
        /// When set, the router is an AS boundary router
        /// </summary>
        public int eBit
        {
            get
            {
                byte flags = (byte)((this.header.Bytes[this.header.Offset + RouterLSAFields.RouterOptionsPosition] >> 1) & 1);
                return flags;
            }
            set
            {
                this.header.Bytes[this.header.Offset + RouterLSAFields.RouterOptionsPosition] |= (byte)((value & 1) << 1);
            }
        }

        /// <summary>
        /// When set, the router is an area border router
        /// </summary>
        public int bBit
        {
            get
            {
                byte flags = (byte)(this.header.Bytes[this.header.Offset + RouterLSAFields.RouterOptionsPosition] & 1);
                return flags;
            }
            set
            {
                this.header.Bytes[this.header.Offset + RouterLSAFields.RouterOptionsPosition] |= (byte)(value & 1);
            }
        }

        /// <summary>
        /// The number of the contained links in this RouterLSA
        /// </summary>
        public ushort LinkNumber
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(this.header.Bytes,this.header.Offset + RouterLSAFields.LinkNumberPosition);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + RouterLSAFields.LinkNumberPosition);
            }
        }

        /// <summary>
        /// The contained router links in this RouterLSA
        /// </summary>
        /// <see cref="PacketDotNet.RouterLink"/>
        public List<RouterLink> RouterLinks
        {
            get
            {
                List<RouterLink> ret = new List<RouterLink>();

                int offset = this.header.Offset + RouterLSAFields.RouterLinksStart;

                for (int i = 0; i < this.LinkNumber; i++)
                {
                    RouterLink l = new RouterLink(this.header.Bytes, offset, RouterLink.RouterLinkLength);
                    ret.Add(l);
                    offset += RouterLink.RouterLinkLength;
                }

                return ret;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="PacketDotNet.RouterLSA"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="PacketDotNet.RouterLSA"/>.</returns>
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.Append("Router LSA, ");
            ret.AppendFormat("V: {0}, E: {1}, B: {2}, ", this.vBit, this.eBit, this.bBit);
            ret.AppendFormat("links#: {0}", this.LinkNumber);
            return ret.ToString();
        }
    }
}