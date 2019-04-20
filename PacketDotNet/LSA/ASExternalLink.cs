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

        internal ByteArraySegment Header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLink()
        {
            var b = new byte[Length];
            Header = new ByteArraySegment(b);
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
        public ASExternalLink(byte[] packet, int offset, int length)
        {
            Header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// Bytes representation
        /// </summary>
        public byte[] Bytes => Header.Bytes;

        /// <summary>
        /// The type of external metric.  If bit E is set, the metric
        /// specified is a Type 2 external metric.
        /// </summary>
        public byte EBit
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
                return (byte) ((val >> 31) & 0xFF);
            }
            set
            {
                var original = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
                var val = (uint) ((value & 1) << 31) | original;
                EndianBitConverter.Big.CopyBytes(val, Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// A 32-bit field attached to each external route.  This is not used by the OSPF protocol itself.
        /// </summary>
        public uint ExternalRouteTag
        {
            get => EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.ExternalRouteTagPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.ExternalRouteTagPosition);
        }

        /// <summary>
        /// Data traffic for the advertised destination will be forwarded to this address.
        /// </summary>
        public IPAddress ForwardingAddress
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.ForwardingAddressPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + LSA.ASExternalLinkFields.ForwardingAddressPosition,
                           address.Length);
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
                var val = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                var original = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
                var val = value & 0x00FFFFFF | original;
                EndianBitConverter.Big.CopyBytes(val, Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// The Type of Service that the following fields concern.
        /// </summary>
        public byte TOS
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
                return (byte) ((val >> 24) & 0x7F);
            }
            set
            {
                var original = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
                var val = (byte) ((value & 0x7F) << 24) | original;
                EndianBitConverter.Big.CopyBytes(val, Header.Bytes, Header.Offset + LSA.ASExternalLinkFields.TOSPosition);
            }
        }
    }
}