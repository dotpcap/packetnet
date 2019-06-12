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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// .Net analog of a ieee80211_radiotap_header from airpcap.h
    /// </summary>
    public class RadioPacket : InternetLinkLayerPacket
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
// NOTE: No need to warn about lack of use, the compiler won't
//       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioPacket" /> class.
        /// </summary>
        public RadioPacket()
        {
            Present = new uint[1];
            RadioTapFields = new SortedDictionary<RadioTapType, RadioTapField>();
            Length = (ushort) RadioFields.DefaultHeaderLength;
        }

        internal RadioPacket(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            // slice off the header portion
            Header = new ByteArraySegment(byteArraySegment)
            {
                Length = RadioFields.DefaultHeaderLength
            };

            Version = VersionBytes;
            Length = LengthBytes;

            // update the header size based on the headers packet length
            Header.Length = Length;
            Present = ReadPresentFields();
            RadioTapFields = ReadRadioTapFields();

            //Before we attempt to parse the payload we need to work out if 
            //the FCS was valid and if it will be present at the end of the frame
            var flagsField = this[RadioTapType.Flags] as FlagsRadioTapField;
            PayloadPacketOrData = new Lazy<PacketOrByteArraySegment>(() => ParseNextSegment(Header.NextSegment(), flagsField), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets the <see cref="RadioTapField" /> with the specified type, or null if the
        /// field is not in the packet.
        /// </summary>
        /// <param name='type'>
        /// Radio Tap field type
        /// </param>
        public RadioTapField this[RadioTapType type]
        {
            get
            {
                RadioTapFields.TryGetValue(type, out var field);
                return field;
            }
        }

        /// <summary>
        /// Length of the whole header in bytes, including it_version, it_pad, it_len
        /// and data fields
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// Version 0. Only increases for drastic changes, introduction of compatible
        /// new fields does not count.
        /// </summary>
        public byte Version { get; set; }

        private ushort LengthBytes
        {
            get => EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                      Header.Offset + RadioFields.LengthPosition);
            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + RadioFields.LengthPosition);
        }

        /// <summary>
        /// Returns an array of UInt32 bitmap entries. Each bit in the bitmap indicates
        /// which fields are present. Set bit 31 (0x8000000)
        /// to extend the bitmap by another 32 bits. Additional extensions are made
        /// by setting bit 31.
        /// </summary>
        private uint[] Present { get; set; }

        /// <summary>
        /// Array of radio tap fields
        /// </summary>
        private SortedDictionary<RadioTapType, RadioTapField> RadioTapFields { get; }

        private byte[] UnhandledFieldBytes { get; set; }

        private byte VersionBytes
        {
            get => Header.Bytes[Header.Offset + RadioFields.VersionPosition];
            set => Header.Bytes[Header.Offset + RadioFields.VersionPosition] = value;
        }

        private uint[] ReadPresentFields()
        {
            // make an array of the bitmask fields
            // the highest bit indicates whether other bitmask fields follow
            // the current field
            var bitmaskFields = new List<uint>();
            var bitmask = EndianBitConverter.Little.ToUInt32(Header.Bytes,
                                                             Header.Offset + RadioFields.PresentPosition);

            bitmaskFields.Add(bitmask);
            var bitmaskOffsetInBytes = 4;
            while (((bitmask >> 31) & 0x1) == 1)
            {
                // retrieve the next field
                bitmask = EndianBitConverter.Little.ToUInt32(Header.Bytes,
                                                             Header.Offset + RadioFields.PresentPosition + bitmaskOffsetInBytes);

                bitmaskFields.Add(bitmask);
                bitmaskOffsetInBytes += 4;
            }

            return bitmaskFields.ToArray();
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public override string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            var color = "";
            var colorEscape = "";

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
                var properties = new Dictionary<string, string>
                {
                    { "version", Version.ToString() },
                    { "length", Length.ToString() },
                    { "present", " (0x" + Present[0].ToString("x") + ")" }
                };

                var radioTapFields = RadioTapFields;

                foreach (var r in radioTapFields)
                {
                    properties.Add(r.Value.FieldType.ToString(),
                                   r.Value.ToString());
                }

                // calculate the padding needed to right-justify the property names
                var padLength = RandomUtils.LongestStringLength(new List<string>(properties.Keys));

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
        /// Add the specified field to the packet.
        /// </summary>
        /// <param name='field'>
        /// Field to be added
        /// </param>
        public void Add(RadioTapField field)
        {
            RadioTapFields[field.FieldType] = field;
            var presenceBit = (int) field.FieldType;
            var presenceFieldIndex = presenceBit / 32;
            if (Present.Length <= presenceFieldIndex)
            {
                var newPresentFields = new uint[presenceFieldIndex];
                Array.Copy(Present, newPresentFields, Present.Length);
                //set bit 31 to true for every present field except the last one
                for (var i = 0; i < newPresentFields.Length - 1; i++)
                {
                    newPresentFields[i] |= 0x80000000;
                }

                Present = newPresentFields;
            }

            Present[presenceFieldIndex] |= (uint) (1 << presenceBit);

            Length = UpdatePresentAndBeyond(null);
        }

        /// <summary>
        /// Removes a field of the specified type if one is present in the packet.
        /// </summary>
        /// <param name='fieldType'>
        /// Field type.
        /// </param>
        public void Remove(RadioTapType fieldType)
        {
            if (RadioTapFields.TryGetValue(fieldType, out var field))
            {
                RadioTapFields.Remove(fieldType);
                var presenceBit = (int) field.FieldType;
                var presenceField = presenceBit / 32;
                Present[presenceField] &= (uint) ~(1 << presenceBit);
                Length = UpdatePresentAndBeyond(null);
            }
        }

        /// <summary>
        /// Checks for the presence of a field of the specified type in the packet.
        /// </summary>
        /// <param name='fieldType'>
        /// The field type to check for.
        /// </param>
        /// <returns><c>true</c> if the packet contains a field of the specified type; otherwise, <c>false</c>.</returns>
        public bool Contains(RadioTapType fieldType)
        {
            return RadioTapFields.ContainsKey(fieldType);
        }

        private SortedDictionary<RadioTapType, RadioTapField> ReadRadioTapFields()
        {
            var bitmasks = Present;

            var result = new SortedDictionary<RadioTapType, RadioTapField>();

            var bitIndex = 0;

            // create a binary reader that points to the memory immediately after the bitmasks
            var offsetAfterBitmasks = Header.Offset +
                         RadioFields.PresentPosition +
                         bitmasks.Length * Marshal.SizeOf(typeof(uint));
            var remainingHeaderBytes = Length - offsetAfterBitmasks;

            var br = new BinaryReader(new MemoryStream(Header.Bytes,
                                                       offsetAfterBitmasks,
                                                       remainingHeaderBytes));

            // now go through each of the bitmask fields looking at the least significant
            // bit first to retrieve each field
            var bitmaskArray = new int[1];
            foreach (var bitmask in bitmasks)
            {
                bitmaskArray[0] = (int) bitmask;
                var ba = new BitArray(bitmaskArray);

                var unhandledFieldFound = false;

                // look at all of the bits, note we don't want to consider the
                // highest bit since that indicates another bitfield that follows
                for (var x = 0; x < 31; x++)
                {
                    if (ba[x])
                    {
                        var wasFieldAlignmentFound = RadioTapField.FieldAlignment(bitIndex, out ushort fieldAlignment);

                        if (wasFieldAlignmentFound)
                        {
                            // skip over the padding bytes to align to the field we are trying to read
                            var remainder = (offsetAfterBitmasks + br.BaseStream.Position) % fieldAlignment;
                            long toAdvance = 0;
                            if (remainder != 0)
                            {
                                toAdvance = fieldAlignment - remainder;
                            }
                            br.BaseStream.Position += toAdvance;
                        }

                        var field = RadioTapField.Parse(bitIndex, br);
                        if (field != null)
                        {
                            result[field.FieldType] = field;
                        }
                        else
                        {
                            //We have found a field that we dont handle. As we dont know how big
                            //it is we can't handle any fields after it. We will just copy
                            //the rest of the data around as a lump
                            unhandledFieldFound = true;
                            break;
                        }
                    }

                    bitIndex++;
                }

                if (unhandledFieldFound)
                {
                    break;
                }
            }

            //this will read the rest of the bytes. We pass in max value because we dont know how
            //much there is but this will ensure we get up to the end of the buffer
            UnhandledFieldBytes = br.ReadBytes(UInt16.MaxValue);

            return result;
        }

        /// <summary>
        /// Updates the present and beyond bytes in the header.
        /// </summary>
        /// <returns>The number of bytes in the <paramref name="header"/>, including the bytes prior to the
        /// present bytes that aren't written here.</returns>
        /// <param name="header">Header bytes. If null no bytes are to be written</param>
        private ushort UpdatePresentAndBeyond(ByteArraySegment header)
        {
            var offset = 0;
            if (header != null)
            {
                offset = header.Offset;
            }

            var index = RadioFields.PresentPosition;
            foreach (var presentField in Present)
            {
                if (header != null)
                {
                    EndianBitConverter.Little.CopyBytes(presentField,
                                                        header.Bytes,
                                                        offset + index);
                }

                index += RadioFields.PresentLength;
            }

            foreach (var field in RadioTapFields)
            {
                // align the offset to the field alignment by adding padding if necessary, as per the radiotap spec
                var paddingByteCount = 0;
                var remainder = (offset + index) % field.Value.Alignment;
                if (remainder != 0)
                {
                    paddingByteCount = field.Value.Alignment - remainder;
                    for (var i = 0; i < paddingByteCount; i++)
                    {
                        if (header != null)
                        {
                            header.Bytes[header.Offset + index] = 0;
                        }
                        index++;
                    }
                }

                if(header != null)
                {
                    //then copy the field data to the appropriate index
                    field.Value.CopyTo(header.Bytes, offset + index);
                }
                index += field.Value.Length;
            }

            if (UnhandledFieldBytes != null && UnhandledFieldBytes.Length > 0)
            {
                if(header != null)
                {
                    Array.Copy(UnhandledFieldBytes, 0, header.Bytes, offset + index, UnhandledFieldBytes.Length);
                }
                index += UnhandledFieldBytes.Length;
            }

            return(ushort)index;
        }

        /// <summary>
        /// Called to ensure that field values are updated before
        /// the packet bytes are retrieved
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (Header == null || Header.Length < Length)
            {
                //the backing buffer isn't big enough to accommodate the info elements so we need to resize it
                Header = new ByteArraySegment(new byte[Length]);
            }

            VersionBytes = Version;
            LengthBytes = Length;
            UpdatePresentAndBeyond(Header);
        }

        internal static PacketOrByteArraySegment ParseNextSegment(ByteArraySegment payload, FlagsRadioTapField flagsField)
        {
            var payloadPacketOrData = new PacketOrByteArraySegment();
            MacFrame frame;

            if (flagsField != null)
            {
                var fcsPresent = (flagsField.Flags & RadioTapFlags.FcsIncludedInFrame) == RadioTapFlags.FcsIncludedInFrame;

                frame = fcsPresent ? MacFrame.ParsePacketWithFcs(payload) : MacFrame.ParsePacket(payload);
            }
            else
            {
                frame = MacFrame.ParsePacket(payload);
            }

            if (frame == null)
            {
                payloadPacketOrData.ByteArraySegment = payload;
            }
            else
            {
                payloadPacketOrData.Packet = frame;
            }

            return payloadPacketOrData;
        }
    }
}