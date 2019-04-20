using System;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa
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

        private readonly ByteArraySegment _header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ASExternalLink()
        {
            var b = new byte[Length];
            _header = new ByteArraySegment(b);
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
            _header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// Bytes representation
        /// </summary>
        public byte[] Bytes => _header.Bytes;

        /// <summary>
        /// The type of external metric.  If bit E is set, the metric
        /// specified is a Type 2 external metric.
        /// </summary>
        public byte EBit
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
                return (byte) ((val >> 31) & 0xFF);
            }
            set
            {
                var original = EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
                var val = (uint) ((value & 1) << 31) | original;
                EndianBitConverter.Big.CopyBytes(val, _header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// A 32-bit field attached to each external route.  This is not used by the OSPF protocol itself.
        /// </summary>
        public uint ExternalRouteTag
        {
            get => EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.ExternalRouteTagPosition);
            set => EndianBitConverter.Big.CopyBytes(value, _header.Bytes, _header.Offset + ASExternalLinkFields.ExternalRouteTagPosition);
        }

        /// <summary>
        /// Data traffic for the advertised destination will be forwarded to this address.
        /// </summary>
        public IPAddress ForwardingAddress
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.ForwardingAddressPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           _header.Bytes,
                           _header.Offset + ASExternalLinkFields.ForwardingAddressPosition,
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
                var val = EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                var original = EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
                var val = value & 0x00FFFFFF | original;
                EndianBitConverter.Big.CopyBytes(val, _header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }

        /// <summary>
        /// The Type of Service that the following fields concern.
        /// </summary>
        public byte Tos
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
                return (byte) ((val >> 24) & 0x7F);
            }
            set
            {
                var original = EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
                var val = (byte) ((value & 0x7F) << 24) | original;
                EndianBitConverter.Big.CopyBytes(val, _header.Bytes, _header.Offset + ASExternalLinkFields.TOSPosition);
            }
        }
    }
}