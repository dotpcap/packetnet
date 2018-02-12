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
using System.IO;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Channel field
    /// </summary>
    public class ChannelRadioTapField : RadioTapField
    {
        /// <summary>Type of the field</summary>
        public override RadioTapType FieldType => RadioTapType.Channel;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override ushort Length => 4;

        /// <summary>
        /// Frequency in MHz
        /// </summary>
        public UInt16 FrequencyMHz { get; set; }

        /// <summary>
        /// Channel number derived from frequency
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// Channel flags
        /// </summary>
        public RadioTapChannelFlags Flags;

        /// <summary>
        /// Convert a frequency to a channel
        /// </summary>
        /// <remarks>There is some overlap between the 802.11b/g channel numbers and the 802.11a channel numbers. This means that while a particular frequncy will only
        /// ever map to single channel number the same channel number may be returned for more than one frequency. At present this affects channel numbers 8 and 12.</remarks>
        /// <param name="frequencyMHz">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public static int ChannelFromFrequencyMHz(int frequencyMHz)
        {
            switch (frequencyMHz)
            {
                //802.11 bg channel numbers
                case 2412:
                    return 1;
                case 2417:
                    return 2;
                case 2422:
                    return 3;
                case 2427:
                    return 4;
                case 2432:
                    return 5;
                case 2437:
                    return 6;
                case 2442:
                    return 7;
                case 2447:
                    return 8;
                case 2452:
                    return 9;
                case 2457:
                    return 10;
                case 2462:
                    return 11;
                case 2467:
                    return 12;
                case 2472:
                    return 13;
                case 2484:
                    return 14;
                //802.11 a channel numbers
                case 4920:
                    return 240;
                case 4940:
                    return 244;
                case 4960:
                    return 248;
                case 4980:
                    return 252;
                case 5040:
                    return 8;
                case 5060:
                    return 12;
                case 5080:
                    return 16;
                case 5170:
                    return 34;
                case 5180:
                    return 36;
                case 5190:
                    return 38;
                case 5200:
                    return 40;
                case 5210:
                    return 42;
                case 5220:
                    return 44;
                case 5230:
                    return 46;
                case 5240:
                    return 48;
                case 5260:
                    return 52;
                case 5280:
                    return 56;
                case 5300:
                    return 60;
                case 5320:
                    return 64;
                case 5500:
                    return 100;
                case 5520:
                    return 104;
                case 5540:
                    return 108;
                case 5560:
                    return 112;
                case 5580:
                    return 116;
                case 5600:
                    return 120;
                case 5620:
                    return 124;
                case 5640:
                    return 128;
                case 5660:
                    return 132;
                case 5680:
                    return 136;
                case 5700:
                    return 140;
                case 5745:
                    return 149;
                case 5765:
                    return 153;
                case 5785:
                    return 157;
                case 5805:
                    return 161;
                case 5825:
                    return 165;
                default:
                    return 0;
            }

            ;
        }

        /// <summary>
        /// Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public override void CopyTo(byte[] dest, int offset)
        {
            EndianBitConverter.Little.CopyBytes(this.FrequencyMHz, dest, offset);
            EndianBitConverter.Little.CopyBytes((UInt16) this.Flags, dest, offset + 2);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="br">
        /// A <see cref="BinaryReader"/>
        /// </param>
        public ChannelRadioTapField(BinaryReader br)
        {
            this.FrequencyMHz = br.ReadUInt16();
            this.Channel = ChannelFromFrequencyMHz(this.FrequencyMHz);
            this.Flags = (RadioTapChannelFlags) br.ReadUInt16();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.ChannelRadioTapField"/> class.
        /// </summary>
        public ChannelRadioTapField()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.ChannelRadioTapField"/> class.
        /// </summary>
        /// <param name='FrequencyMhz'>
        /// Tx/Rx Frequency in MHz.
        /// </param>
        /// <param name='Flags'>
        /// Flags.
        /// </param>
        public ChannelRadioTapField(UInt16 FrequencyMhz, RadioTapChannelFlags Flags)
        {
            this.FrequencyMHz = this.FrequencyMHz;
            this.Channel = ChannelFromFrequencyMHz(this.FrequencyMHz);
            this.Flags = Flags;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("FrequencyMHz {0}, Channel {1}, Flags {2}", this.FrequencyMHz, this.Channel, this.Flags);
        }
    }
}