namespace PacketDotNet
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a RouterLink
    /// </summary>
    public class RouterLinkFields
    {
        /// <summary> The length of the LinkID field in bytes</summary>
        public readonly static int LinkIDLength = 4;
        /// <summary> The length of the LinkData field in bytes</summary>
        public readonly static int LinkDataLength = 4;
        /// <summary> The length of the Type field in bytes</summary>
        public readonly static int TypeLength = 1;
        /// <summary> The length of the TOSNumber field in bytes</summary>
        public readonly static int TOSNumberLength = 1;
        /// <summary> The length of the Metric field in bytes</summary>
        public readonly static int MetricLength = 2;

        /// <summary> The relative postion of the LinkID field</summary>
        public readonly static int LinkIDPosition;
        /// <summary> The relative postion of the LinkData field</summary>
        public readonly static int LinkDataPosition;
        /// <summary> The relative postion of the Type field</summary>
        public readonly static int TypePosition;
        /// <summary> The relative postion of the TOSNumber field</summary>
        public readonly static int TOSNumberPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public readonly static int MetricPosition;
        /// <summary> The relative postion of the AdditionalMetrics field</summary>
        public readonly static int AdditionalMetricsPosition;

        static RouterLinkFields()
        {
            LinkIDPosition = 0;
            LinkDataPosition = LinkIDPosition + LinkIDLength;
            TypePosition = LinkDataPosition + LinkDataLength;
            TOSNumberPosition = TypePosition + TypeLength;
            MetricPosition = TOSNumberPosition + TOSNumberLength;
            AdditionalMetricsPosition = MetricPosition + MetricLength;
        }
    }
}