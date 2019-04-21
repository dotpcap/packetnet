namespace PacketDotNet.Lsa
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Summary-LSA.
    /// </summary>
    public struct SummaryLinkAdvertisementFields
    {
        /// <summary>The length of the Metric field in bytes</summary>
        public static readonly int MetricLength = 4;

        /// <summary>The relative position of the Metric field</summary>
        public static readonly int MetricPosition;

        /// <summary>The length of the NetworkMask field in bytes</summary>
        public static readonly int NetworkMaskLength = 4;

        /// <summary>The relative position of the NetworkMask field</summary>
        public static readonly int NetworkMaskPosition;

        /// <summary>The length of the TypeOfServiceMetric field in bytes</summary>
        public static readonly int TosMetricLength = 4;

        /// <summary>The relative position of the TypeOfServiceMetric field</summary>
        public static readonly int TosMetricPosition;

        static SummaryLinkAdvertisementFields()
        {
            NetworkMaskPosition = LinkStateFields.HeaderEnd;
            MetricPosition = NetworkMaskPosition + NetworkMaskLength;
            TosMetricPosition = MetricPosition + MetricLength;
        }
    }
}