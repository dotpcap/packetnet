namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Router-LSA
    /// </summary>
    public class RouterLSAFields : LSAFields
    {
        /// <summary> The length of the RouterOptions field in bytes</summary>
        public readonly static int RouterOptionsLength = 2;
        /// <summary> The length of the LinkNumber field in bytes</summary>
        public readonly static int LinkNumberLength = 2;

        /// <summary> The relative postion of the RouterOptions field</summary>
        public readonly static int RouterOptionsPosition;
        /// <summary> The relative postion of the LinkNumber field</summary>
        public readonly static int LinkNumberPosition;
        /// <summary> The relative postion of the start of the RouterLink(s)</summary>
        public readonly static int RouterLinksStart;

        static RouterLSAFields()
        {
            RouterOptionsPosition = HeaderEnd;
            LinkNumberPosition = RouterOptionsPosition + RouterOptionsLength;
            RouterLinksStart = LinkNumberPosition + LinkNumberLength;
        }
    }
}