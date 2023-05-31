namespace PacketDotNet.Lsa;

    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a LinkStateRequest
    /// </summary>
    public struct LinkStateRequestFields
    {
        /// <summary>The length of the AdvertisingRouter field in bytes</summary>
        public static readonly int AdvertisingRouterLength = 4;

        /// <summary>The relative position of the AdvertisingRouter field</summary>
        public static readonly int AdvertisingRouterPosition;

        /// <summary>The length of the LinkStateID field in bytes</summary>
        public static readonly int LinkStateIdLength = 4;

        /// <summary>The relative position of the LinkStateID field</summary>
        public static readonly int LinkStateIdPosition;

        /// <summary>The length of the LSType field in bytes</summary>
        public static readonly int LinkStateTypeLength = 4;

        /// <summary>The relative position of the LSType field</summary>
        public static readonly int LinkStateTypePosition;

        static LinkStateRequestFields()
        {
            LinkStateTypePosition = 0;
            LinkStateIdPosition = LinkStateTypePosition + LinkStateTypeLength;
            AdvertisingRouterPosition = LinkStateIdPosition + LinkStateIdLength;
        }
    }