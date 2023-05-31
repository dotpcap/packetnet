namespace PacketDotNet.Lsa;

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a AS-External-LSA
    /// </summary>
    public struct ASExternalLinkAdvertisementFields
    {
        /// <summary>The relative position of the Metric field</summary>
        public static readonly int MetricPosition;

        /// <summary>The length of the NetworkMask field in bytes</summary>
        public static readonly int NetworkMaskLength = 4;

        /// <summary>The relative position of the NetworkMask field</summary>
        public static readonly int NetworkMaskPosition;

        static ASExternalLinkAdvertisementFields()
        {
            NetworkMaskPosition = LinkStateFields.HeaderEnd;
            MetricPosition = NetworkMaskPosition + NetworkMaskLength;
        }
    }