using System;

namespace PacketDotNet.LSA
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a RouterLink
    /// </summary>
    public class RouterLinkFields
    {
        /// <summary> The relative postion of the AdditionalMetrics field</summary>
        public static readonly Int32 AdditionalMetricsPosition;

        /// <summary> The length of the LinkData field in bytes</summary>
        public static readonly Int32 LinkDataLength = 4;

        /// <summary> The relative postion of the LinkData field</summary>
        public static readonly Int32 LinkDataPosition;

        /// <summary> The length of the LinkID field in bytes</summary>
        public static readonly Int32 LinkIDLength = 4;

        /// <summary> The relative postion of the LinkID field</summary>
        public static readonly Int32 LinkIDPosition;

        /// <summary> The length of the Metric field in bytes</summary>
        public static readonly Int32 MetricLength = 2;

        /// <summary> The relative postion of the Metric field</summary>
        public static readonly Int32 MetricPosition;

        /// <summary> The length of the TOSNumber field in bytes</summary>
        public static readonly Int32 TOSNumberLength = 1;

        /// <summary> The relative postion of the TOSNumber field</summary>
        public static readonly Int32 TOSNumberPosition;

        /// <summary> The length of the Type field in bytes</summary>
        public static readonly Int32 TypeLength = 1;

        /// <summary> The relative postion of the Type field</summary>
        public static readonly Int32 TypePosition;

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