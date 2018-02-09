namespace PacketDotNet
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Summary-LSA.
    /// </summary>
    public class SummaryLSAFields : LSAFields
    {
        /// <summary> The length of the NetworkMask field in bytes</summary>
        public readonly static int NetworkMaskLength = 4;
        /// <summary> The length of the Metric field in bytes</summary>
        public readonly static int MetricLength = 4;
        /// <summary> The length of the TOSMetric field in bytes</summary>
        public readonly static int TOSMetricLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public readonly static int NetworkMaskPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public readonly static int MetricPosition;
        /// <summary> The relative postion of the TOSMetric field</summary>
        public readonly static int TOSMetricPosition;

        static SummaryLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            MetricPosition  = NetworkMaskPosition + NetworkMaskLength;
            TOSMetricPosition = MetricPosition + MetricLength;
        }
    }
}