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
        public static readonly LSAType LSAType = LSAType.Summary;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SummaryLSA()
        {
            Byte[] b = new Byte[SummaryLSAFields.TOSMetricPosition];
            this.Header = new ByteArraySegment(b);
            this.LSType = LSAType;
            this.Length = (UInt16)this.Header.Bytes.Length;
        }

        /// <summary>
        /// Constructs a Summary LSA with a list of TOS metrics
        /// </summary>
        public SummaryLSA(List<TOSMetric> metrics)
        {
            Int32 length = SummaryLSAFields.TOSMetricPosition + metrics.Count * TOSMetric.TOSMetricLength;
            Int32 offset = SummaryLSAFields.TOSMetricPosition;
            Byte[] b = new Byte[length];

            foreach (TOSMetric m in metrics)
            {
                Array.Copy(m.Bytes, 0, b, offset, TOSMetric.TOSMetricLength);
                offset += TOSMetric.TOSMetricLength;
            }

            this.Header = new ByteArraySegment(b);
            this.LSType = LSAType;
            this.Length = (UInt16)this.Header.Bytes.Length;
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
        public SummaryLSA(Byte[] packet, Int32 offset, Int32 length) :
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
                var val = EndianBitConverter.Little.ToUInt32(this.Header.Bytes, this.Header.Offset + SummaryLSAFields.NetworkMaskPosition);
                return new IPAddress(val);
            }
            set
            {
                Byte[] address = value.GetAddressBytes();
                Array.Copy(address, 0,
                    this.Header.Bytes, this.Header.Offset + SummaryLSAFields.NetworkMaskPosition,
                    address.Length);
            }
        }

        /// <summary>
        /// The cost of this route.  Expressed in the same units as the interface costs in the router-LSAs.
        /// </summary>
        public UInt32 Metric
        {
            get
            {
                var val = EndianBitConverter.Big.ToUInt32(this.Header.Bytes, this.Header.Offset + SummaryLSAFields.MetricPosition);
                return val & 0x00FFFFFF;
            }
            set
            {
                var theValue = value & 0x00FFFFFF;
                EndianBitConverter.Big.CopyBytes(theValue, this.Header.Bytes, this.Header.Offset + SummaryLSAFields.MetricPosition);
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

                Int32 tosCnt = (this.Length - SummaryLSAFields.TOSMetricPosition) / TOSMetric.TOSMetricLength;

                for (Int32 i = 0; i < tosCnt; i++)
                {
                    var metric = EndianBitConverter.Big.ToUInt32(this.Header.Bytes, this.Header.Offset + SummaryLSAFields.TOSMetricPosition + i * TOSMetric.TOSMetricLength);
                    TOSMetric m = new TOSMetric
                    {
                        TOS = (Byte)((metric & 0xFF000000) >> 24),
                        Metric = metric & 0x00FFFFFF
                    };
                    ret.Add(m);
                }
                return ret;
            }
        }
    }
}