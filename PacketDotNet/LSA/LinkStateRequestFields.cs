using System;

namespace PacketDotNet.LSA
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a LinkStateRequest
    /// </summary>
    public class LinkStateRequestFields
    {
        /// <summary> The length of the AdvertisingRouter field in bytes</summary>
        public static readonly Int32 AdvertisingRouterLength = 4;

        /// <summary> The relative postion of the AdvertisingRouter field</summary>
        public static readonly Int32 AdvertisingRouterPosition;

        /// <summary> The length of the LinkStateID field in bytes</summary>
        public static readonly Int32 LinkStateIdLength = 4;

        /// <summary> The relative postion of the LinkStateID field</summary>
        public static readonly Int32 LinkStateIdPosition;

        /// <summary> The length of the LSType field in bytes</summary>
        public static readonly Int32 LSTypeLength = 4;

        /// <summary> The relative postion of the LSType field</summary>
        public static readonly Int32 LSTypePosition;

        static LinkStateRequestFields()
        {
            LSTypePosition = 0;
            LinkStateIdPosition = LSTypePosition + LSTypeLength;
            AdvertisingRouterPosition = LinkStateIdPosition + LinkStateIdLength;
        }
    }
}