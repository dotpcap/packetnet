using System;
using System.Net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Describes a particular external destination
    /// </summary>
    public class ASExternalLink
    {
        /// <summary>
        /// The length.
        /// </summary>
        public static readonly int Length = 12;
        internal ByteArraySegment header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLink()
        {
            byte[] b = new byte[ASExternalLink.Length];
            this.header = new ByteArraySegment(b);
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
        public ASExternalLink(byte[] packet, int offset, int length)
        {
            this.header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// The type of external metric.  If bit E is set, the metric
        /// specified is a Type 2 external metric.
        /// </summary>
        public byte eBit
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
                return (byte)((val >> 31) & 0xFF);
            }
            set
            {
                uint original = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
                uint val = (uint)((value & 1) << 31) | original;
                EndianBitConverter.Big.CopyBytes(val, this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// The Type of Service that the following fields concern.
        /// </summary>
        public byte TOS
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
                return (byte)((val >> 24) & 0x7F);
            }
            set
            {
                uint original = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
                var val = (byte)((value & 0x7F) << 24) | original;
                EndianBitConverter.Big.CopyBytes(val, this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// The cost of this route.  Interpretation depends on the external
        /// type indication (bit E above).
        /// </summary>
        public uint Metric
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                uint original = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
                var val = value & 0x00FFFFFF | original;
                EndianBitConverter.Big.CopyBytes(val, this.header.Bytes, this.header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// Data traffic for the advertised destination will be forwarded to this address.
        /// </summary>
        public IPAddress ForwardingAddress
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.ForwardingAddressPosition);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.header.Bytes, this.header.Offset + ASExternalLinkFields.ForwardingAddressPosition,
                    address.Length);
            }
        }

        /// <summary>
        ///  A 32-bit field attached to each external route.  This is not used by the OSPF protocol itself.
        /// </summary>
        public uint ExternalRouteTag
        {
            get
            {
                return EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + ASExternalLinkFields.ExternalRouteTagPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + ASExternalLinkFields.ExternalRouteTagPosition);
            }
        }

        /// <summary>
        /// Bytes representation
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                return this.header.Bytes;
            }
        }
    }
}