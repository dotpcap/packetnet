using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Router link contained in a RouterLSA.
    /// </summary>
    public class RouterLink
    {
        /// <summary>
        /// The length of the router link.
        /// </summary>
        public static readonly int RouterLinkLength = 12;
        internal ByteArraySegment header;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RouterLink()
        {
            byte[] b = new byte[RouterLinkLength];
            this.header = new ByteArraySegment(b);
        }

        /// <summary>
        /// Constructs router link from a list of TOS metrics
        /// </summary>
        public RouterLink(List<TOSMetric> metrics)
        {
            int length = RouterLinkLength + metrics.Count * TOSMetric.TOSMetricLength;
            int offset = RouterLinkFields.AdditionalMetricsPosition;
            byte[] b = new byte[length];

            foreach (TOSMetric m in metrics)
            {
                Array.Copy(m.Bytes, 0, b, offset, TOSMetric.TOSMetricLength);
                offset += TOSMetric.TOSMetricLength;
            }

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
        public RouterLink(byte[] packet, int offset, int length)
        {
            this.header = new ByteArraySegment(packet, offset, length);
        }


        /// <summary>
        /// Identifies the object that this router link connects to.  Value
        /// depends on the link's Type.
        /// </summary>
        public IPAddress LinkID
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + RouterLinkFields.LinkIDPosition);
                return new IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + RouterLinkFields.LinkIDPosition),
                    address.Length);
            }
        }

        /// <summary>
        /// Value again depends on the link's Type field. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public IPAddress LinkData
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + RouterLinkFields.LinkDataPosition);
                return new IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + RouterLinkFields.LinkDataPosition),
                    address.Length);
            }
        }

        /// <summary>
        /// A quick description of the router link. See http://www.ietf.org/rfc/rfc2328.txt for details.
        /// </summary>
        public byte Type
        {
            get
            {
                return this.header.Bytes[this.header.Offset + RouterLinkFields.TypePosition];
            }
            set
            {
                this.header.Bytes[this.header.Offset + RouterLinkFields.TypePosition] = value;
            }
        }

        /// <summary>
        /// The number of different TOS metrics given for this link, not
        /// counting the required link metric
        /// </summary>
        public byte TOSNumber
        {
            get
            {
                return this.header.Bytes[this.header.Offset + RouterLinkFields.TOSNumberPosition];
            }
            set
            {
                this.header.Bytes[this.header.Offset + RouterLinkFields.TOSNumberPosition] = value;
            }
        }

        /// <summary>
        /// The cost of using this router link.
        /// </summary>
        public ushort Metric
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(this.header.Bytes, this.header.Offset + RouterLinkFields.MetricPosition);
            }
            set
            {
                EndianBitConverter.Big.CopyBytes(value, this.header.Bytes, this.header.Offset + RouterLinkFields.MetricPosition);
            }
        }

        /// <summary>
        /// List of TOS metrics, contained in this LSA. Deprecated by RFC 4915
        /// </summary>
        public List<TOSMetric> TOSMetrics
        {
            get
            {
                List<TOSMetric> metrics = new List<TOSMetric>();

                for (int i = 0; i < this.TOSNumber; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + RouterLinkFields.AdditionalMetricsPosition + i * TOSMetric.TOSMetricLength);
                    TOSMetric m = new TOSMetric
                    {
                        TOS = (byte)((metric & 0xFF000000) >> 3),
                        Metric = metric & 0x00FFFFFF
                    };
                    metrics.Add(m);
                }
                return metrics;
            }
        }

        /// <summary>
        /// bytes representation
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