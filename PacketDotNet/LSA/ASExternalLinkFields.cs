namespace PacketDotNet.Lsa
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a ASExternalLink
    /// </summary>
    public struct ASExternalLinkFields
    {
        /// <summary>The length of the ExternalRouteTag field in bytes</summary>
        public static readonly int ExternalRouteTagLength = 4;

        /// <summary>The relative position of the ExternalRouteTag field</summary>
        public static readonly int ExternalRouteTagPosition;

        /// <summary>The length of the ForwardingAddress field in bytes</summary>
        public static readonly int ForwardingAddressLength = 4;

        /// <summary>The relative position of the ForwardingAddress field</summary>
        public static readonly int ForwardingAddressPosition;

        /// <summary>The length of the TOS field in bytes</summary>
        public static readonly int TOSLength = 4;

        /// <summary>The relative position of the TOSPosition field</summary>
        public static readonly int TOSPosition;

        static ASExternalLinkFields()
        {
            TOSPosition = 0;
            ForwardingAddressPosition = TOSPosition + TOSLength;
            ExternalRouteTagPosition = ForwardingAddressPosition + ForwardingAddressLength;
        }
    }
}