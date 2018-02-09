namespace PacketDotNet
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a Network-LSA
    /// </summary>
    public class NetworkLSAFields : LSAFields
    {
        /// <summary> The length of the NetworkMask field in bytes</summary>
        public readonly static int NetworkMaskLength = 4;
        /// <summary> The length of the AttachedRouter field in bytes</summary>
        public readonly static int AttachedRouterLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public readonly static int NetworkMaskPosition;
        /// <summary> The relative postion of the AttachedRouter field</summary>
        public readonly static int AttachedRouterPosition;

        static NetworkLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            AttachedRouterPosition = NetworkMaskPosition + NetworkMaskLength;
        }
    }
}