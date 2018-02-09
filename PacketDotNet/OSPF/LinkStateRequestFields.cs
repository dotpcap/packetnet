namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a LinkStateRequest
    /// </summary>
    public class LinkStateRequestFields
    {
        /// <summary> The length of the LSType field in bytes</summary>
        public readonly static int LSTypeLength = 4;
        /// <summary> The length of the LinkStateID field in bytes</summary>
        public readonly static int LinkStateIdLength = 4;
        /// <summary> The length of the AdvertisingRouter field in bytes</summary>
        public readonly static int AdvertisingRouterLength = 4;

        /// <summary> The relative postion of the LSType field</summary>
        public readonly static int LSTypePosition;
        /// <summary> The relative postion of the LinkStateID field</summary>
        public readonly static int LinkStateIdPosition;
        /// <summary> The relative postion of the AdvertisingRouter field</summary>
        public readonly static int AdvertisingRouterPosition;

        static LinkStateRequestFields()
        {
            LSTypePosition = 0;
            LinkStateIdPosition  = LSTypePosition + LSTypeLength;
            AdvertisingRouterPosition = LinkStateIdPosition + LinkStateIdLength;
        }
    }
}