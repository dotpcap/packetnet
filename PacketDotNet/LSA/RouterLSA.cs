using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.LSA
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
        public static readonly LSAType LSAType = LSAType.Router;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RouterLSA()
        {
            var b = new byte[RouterLSAFields.RouterLinksStart];
            Header = new ByteArraySegment(b);
            LSType = LSAType;
            LinkNumber = 0;
            Length = (ushort) Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Router LSA with a list of router links
        /// </summary>
        public RouterLSA(List<RouterLink> links)
        {
            var length = 0;
            var offset = RouterLSAFields.RouterLinksStart;
            foreach (var l in links)
            {
                length += l.Bytes.Length;
            }

            length += RouterLSAFields.RouterLinksStart;

            var b = new byte[length];
            Header = new ByteArraySegment(b);
            foreach (var l in links)
            {
                Array.Copy(l.Bytes, 0, b, offset, l.Bytes.Length);
                offset += l.Bytes.Length;
            }

            LSType = LSAType;
            LinkNumber = (ushort) links.Count;
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
        public RouterLSA(byte[] packet, int offset, int length) :
            base(packet, offset, length)
        { }

        /// <summary>
        /// When set, the router is an area border router
        /// </summary>
        public int BBit
        {
            get
            {
                var flags = (byte) (Header.Bytes[Header.Offset + RouterLSAFields.RouterOptionsPosition] & 1);
                return flags;
            }
            set => Header.Bytes[Header.Offset + RouterLSAFields.RouterOptionsPosition] |= (byte) (value & 1);
        }

        /// <summary>
        /// When set, the router is an AS boundary router
        /// </summary>
        public int EBit
        {
            get
            {
                var flags = (byte) ((Header.Bytes[Header.Offset + RouterLSAFields.RouterOptionsPosition] >> 1) & 1);
                return flags;
            }
            set => Header.Bytes[Header.Offset + RouterLSAFields.RouterOptionsPosition] |= (byte) ((value & 1) << 1);
        }

        /// <summary>
        /// The number of the contained links in this RouterLSA
        /// </summary>
        public ushort LinkNumber
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + RouterLSAFields.LinkNumberPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + RouterLSAFields.LinkNumberPosition);
        }

        /// <summary>
        /// The contained router links in this RouterLSA
        /// </summary>
        /// <see cref="RouterLink" />
        public List<RouterLink> RouterLinks
        {
            get
            {
                var ret = new List<RouterLink>();

                var offset = Header.Offset + RouterLSAFields.RouterLinksStart;

                for (var i = 0; i < LinkNumber; i++)
                {
                    var l = new RouterLink(Header.Bytes, offset, RouterLink.RouterLinkLength);
                    ret.Add(l);
                    offset += RouterLink.RouterLinkLength;
                }

                return ret;
            }
        }

        /// <summary>
        /// When set, the router is an endpoint of one or more fully
        /// adjacent virtual links having the described area as Transit area
        /// </summary>
        public int VBit
        {
            get
            {
                var flags = (byte) ((Header.Bytes[Header.Offset + RouterLSAFields.RouterOptionsPosition] >> 2) & 1);
                return flags;
            }
            set => Header.Bytes[Header.Offset + RouterLSAFields.RouterOptionsPosition] |= (byte) ((value & 1) << 2);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the current <see cref="RouterLSA" />.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents the current <see cref="RouterLSA" />.</returns>
        public override string ToString()
        {
            var ret = new StringBuilder();
            ret.Append("Router LSA, ");
            ret.AppendFormat("V: {0}, E: {1}, B: {2}, ", VBit, EBit, BBit);
            ret.AppendFormat("links#: {0}", LinkNumber);
            return ret.ToString();
        }
    }
}