namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// 802.11n MAC Extension flags.
    /// </summary>
    public enum PpiMacExtensionFlags : uint
    {
        /// <summary>
        /// Indicates the use of Greenfield (or HT) mode. In greenfield mode support for 802.11 a/b/g devices is sacrificed for
        /// increased efficiency.
        /// </summary>
        GreenField = 0x1,

        /// <summary>
        /// Indicates the High Throughput (HT) mode. If not set channel width is 20MHz, if set it is 40MHz.
        /// </summary>
        HtIndicator = 0x2,

        /// <summary>
        /// Indicates the use of a Short Guard Interval (SGI).
        /// </summary>
        RxSgi = 0x4,

        /// <summary>
        /// Indicates the use of HT Duplicate mode.
        /// </summary>
        DuplicateRx = 0x8,

        /// <summary>
        /// Indicates the use of MPDU aggregation.
        /// </summary>
        Aggregate = 0x10,

        /// <summary>
        /// Indicates the presence of more aggregate frames.
        /// </summary>
        MoreAggregates = 0x20,

        /// <summary>
        /// Indicates there was a CRC error in the A-MPDU delimiter after this frame.
        /// </summary>
        AggregateDelimiterCrc = 0x40
    }
}