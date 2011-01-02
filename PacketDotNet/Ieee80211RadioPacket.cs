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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// .Net analog of a ieee80211_radiotap_header from airpcap.h
    /// </summary>
    public class Ieee80211RadioPacket : InternetLinkLayerPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Version 0. Only increases for drastic changes, introduction of compatible
        /// new fields does not count.
        /// </summary>
        public byte Version
        {
            get
            {
                return header.Bytes[header.Offset + Ieee80211RadioFields.VersionPosition];
            }

            internal set
            {
                header.Bytes[header.Offset + Ieee80211RadioFields.VersionPosition] = value;
            }
        }

        /// <summary>
        /// Length of the whole header in bytes, including it_version, it_pad, it_len
        /// and data fields
        /// </summary>
        public UInt16 Length
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                          header.Offset + Ieee80211RadioFields.LengthPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                    header.Bytes,
                                                    header.Offset + Ieee80211RadioFields.LengthPosition);
            }
        }

        /// <summary>
        /// Returns an array of UInt32 bitmap entries. Each bit in the bitmap indicates
        /// which fields are present. Set bit 31 (0x8000000)
        /// to extend the bitmap by another 32 bits. Additional extensions are made
        /// by setting bit 31.
        /// </summary>
        public UInt32[] Present
        {
            get
            {
                // make an array of the bitmask fields
                // the highest bit indicates whether other bitmask fields follow
                // the current field
                var bitmaskFields = new List<UInt32>();
                UInt32 bitmask = EndianBitConverter.Little.ToUInt32(header.Bytes,
                                                                    header.Offset + Ieee80211RadioFields.PresentPosition);
                bitmaskFields.Add(bitmask);
                int bitmaskOffsetInBytes = 4;
                while ((bitmask & (1 << 31)) == 1)
                {
                    // retrieve the next field
                    bitmask = EndianBitConverter.Little.ToUInt32(header.Bytes,
                                                                 header.Offset + Ieee80211RadioFields.PresentPosition + bitmaskOffsetInBytes);
                    bitmaskFields.Add(bitmask);
                    bitmaskOffsetInBytes += 4;
                }

                return bitmaskFields.ToArray();
            }
        }

        internal Ieee80211RadioPacket(ByteArraySegment bas)
        {
            log.Debug("");

            // slice off the header portion
            header = new ByteArraySegment(bas);
            header.Length = Ieee80211RadioFields.DefaultHeaderLength;

            // update the header size based on the headers packet length
            header.Length = Length;

            // parse the encapsulated bytes
//            payloadPacketOrData = ParseEncapsulatedBytes(header, Type, Timeval);
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if (outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("{0}[Ieee80211RadioPacket: Version={2}, Length={3}, Present[0]=0x{4:x}]{1}",
                    color,
                    colorEscape,
                    Version,
                    Length,
                    Present[0]);
            }

            if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string, string> properties = new Dictionary<string, string>();
                properties.Add("version", Version.ToString());
                properties.Add("length", Length.ToString());
                properties.Add("present", " (0x" + Present[0].ToString("x") + ")");

                var radioTapFields = this.RadioTapFields;

                foreach (var r in radioTapFields)
                {
                    properties.Add(r.FieldType.ToString(),
                                   r.ToString());
                }

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("Ieee80211RadioPacket");
                foreach (var property in properties)
                {
                    buffer.AppendLine("TAP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }
                buffer.AppendLine("TAP:");
            }

            // append the base output
            buffer.Append(base.ToString(outputFormat));

            return buffer.ToString();
        }

        /// <summary>
        /// Array of radio tap fields
        /// </summary>
        public List<RadioTapField> RadioTapFields
        {
            get
            {
                var bitmasks = Present;

                var retval = new List<RadioTapField>();

                int bitIndex = 0;

                // create a binary reader that points to the memory immediately after the bitmasks
                var offset = header.Offset +
                             Ieee80211RadioFields.PresentPosition +
                             (bitmasks.Length) * Marshal.SizeOf(typeof(UInt32));
                var br = new BinaryReader(new MemoryStream(header.Bytes,
                                                           offset,
                                                           (int)(Length - offset)));

                // now go through each of the bitmask fields looking at the least significant
                // bit first to retrieve each field
                foreach (var bmask in bitmasks)
                {
                    int[] bmaskArray = new int[1];
                    bmaskArray[0] = (int)bmask;
                    var ba = new BitArray(bmaskArray);

                    // look at all of the bits, note we don't want to consider the
                    // highest bit since that indicates another bitfield that follows
                    for (int x = 0; x < 31; x++)
                    {
                        if (ba[x] == true)
                        {
                            retval.Add(RadioTapField.Parse(bitIndex, br));
                        }
                        bitIndex++;
                    }
                }

                return retval;
            }
        }
    }
}
