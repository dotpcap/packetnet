/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
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