using System;

namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a ASExternalLink
    /// </summary>
    public class ASExternalLinkFields
    {
        /// <summary> The length of the TOS field in bytes</summary>
        public static readonly Int32 TOSLength = 4;
        /// <summary> The length of the ForwardingAddress field in bytes</summary>
        public static readonly Int32 ForwardingAddressLength = 4;
        /// <summary> The length of the ExternalRouteTag field in bytes</summary>
        public static readonly Int32 ExternalRouteTagLength = 4;

        /// <summary> The relative postion of the TOSPosition field</summary>
        public static readonly Int32 TOSPosition;
        /// <summary> The relative postion of the ForwardingAddress field</summary>
        public static readonly Int32 ForwardingAddressPosition;
        /// <summary> The relative postion of the ExternalRouteTag field</summary>
        public static readonly Int32 ExternalRouteTagPosition;

        static ASExternalLinkFields()
        {
            TOSPosition = 0;
            ForwardingAddressPosition  = TOSPosition + TOSLength;
            ExternalRouteTagPosition = ForwardingAddressPosition + ForwardingAddressLength;
        }
    }
}