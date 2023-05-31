using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Lsa;

    /// <summary>
    /// Router link contained in a RouterLinksAdvertisement.
    /// </summary>
    public class RouterLink
    {
        /// <summary>
        /// The length of the router link.
        /// </summary>
        public const int RouterLinkLength = 12;

        private readonly ByteArraySegment _header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RouterLink()
        {
            var b = new byte[RouterLinkLength];
            _header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs router link from a list of TOS metrics
        /// </summary>
        public RouterLink(IReadOnlyCollection<TypeOfServiceMetric> metrics)
        {
            var length = RouterLinkLength + metrics.Count * TypeOfServiceMetric.Length;
            var offset = RouterLinkFields.AdditionalMetricsPosition;
            var b = new byte[length];

            foreach (var m in metrics)
            {
                Array.Copy(m.Bytes, 0, b, offset, TypeOfServiceMetric.Length);
                offset += TypeOfServiceMetric.Length;
            }

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
        public RouterLink(byte[] packet, int offset, int length)
        {
            _header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// bytes representation
        /// </summary>
        public byte[] Bytes => _header.Bytes;

        /// <summary>
        /// Value again depends on the link's Type field. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public IPAddress LinkData
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(_header.Bytes, _header.Offset + RouterLinkFields.LinkDataPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           _header.Bytes,
                           _header.Offset + RouterLinkFields.LinkDataPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// Identifies the object that this router link connects to.  Value
        /// depends on the link's Type.
        /// </summary>
        public IPAddress LinkId
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(_header.Bytes, _header.Offset + RouterLinkFields.LinkIdPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           _header.Bytes,
                           _header.Offset + RouterLinkFields.LinkIdPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The cost of using this router link.
        /// </summary>
        public ushort Metric
        {
            get => EndianBitConverter.Big.ToUInt16(_header.Bytes, _header.Offset + RouterLinkFields.MetricPosition);
            set => EndianBitConverter.Big.CopyBytes(value, _header.Bytes, _header.Offset + RouterLinkFields.MetricPosition);
        }

        /// <summary>
        /// List of TOS metrics, contained in this LSA. Deprecated by RFC 4915
        /// </summary>
        public List<TypeOfServiceMetric> TosMetrics
        {
            get
            {
                var metrics = new List<TypeOfServiceMetric>();

                for (var i = 0; i < TosNumber; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(_header.Bytes, _header.Offset + RouterLinkFields.AdditionalMetricsPosition + i * TypeOfServiceMetric.Length);
                    var m = new TypeOfServiceMetric
                    {
                        TypeOfService = (byte) ((metric & 0xFF000000) >> 3),
                        Metric = metric & 0x00FFFFFF
                    };

                    metrics.Add(m);
                }

                return metrics;
            }
        }

        /// <summary>
        /// The number of different TOS metrics given for this link, not
        /// counting the required link metric
        /// </summary>
        public byte TosNumber
        {
            get => _header.Bytes[_header.Offset + RouterLinkFields.TOSNumberPosition];
            set => _header.Bytes[_header.Offset + RouterLinkFields.TOSNumberPosition] = value;
        }

        /// <summary>
        /// A quick description of the router link. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public byte Type
        {
            get => _header.Bytes[_header.Offset + RouterLinkFields.TypePosition];
            set => _header.Bytes[_header.Offset + RouterLinkFields.TypePosition] = value;
        }
    }