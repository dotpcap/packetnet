namespace PacketDotNet.OSPF
{
    /// <summary>
    /// Represents the length (in bytes) and the relative position
    /// of the fields in a AS-External-LSA
    /// </summary>
    public class ASExternalLSAFields : LSAFields
    {
        /// <summary> The length of the NetworkMask field in bytes</summary>
        public readonly static int NetworkMaskLength = 4;

        /// <summary> The relative postion of the NetworkMask field</summary>
        public readonly static int NetworkMaskPosition;
        /// <summary> The relative postion of the Metric field</summary>
        public readonly static int MetricPosition;

        static ASExternalLSAFields()
        {
            NetworkMaskPosition = HeaderEnd;
            MetricPosition = NetworkMaskPosition + NetworkMaskLength;
        }
    }
}