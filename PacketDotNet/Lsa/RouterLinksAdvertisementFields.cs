namespace PacketDotNet.Lsa;

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Router-LSA
    /// </summary>
    public struct RouterLinksAdvertisementFields
    {
        /// <summary>The length of the LinkNumber field in bytes</summary>
        public static readonly int LinkNumberLength = 2;

        /// <summary>The relative position of the LinkNumber field</summary>
        public static readonly int LinkNumberPosition;

        /// <summary>The relative position of the start of the RouterLink(s)</summary>
        public static readonly int RouterLinksStart;

        /// <summary>The length of the RouterOptions field in bytes</summary>
        public static readonly int RouterOptionsLength = 2;

        /// <summary>The relative position of the RouterOptions field</summary>
        public static readonly int RouterOptionsPosition;

        static RouterLinksAdvertisementFields()
        {
            RouterOptionsPosition = LinkStateFields.HeaderEnd;
            LinkNumberPosition = RouterOptionsPosition + RouterOptionsLength;
            RouterLinksStart = LinkNumberPosition + LinkNumberLength;
        }
    }