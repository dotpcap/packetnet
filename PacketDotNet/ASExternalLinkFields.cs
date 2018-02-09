namespace PacketDotNet
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a ASExternalLink
    /// </summary>
    public class ASExternalLinkFields
    {
        /// <summary> The length of the TOS field in bytes</summary>
        public readonly static int TOSLength = 4;
        /// <summary> The length of the ForwardingAddress field in bytes</summary>
        public readonly static int ForwardingAddressLength = 4;
        /// <summary> The length of the ExternalRouteTag field in bytes</summary>
        public readonly static int ExternalRouteTagLength = 4;

        /// <summary> The relative postion of the TOSPosition field</summary>
        public readonly static int TOSPosition;
        /// <summary> The relative postion of the ForwardingAddress field</summary>
        public readonly static int ForwardingAddressPosition;
        /// <summary> The relative postion of the ExternalRouteTag field</summary>
        public readonly static int ExternalRouteTagPosition;

        static ASExternalLinkFields()
        {
            TOSPosition = 0;
            ForwardingAddressPosition  = TOSPosition + TOSLength;
            ExternalRouteTagPosition = ForwardingAddressPosition + ForwardingAddressLength;
        }
    }
}