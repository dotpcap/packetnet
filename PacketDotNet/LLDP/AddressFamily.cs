namespace PacketDotNet.LLDP
{
    /// <summary>
    /// The IANA (Internet Assigned Numbers Authority) Address Family
    /// </summary>
    /// <remarks>Source http://www.iana.org/assignments/address-family-numbers/</remarks>
    public enum AddressFamily
    {
        /// <summary>IP version 4</summary>
        IPv4 = 1,
        /// <summary>IP version 6</summary>
        IPv6 = 2,
        /// <summary>NSAP</summary>
        NSAP = 3,
        /// <summary>HDLC</summary>
        HDLC = 4,
        /// <summary>BBN 1822</summary>
        BBN1822 = 5,
        /// <summary>802 (includes all 802 media plus Ethernet "canonical format")</summary>
        Eth802 = 6,
        /// <summary>E.163</summary>
        E163 = 7
        // Add more if necessary
        // See remarks for more info on where
        // to find more info
    }
}