using System;

namespace PacketDotNet.LSA
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Network-LSA
    /// </summary>
    public class NetworkLSAFields : LSAFields
    {
        /// <summary> The length of the AttachedRouter field in bytes</summary>
        public static readonly Int32 AttachedRouterLength = 4;

        /// <summary> The relative postion of the AttachedRouter field</summary>
        public static readonly Int32 AttachedRouterPosition;

        /// <summary> The length of the NetworkMask field in bytes</summary>
        public static readonly Int32 NetworkMaskLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public static readonly Int32 NetworkMaskPosition;

        static NetworkLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            AttachedRouterPosition = NetworkMaskPosition + NetworkMaskLength;
        }
    }
}