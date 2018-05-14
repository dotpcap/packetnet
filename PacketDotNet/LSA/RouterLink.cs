using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Router link contained in a RouterLSA.
    /// </summary>
    public class RouterLink
    {
        /// <summary>
        /// The length of the router link.
        /// </summary>
        public const Int32 RouterLinkLength = 12;

        internal ByteArraySegment Header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RouterLink()
        {
            var b = new Byte[RouterLinkLength];
            Header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs router link from a list of TOS metrics
        /// </summary>
        public RouterLink(IReadOnlyCollection<TOSMetric> metrics)
        {
            var length = RouterLinkLength + metrics.Count * TOSMetric.TOSMetricLength;
            var offset = LSA.RouterLinkFields.AdditionalMetricsPosition;
            var b = new Byte[length];

            foreach (var m in metrics)
            {
                Array.Copy(m.Bytes, 0, b, offset, TOSMetric.TOSMetricLength);
                offset += TOSMetric.TOSMetricLength;
            }

            Header = new ByteArraySegment(b);
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
        public RouterLink(Byte[] packet, Int32 offset, Int32 length)
        {
            Header = new ByteArraySegment(packet, offset, length);
        }

        /// <summary>
        /// bytes representation
        /// </summary>
        public Byte[] Bytes => Header.Bytes;

        /// <summary>
        /// Value again depends on the link's Type field. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public IPAddress LinkData
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + LSA.RouterLinkFields.LinkDataPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + LSA.RouterLinkFields.LinkDataPosition,
                           address.Length);
            }
        }


        /// <summary>
        /// Identifies the object that this router link connects to.  Value
        /// depends on the link's Type.
        /// </summary>
        public IPAddress LinkID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(Header.Bytes, Header.Offset + LSA.RouterLinkFields.LinkIDPosition);
                return new IPAddress(val);
            }
            set
            {
                var address = value.GetAddressBytes();
                Array.Copy(address,
                           0,
                           Header.Bytes,
                           Header.Offset + LSA.RouterLinkFields.LinkIDPosition,
                           address.Length);
            }
        }

        /// <summary>
        /// The cost of using this router link.
        /// </summary>
        public UInt16 Metric
        {
            get => EndianBitConverter.Big.ToUInt16(Header.Bytes, Header.Offset + LSA.RouterLinkFields.MetricPosition);
            set => EndianBitConverter.Big.CopyBytes(value, Header.Bytes, Header.Offset + LSA.RouterLinkFields.MetricPosition);
        }

        /// <summary>
        /// List of TOS metrics, contained in this LSA. Deprecated by RFC 4915
        /// </summary>
        public List<TOSMetric> TOSMetrics
        {
            get
            {
                var metrics = new List<TOSMetric>();

                for (var i = 0; i < TOSNumber; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(Header.Bytes, Header.Offset + LSA.RouterLinkFields.AdditionalMetricsPosition + i * TOSMetric.TOSMetricLength);
                    var m = new TOSMetric
                    {
                        TOS = (Byte) ((metric & 0xFF000000) >> 3),
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
        public Byte TOSNumber
        {
            get => Header.Bytes[Header.Offset + LSA.RouterLinkFields.TOSNumberPosition];
            set => Header.Bytes[Header.Offset + LSA.RouterLinkFields.TOSNumberPosition] = value;
        }

        /// <summary>
        /// A quick description of the router link. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public Byte Type
        {
            get => Header.Bytes[Header.Offset + LSA.RouterLinkFields.TypePosition];
            set => Header.Bytes[Header.Offset + LSA.RouterLinkFields.TypePosition] = value;
        }
    }
}