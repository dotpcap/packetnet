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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet
{
    ///<summary>
    /// NOTE: Might be out-of-date information since it mentions Ethereal
    /// NOTE: ethereal does NOT handle the following:
    /// IEEE80211_RADIOTAP_FHSS:
    /// IEEE80211_RADIOTAP_LOCK_QUALITY:
    /// IEEE80211_RADIOTAP_TX_ATTENUATION:
    /// IEEE80211_RADIOTAP_DB_TX_ATTENUATION:
    /// </summary>
    [Flags]
    public enum Ieee80211RadioTapType : int
    {
        /// <summary>
        /// IEEE80211_RADIOTAP_TSFT              u_int64_t       microseconds
        ///
        ///     Value in microseconds of the MAC's 64-bit 802.11 Time
        ///     Synchronization Function timer when the first bit of the
        ///     MPDU arrived at the MAC. For received frames, only.
        ///
        /// </summary>
        IEEE80211_RADIOTAP_TSFT = 0,

        /// <summary>
        /// IEEE80211_RADIOTAP_FLAGS             u_int8_t        bitmap
        ///
        ///     Properties of transmitted and received frames. See flags
        ///     defined below.
        /// </summary>
        IEEE80211_RADIOTAP_FLAGS = 1,

        /// <summary>
        /// IEEE80211_RADIOTAP_RATE              u_int8_t        500kb/s
        ///
        ///     Tx/Rx data rate
        /// </summary>
        IEEE80211_RADIOTAP_RATE = 2,

        ///<summary>
        /// IEEE80211_RADIOTAP_CHANNEL           2 x u_int16_t   MHz, bitmap
        ///
        ///     Tx/Rx frequency in MHz, followed by flags (see below).
        ///</summary>
        IEEE80211_RADIOTAP_CHANNEL = 3,

        /// <summary>
        /// IEEE80211_RADIOTAP_FHSS              u_int16_t       see below
        ///
        ///     For frequency-hopping radios, the hop set (first byte)
        ///     and pattern (second byte).
        /// </summary>
        IEEE80211_RADIOTAP_FHSS = 4,

        /// <summary>
        /// IEEE80211_RADIOTAP_DBM_ANTSIGNAL     int8_t          decibels from
        ///                                                      one milliwatt (dBm)
        ///
        ///     RF signal power at the antenna, decibel difference from
        ///     one milliwatt.
        /// </summary>
        IEEE80211_RADIOTAP_DBM_ANTSIGNAL = 5,

        /// <summary>
        /// IEEE80211_RADIOTAP_DBM_ANTNOISE      int8_t          decibels from
        ///                                                      one milliwatt (dBm)
        ///
        ///     RF noise power at the antenna, decibel difference from one
        ///     milliwatt.
        /// </summary>
        IEEE80211_RADIOTAP_DBM_ANTNOISE = 6,


        /// <summary>
        /// IEEE80211_RADIOTAP_LOCK_QUALITY     u_int16_t       unitless
        ///
        ///     Quality of Barker code lock. Unitless. Monotonically
        ///     nondecreasing with "better" lock strength. Called "Signal
        ///     Quality" in datasheets.  (Is there a standard way to measure
        ///     this?)
        /// </summary>
        IEEE80211_RADIOTAP_LOCK_QUALITY = 7,


        /// <summary>
        /// IEEE80211_RADIOTAP_TX_ATTENUATION    u_int16_t       unitless
        ///
        ///     Transmit power expressed as unitless distance from max
        ///     power set at factory calibration.  0 is max power.
        ///     Monotonically nondecreasing with lower power levels.
        /// </summary>
        IEEE80211_RADIOTAP_TX_ATTENUATION = 8,


        /// <summary>
        /// IEEE80211_RADIOTAP_DB_TX_ATTENUATION u_int16_t       decibels (dB)
        ///
        ///     Transmit power expressed as decibel distance from max power
        ///     set at factory calibration.  0 is max power.  Monotonically
        ///     nondecreasing with lower power levels.
        /// </summary>
        IEEE80211_RADIOTAP_DB_TX_ATTENUATION = 9,


        /// <summary>
        /// IEEE80211_RADIOTAP_DBM_TX_POWER      int8_t          decibels from
        ///                                                      one milliwatt (dBm)
        ///
        ///     Transmit power expressed as dBm (decibels from a 1 milliwatt
        ///     reference). This is the absolute power level measured at
        ///     the antenna port.
        /// </summary>
        IEEE80211_RADIOTAP_DBM_TX_POWER = 10,


        /// <summary>
        /// IEEE80211_RADIOTAP_ANTENNA           u_int8_t        antenna index
        ///
        ///     Unitless indication of the Rx/Tx antenna for this packet.
        ///     The first antenna is antenna 0.
        /// </summary>
        IEEE80211_RADIOTAP_ANTENNA = 11,


        /// <summary>
        /// IEEE80211_RADIOTAP_DB_ANTSIGNAL      u_int8_t        decibel (dB)
        ///
        ///     RF signal power at the antenna, decibel difference from an
        ///     arbitrary, fixed reference.
        /// </summary>
        IEEE80211_RADIOTAP_DB_ANTSIGNAL = 12,


        /// <summary>
        /// IEEE80211_RADIOTAP_DB_ANTNOISE       u_int8_t        decibel (dB)
        ///
        ///     RF noise power at the antenna, decibel difference from an
        ///     arbitrary, fixed reference point.
        /// </summary>
        IEEE80211_RADIOTAP_DB_ANTNOISE = 13,


        /// <summary>
        /// IEEE80211_RADIOTAP_FCS              u_int32_t       data
        ///
        ///      FCS from frame in network byte order.
        /// </summary>
        IEEE80211_RADIOTAP_FCS = 14,

        /// <summary>
        /// Indicates that the flags bitmaps have been extended
        /// </summary>
        IEEE80211_RADIOTAP_EXT = 31,
    };
}
