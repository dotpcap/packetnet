using System;

namespace PacketDotNet.LSA
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a AS-External-LSA
    /// </summary>
    public class ASExternalLSAFields : LSAFields
    {
        /// <summary> The relative postion of the Metric field</summary>
        public static readonly Int32 MetricPosition;

        /// <summary> The length of the NetworkMask field in bytes</summary>
        public static readonly Int32 NetworkMaskLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public static readonly Int32 NetworkMaskPosition;

        static ASExternalLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            MetricPosition = NetworkMaskPosition + NetworkMaskLength;
        }
    }
}