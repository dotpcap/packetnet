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
using PacketDotNet.IP;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
    {
        /// <summary>
        /// .Net analog of a ieee80211_radiotap_header from airpcap.h
        /// </summary>
        public class RadioPacket : InternetLinkLayerPacket
        {
#if DEBUG
            private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

            /// <summary>
            /// Version 0. Only increases for drastic changes, introduction of compatible
            /// new fields does not count.
            /// </summary>
            public Byte Version { get; set; }
            
            private Byte VersionBytes
            {
                get => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + RadioFields.VersionPosition];

                set => this.HeaderByteArraySegment.Bytes[this.HeaderByteArraySegment.Offset + RadioFields.VersionPosition] = value;
            }

            /// <summary>
            /// Length of the whole header in bytes, including it_version, it_pad, it_len
            /// and data fields
            /// </summary>
            public UInt16 Length {get; set;}
            
            private UInt16 LengthBytes
            {
                get => EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + RadioFields.LengthPosition);

                set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + RadioFields.LengthPosition);
            }

            /// <summary>
            /// Returns an array of UInt32 bitmap entries. Each bit in the bitmap indicates
            /// which fields are present. Set bit 31 (0x8000000)
            /// to extend the bitmap by another 32 bits. Additional extensions are made
            /// by setting bit 31.
            /// </summary>
            private UInt32[] Present {get; set;}
                
            private UInt32[] ReadPresentFields()
            {
                // make an array of the bitmask fields
                // the highest bit indicates whether other bitmask fields follow
                // the current field
                var bitmaskFields = new List<UInt32>();
                UInt32 bitmask = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + RadioFields.PresentPosition);
                bitmaskFields.Add(bitmask);
                Int32 bitmaskOffsetInBytes = 4;
                while ((bitmask & (1 << 31)) == 1)
                {
                    // retrieve the next field
                    bitmask = EndianBitConverter.Little.ToUInt32(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + RadioFields.PresentPosition + bitmaskOffsetInBytes);
                    bitmaskFields.Add(bitmask);
                    bitmaskOffsetInBytes += 4;
                }

                return bitmaskFields.ToArray();
            }
   
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.RadioPacket"/> class.
            /// </summary>
            public RadioPacket()
            {
                this.Present = new UInt32[1];
                this.RadioTapFields = new SortedDictionary<RadioTapType, RadioTapField>();
                this.Length = (UInt16)RadioFields.DefaultHeaderLength;
            }
            
            internal RadioPacket (ByteArraySegment bas)
            {
                Log.Debug ("");

            // slice off the header portion
                this.HeaderByteArraySegment = new ByteArraySegment(bas)
            {
                Length = RadioFields.DefaultHeaderLength
            };
                this.Version = this.VersionBytes;
                this.Length = this.LengthBytes;
                
                // update the header size based on the headers packet length
                this.HeaderByteArraySegment.Length = this.Length;
                this.Present = this.ReadPresentFields();
                this.RadioTapFields = this.ReadRadioTapFields();
    
                //Before we attempt to parse the payload we need to work out if 
                //the FCS was valid and if it will be present at the end of the frame
                FlagsRadioTapField flagsField = this[RadioTapType.Flags] as FlagsRadioTapField;
                this.PayloadPacketOrData = ParseEncapsulatedBytes(this.HeaderByteArraySegment.EncapsulatedBytes(), flagsField);
            }

            /// <summary cref="Packet.ToString(StringOutputType)" />
            public override String ToString(StringOutputType outputFormat)
            {
                var buffer = new StringBuilder();
                String color = "";
                String colorEscape = "";

                if (outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
                {
                    color = this.Color;
                    colorEscape = AnsiEscapeSequences.Reset;
                }

                switch (outputFormat)
                {
                    case StringOutputType.Normal:
                    case StringOutputType.Colored:
                        // build the output string
                        buffer.AppendFormat("{0}[Ieee80211RadioPacket: Version={2}, Length={3}, Present[0]=0x{4:x}]{1}",
                            color,
                            colorEscape, this.Version, this.Length, this.Present[0]);
                        break;
                    case StringOutputType.Verbose:
                    case StringOutputType.VerboseColored:
                    // collect the properties and their value
                    Dictionary<String, String> properties = new Dictionary<String, String>
                    {
                        { "version", this.Version.ToString() },
                        { "length", this.Length.ToString() },
                        { "present", " (0x" + this.Present[0].ToString("x") + ")" }
                    };

                    var radioTapFields = this.RadioTapFields;

                        foreach (var r in radioTapFields)
                        {
                            properties.Add(r.Value.FieldType.ToString(),
                                r.Value.ToString());
                        }

                        // calculate the padding needed to right-justify the property names
                        Int32 padLength = RandomUtils.LongestStringLength(new List<String>(properties.Keys));

                        // build the output string
                        buffer.AppendLine("Ieee80211RadioPacket");
                        foreach (var property in properties)
                        {
                            buffer.AppendLine("TAP: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                        }
                        buffer.AppendLine("TAP:");
                        break;
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
                this.RadioTapFields[field.FieldType] = field;
                this.Length += field.Length;
                var presenceBit = (Int32)field.FieldType;
                var presenceField = (presenceBit / 32);
                if(this.Present.Length <= presenceField)
                {
                    var newPresentFields = new UInt32[presenceField];
                    Array.Copy(this.Present, newPresentFields, this.Present.Length);
                    //set bit 31 to true for every present field except the last one
                    for(Int32 i = 0; i < newPresentFields.Length - 1; i++)
                    {
                        newPresentFields[i] |= 0x80000000;
                    }

                    this.Present = newPresentFields;
                }

                this.Present[presenceField] |= (UInt32)(1 << presenceBit);
            }
            
            /// <summary>
            /// Removes a field of the specified type if one is present in the packet.
            /// </summary>
            /// <param name='fieldType'>
            /// Field type.
            /// </param>
            public void Remove(RadioTapType fieldType)
            {
                RadioTapField field;
                if(this.RadioTapFields.TryGetValue(fieldType, out field))
                {
                    this.RadioTapFields.Remove(fieldType);
                    this.Length -= field.Length;
                    var presenceBit = (Int32)field.FieldType;
                    var presenceField = (presenceBit / 32);
                    this.Present[presenceField] &= (UInt32)~(1 << presenceBit);
                }
            }
            
            /// <summary>
            /// Checks for the presence of a field of the specified type in the packet.
            /// </summary>
            /// <param name='fieldType'>
            /// The field type to check for.
            /// </param>
            /// <returns><c>true</c> if the packet contains a field of the specified type; otherwise, <c>false</c>.</returns>
            public Boolean Contains(RadioTapType fieldType)
            {
                return this.RadioTapFields.ContainsKey(fieldType);
            }
            
            /// <summary>
            /// Gets the <see cref="PacketDotNet.Ieee80211.RadioTapField"/> with the specified type, or null if the
            /// field is not in the packet.
            /// </summary>
            /// <param name='type'>
            /// Radio Tap field type
            /// </param>
            public RadioTapField this[RadioTapType type]
            {
                get
                {
                    RadioTapField field;
                    this.RadioTapFields.TryGetValue(type, out field);
                    return field;
                }
            }
            
            /// <summary>
            /// Array of radio tap fields
            /// </summary>
            private SortedDictionary<RadioTapType, RadioTapField> RadioTapFields { get; set; }
            
            private Byte[] UnhandledFieldBytes {get; set;}
            
            private SortedDictionary<RadioTapType, RadioTapField> ReadRadioTapFields()
            {
                var bitmasks = this.Present;

                var retval = new SortedDictionary<RadioTapType, RadioTapField>();

                Int32 bitIndex = 0;

                // create a binary reader that points to the memory immediately after the bitmasks
                var offset = this.HeaderByteArraySegment.Offset +
                             RadioFields.PresentPosition +
                             (bitmasks.Length) * Marshal.SizeOf (typeof(UInt32));
                var br = new BinaryReader (new MemoryStream (this.HeaderByteArraySegment.Bytes,
                                                           offset,
                                                           (Int32)(this.Length - offset)));

                // now go through each of the bitmask fields looking at the least significant
                // bit first to retrieve each field
                foreach (var bmask in bitmasks)
                {
                    Int32[] bmaskArray = new Int32[1];
                    bmaskArray [0] = (Int32)bmask;
                    var ba = new BitArray (bmaskArray);
                    
                    Boolean unhandledFieldFound = false;

                    // look at all of the bits, note we don't want to consider the
                    // highest bit since that indicates another bitfield that follows
                    for (Int32 x = 0; x < 31; x++)
                    {
                        if (ba [x] == true)
                        {
                            var field = RadioTapField.Parse (bitIndex, br);
                            if (field != null)
                            {
                                retval[field.FieldType] = field;
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
                    
                    if(unhandledFieldFound)
                    {
                        break;
                    }
                }
                
                //this will read the rest of the bytes. We pass in max value because we dont know how
                //much there is but this will ensure we get up to the end of the buffer
                this.UnhandledFieldBytes = br.ReadBytes(UInt16.MaxValue);

                return retval;
            }
   
            /// <summary>
            /// Called to ensure that field values are updated before
            /// the packet bytes are retrieved
            /// </summary>
            public override void UpdateCalculatedValues()
            {
                if ((this.HeaderByteArraySegment == null) || (this.HeaderByteArraySegment.Length < this.Length))
                {
                    //the backing buffer isnt big enough to accommodate the info elements so we need to resize it
                    this.HeaderByteArraySegment = new ByteArraySegment (new Byte[this.Length]);
                }

                this.VersionBytes = this.Version;
                this.LengthBytes = this.Length;
                var index = RadioFields.PresentPosition;
                foreach(var presentField in this.Present)
                {
                    EndianBitConverter.Little.CopyBytes(presentField, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + index);
                    index += RadioFields.PresentLength;
                }
                
                foreach(var field in this.RadioTapFields)
                {
                    //then copy the field data to the appropriate index
                    field.Value.CopyTo(this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + index);
                    index += field.Value.Length;
                }
                
                if((this.UnhandledFieldBytes != null) && (this.UnhandledFieldBytes.Length > 0))
                {
                    Array.Copy(this.UnhandledFieldBytes, 0, this.HeaderByteArraySegment.Bytes, this.HeaderByteArraySegment.Offset + index, this.UnhandledFieldBytes.Length);
                }
            }

            internal static PacketOrByteArraySegment ParseEncapsulatedBytes (ByteArraySegment payload, FlagsRadioTapField flagsField)
            {
                var payloadPacketOrData = new PacketOrByteArraySegment ();
                MacFrame frame = null;
                
                if (flagsField != null)
                {
                    Boolean fcsPresent = ((flagsField.Flags & RadioTapFlags.FcsIncludedInFrame) == RadioTapFlags.FcsIncludedInFrame);
                    
                    if (fcsPresent)
                    {
                        frame = MacFrame.ParsePacketWithFcs (payload);
                    }
                    else
                    {
                        frame = MacFrame.ParsePacket (payload);
                    }
                }
                else
                {
                    frame = MacFrame.ParsePacket (payload);
                }
                
                if (frame == null)
                {
                    payloadPacketOrData.TheByteArraySegment = payload;
                }
                else
                {
                    payloadPacketOrData.ThePacket = frame;
                }
                
                return payloadPacketOrData;
            }
        } 
    }
