namespace PacketDotNet.LSA
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Router-LSA
    /// </summary>
    public class RouterLSAFields : LSAFields
    {
        /// <summary> The length of the LinkNumber field in bytes</summary>
        public static readonly int LinkNumberLength = 2;

        /// <summary> The relative postion of the LinkNumber field</summary>
        public static readonly int LinkNumberPosition;

        /// <summary> The relative postion of the start of the RouterLink(s)</summary>
        public static readonly int RouterLinksStart;

        /// <summary> The length of the RouterOptions field in bytes</summary>
        public static readonly int RouterOptionsLength = 2;

        /// <summary> The relative postion of the RouterOptions field</summary>
        public static readonly int RouterOptionsPosition;

        static RouterLSAFields()
        {
            RouterOptionsPosition = HeaderEnd;
            LinkNumberPosition = RouterOptionsPosition + RouterOptionsLength;
            RouterLinksStart = LinkNumberPosition + LinkNumberLength;
        }
    }
}