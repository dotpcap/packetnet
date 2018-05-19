using System;

namespace PacketDotNet.LSA
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Summary-LSA.
    /// </summary>
    public class SummaryLSAFields : LSAFields
    {
        /// <summary> The length of the Metric field in bytes</summary>
        public static readonly Int32 MetricLength = 4;

        /// <summary> The relative postion of the Metric field</summary>
        public static readonly Int32 MetricPosition;

        /// <summary> The length of the NetworkMask field in bytes</summary>
        public static readonly Int32 NetworkMaskLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public static readonly Int32 NetworkMaskPosition;

        /// <summary> The length of the TOSMetric field in bytes</summary>
        public static readonly Int32 TOSMetricLength = 4;

        /// <summary> The relative postion of the TOSMetric field</summary>
        public static readonly Int32 TOSMetricPosition;

        static SummaryLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            MetricPosition = NetworkMaskPosition + NetworkMaskLength;
            TOSMetricPosition = MetricPosition + MetricLength;
        }
    }
}