using System;
using System.Collections.Generic;
using System.Net;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Summary-LSAs are the Type 3 and 4 LSAs.  These LSAs are originated
    /// by area border routers. Summary-LSAs describe inter-area
    /// destinations. Type 3 summary-LSAs are used when the destination is an IP network,
    /// Type 4 - an AS boundary router.
    /// </summary>
    public class SummaryLSA : LSA
    {
        /// <summary>
        /// The type of the lsa.
        /// </summary>
        public static readonly LSAType lsaType = LSAType.Summary;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SummaryLSA()
        {
            byte[] b = new byte[SummaryLSAFields.TOSMetricPosition];
            this.header = new ByteArraySegment(b);
            this.LSType = lsaType;
            this.Length = (ushort)this.header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Summary LSA with a list of TOS metrics
        /// </summary>
        public SummaryLSA(List<TOSMetric> metrics)
        {
            int length = SummaryLSAFields.TOSMetricPosition + metrics.Count * TOSMetric.TOSMetricLength;
            int offset = SummaryLSAFields.TOSMetricPosition;
            byte[] b = new byte[length];

            foreach (TOSMetric m in metrics)
            {
                Array.Copy((Array) m.Bytes, (int) 0, (Array) b, offset, (int) TOSMetric.TOSMetricLength);
                offset += TOSMetric.TOSMetricLength;
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
        public SummaryLSA(byte[] packet, int offset, int length) :
            base(packet, offset, length)
        {

        }

        /// <summary>
        /// For Type 3 summary-LSAs, this indicates the destination
        /// network's IP address mask. This field is not meaningful
        /// and must be zero for Type 4 summary-LSAs.
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                var val = EndianBitConverter.Little.ToUInt32(this.header.Bytes, this.header.Offset + SummaryLSAFields.NetworkMaskPosition);
                return new System.Net.IPAddress(val);
            }
            set
            {
                byte[] address = value.GetAddressBytes();
                Array.Copy((Array) address, (int) 0,
                    (Array) this.header.Bytes, (int) (this.header.Offset + SummaryLSAFields.NetworkMaskPosition),
                    address.Length);
            }
        }

        /// <summary>
        /// The cost of this route.  Expressed in the same units as the interface costs in the router-LSAs.
        /// </summary>
        public uint Metric
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + SummaryLSAFields.MetricPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                var theValue = value & 0x00FFFFFF;
                EndianBitConverter.Big.CopyBytes(theValue, this.header.Bytes, this.header.Offset + SummaryLSAFields.MetricPosition);
            }
        }

        /// <summary>
        /// Additional TOS-specific information  for backward compatibility
        /// with previous versions of the OSPF specification
        /// </summary>
        public List<TOSMetric> TOSMetrics
        {
            get
            {
                List<TOSMetric> ret = new List<TOSMetric>();

                if ((this.Length - SummaryLSAFields.TOSMetricPosition) % TOSMetric.TOSMetricLength != 0)
                {
                    throw new Exception("Malformed summary LSA - bad TOSMetrics size");
                }

                int tosCnt = (this.Length - SummaryLSAFields.TOSMetricPosition) / TOSMetric.TOSMetricLength;

                for (int i = 0; i < tosCnt; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(this.header.Bytes, this.header.Offset + SummaryLSAFields.TOSMetricPosition + i * TOSMetric.TOSMetricLength);
                    TOSMetric m = new TOSMetric
                    {
                        TOS = (byte)((metric & 0xFF000000) >> 24),
                        Metric = metric & 0x00FFFFFF
                    };
                    ret.Add(m);
                }
                return ret;
            }
        }
    }
}