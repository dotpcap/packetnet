#region Header

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
 * Copyright 2011 David Thedens
 */

#endregion Header
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using MiscUtil.Conversion;

using PacketDotNet.Utils;
using System.Linq;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// .Net analog of a PpiHeader.h from airpcap
        /// </summary>
        public class PpiPacket : InternetLinkLayerPacket, IEnumerable
        {
            /// <summary>
            /// PPI packet header flags.
            /// </summary>
            [Flags]
            public enum HeaderFlags : byte
            {
                /// <summary>
                /// Indicates whether or not the PPI fields are aligned to a 32 bit boundary.
                /// </summary>
                Alignment32Bit = 1
            }

            
        #region Properties

            /// <summary>
            /// Length of the whole header in bytes
            /// </summary>
            public UInt16 Length
            {
                get
                {
                    var length = PpiHeaderFields.PpiPacketHeaderLength;

                    foreach (var field in PpiFields )
                    {
                        length += PpiHeaderFields.FieldHeaderLength + field.Length;
                        if((Flags & HeaderFlags.Alignment32Bit) == HeaderFlags.Alignment32Bit)
                        {
                            length += GetDistanceTo32BitAlignment(field.Length);
                        }
                    }
                    return (UInt16)length;
                }
            }
            
            private UInt16 LengthBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16 (header.Bytes,
                                                          header.Offset + PpiHeaderFields.LengthPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes (value,
                                                    header.Bytes,
                                                    header.Offset + PpiHeaderFields.LengthPosition);
                }
            }

            

            /// <summary>
            /// Version 0. Only increases for drastic changes, introduction of compatible
            /// new fields does not count.
            /// </summary>
            public byte Version { get; set; }
            
            private byte VersionBytes
            {
                get
                {
                    return header.Bytes [header.Offset + PpiHeaderFields.VersionPosition];
                }

                set
                {
                    header.Bytes [header.Offset + PpiHeaderFields.VersionPosition] = value;
                }
            }
            
            /// <summary>
            /// Gets or sets the PPI header flags.
            /// </summary>
            /// <value>
            /// The PPI header flags.
            /// </value>
            public HeaderFlags Flags { get; set; }
            
            private HeaderFlags FlagsBytes
            {
                get
                {
                    return (HeaderFlags)header.Bytes[header.Offset + PpiHeaderFields.FlagsPosition];
                }
                
                set
                {
                    header.Bytes[header.Offset + PpiHeaderFields.FlagsPosition] = (byte) value;
                }
            }
            
            /// <summary>
            /// Gets or sets the type of the link type specified in the PPI packet. This should
            /// be the link type of the encapsulated packet.
            /// </summary>
            /// <value>
            /// The link type.
            /// </value>
            public LinkLayers LinkType { get; set; }
            
            private LinkLayers LinkTypeBytes
            {
                get
                {
                    return (LinkLayers) EndianBitConverter.Little.ToUInt32(header.Bytes,
                                                                           header.Offset + PpiHeaderFields.DataLinkTypePosition);
                }
                
                set
                {
                    EndianBitConverter.Little.CopyBytes((uint)LinkType,
                                                     header.Bytes,
                                                     header.Offset + PpiHeaderFields.DataLinkTypePosition);
                }
            }
            
            /// <summary>
            /// Returns the number of PPI fields in the PPI packet.
            /// </summary>
            /// <value>
            /// The number of fields.
            /// </value>
            public int Count { get { return PpiFields.Count; } } 
            /// <summary>
            /// Gets the <see cref="PacketDotNet.Ieee80211.PpiPacket"/> at the specified index.
            /// </summary>
            /// <param name='index'>
            /// Index.
            /// </param>
            public PpiField this[int index]
            {
                get
                {
                    return PpiFields[index];
                }
            }
            
            private List<PpiField> PpiFields { get; set; }

        #endregion Properties

        #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.PpiPacket"/> class.
            /// </summary>
            /// <param name='bas'>
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public PpiPacket (ByteArraySegment bas)
            {
                // slice off the header portion
                header = new ByteArraySegment (bas);
                
                Version = VersionBytes;
                Flags = FlagsBytes;
                
                // update the header size based on the headers packet length
                header.Length = LengthBytes;
                LinkType = LinkTypeBytes;
                PpiFields = ReadPpiFields();
    
                PpiCommon commonField = FindFirstByType(PpiFieldType.PpiCommon) as PpiCommon;
                
                // parse the encapsulated bytes
                payloadPacketOrData = ParseEncapsulatedBytes (header, commonField);
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.PpiPacket"/> class.
            /// </summary>
            public PpiPacket ()
            {
                PpiFields = new List<PpiField>();
                Version = 0;
                LinkType = LinkLayers.Ieee80211;
            }

        #endregion Constructors

        #region Public Methods
            /// <summary>
            /// Add the specified field to the packet.
            /// </summary>
            /// <param name='field'>
            /// the field.
            /// </param>
            public void Add(PpiField field)
            {
                PpiFields.Add(field);
            }
            
            /// <summary>
            /// Removes the specified field from the packet.
            /// </summary>
            /// <param name='field'>
            /// the field.
            /// </param>
            public void Remove(PpiField field)
            {
                PpiFields.Remove(field);
            }
            
            /// <summary>
            /// Removes all fields of the specified type.
            /// </summary>
            /// <param name='type'>
            /// the field type to be removed.
            /// </param>
            public void RemoveAll(PpiFieldType type)
            {
                PpiFields.RemoveAll( field => type == field.FieldType);
            }
            
            /// <summary>
            /// Checks whether the specified field is in the packet.
            /// </summary>
            /// <param name='field'>
            /// <c>true</c> if the field is in the packet, <c>false</c> if not.
            /// </param>
            public bool Contains(PpiField field)
            {
                return PpiFields.Contains(field);
            }
            
            /// <summary>
            /// Checks whether there is field of the specified type in the packet.
            /// </summary>
            /// <param name='type'>
            /// <c>true</c> if there is a field of the specified type in the packet, <c>false</c> if not.
            /// </param>
            public bool Contains(PpiFieldType type)
            {
                return (PpiFields.Find(field => field.FieldType == type) != null);
            }
            
            /// <summary>
            /// Finds the first field in the packet of the specified type.
            /// </summary>
            /// <returns>
            /// The first field in the packet of the specified type, or <c>null</c> if there is no field of the specified type.
            /// </returns>
            /// <param name='type'>
            /// The type of packet to find.
            /// </param>
            public PpiField FindFirstByType (PpiFieldType type)
            {
                var ppiFields = this.PpiFields;
                foreach (var r in ppiFields)
                {
                    if (r.FieldType == type)
                        return r;
                }
                return null;
            }
            
            /// <summary>
            /// Finds the fields in the packet of the specified type.
            /// </summary>
            /// <returns>
            /// The fields of the specified type, or an empty array of there are no fields of that type.
            /// </returns>
            /// <param name='type'>
            /// The type of packet to find.
            /// </param>
            public PpiField[] FindByType(PpiFieldType type)
            {
                return PpiFields.FindAll(p => (p.FieldType == type)).ToArray();
            }
            
            

            /// <summary>
            /// Returns the Ieee80211MacFrame inside of the Packet p or null if
            /// there is no encapsulated packet
            /// </summary>
            /// <param name="p">
            /// A <see cref="Packet"/>
            /// </param>
            /// <returns>
            /// A <see cref="MacFrame"/>
            /// </returns>
            [Obsolete("Use Packet.Extract() instead")]
            public static MacFrame GetEncapsulated (Packet p)
            {
                if (p is PpiPacket)
                {
                    var payload = p.PayloadPacket;
                    return (MacFrame)payload;
                }
                return null;
            }

            /// <summary cref="Packet.ToString(StringOutputType)" />
            public override string ToString (StringOutputType outputFormat)
            {
                var buffer = new StringBuilder ();
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
                    buffer.AppendFormat ("{0}[Ieee80211PpiPacket: Version={2}, Length={3}, {1}",
                    color,
                    colorEscape,
                    Version,
                    Length
                    );
                }

                if (outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
                {
                    // collect the properties and their value
                    var properties = new Dictionary<string, string> ();
                    properties.Add ("version", Version.ToString ());
                    properties.Add ("length", Length.ToString ());

                    var ppiField = this.PpiFields;

                    foreach (var r in ppiField)
                    {
                        properties.Add (r.FieldType.ToString (),
                                   r.ToString ());
                    }

                    // calculate the padding needed to right-justify the property names
                    int padLength = RandomUtils.LongestStringLength (new List<string> (properties.Keys));

                    // build the output string
                    buffer.AppendLine ("Ieee80211PpiPacket");
                    foreach (var property in properties)
                    {
                        buffer.AppendLine ("PPI: " + property.Key.PadLeft (padLength) + " = " + property.Value);
                    }
                    buffer.AppendLine ("PPI:");
                }

                // append the base output
                buffer.Append (base.ToString (outputFormat));

                return buffer.ToString ();
            }
            
            /// <summary>
            /// Gets the enumerator of PPI fields.
            /// </summary>
            /// <returns>
            /// The field enumerator.
            /// </returns>
            public IEnumerator GetEnumerator()
            {
                return PpiFields.GetEnumerator();
            }
   
            /// <summary>
            /// Called to ensure that field values are updated before
            /// the packet bytes are retrieved
            /// </summary>
            public override void UpdateCalculatedValues()
            {
                //If aligned is true then fields must all start on 32bit boundaries so we might need
                //to read some extra padding from the end of the header fields.
                bool aligned = ((Flags & HeaderFlags.Alignment32Bit) == HeaderFlags.Alignment32Bit);
                
                var totalFieldLength = Length;
             
                if ((header == null) || (totalFieldLength > header.Length))
                {
                    header = new ByteArraySegment (new Byte[totalFieldLength]);
                }
                
                header.Length = totalFieldLength;
                
                VersionBytes = Version;
                FlagsBytes = Flags;
                LengthBytes = (ushort)totalFieldLength;
                LinkTypeBytes = LinkType;
                
                MemoryStream ms = new MemoryStream(header.Bytes,
                                                   header.Offset + PpiHeaderFields.FirstFieldPosition,
                                                   totalFieldLength - PpiHeaderFields.FirstFieldPosition);
                BinaryWriter writer = new BinaryWriter(ms);
                foreach (var field in PpiFields)
                {
                    writer.Write((ushort) field.FieldType);
                    writer.Write((ushort) field.Length);
                    writer.Write(field.Bytes);
                    var paddingBytesRequired = GetDistanceTo32BitAlignment(field.Length);
                    if(aligned && (paddingBytesRequired > 0))
                    {
                        writer.Write(new byte[paddingBytesRequired]);
                    }
                }
            }
            
        #endregion Public Methods

        #region Private Methods
            
            /// <summary>
            /// Array of PPI fields
            /// </summary>
            private List<PpiField> ReadPpiFields()
            {
                //If aligned is true then fields must all start on 32bit boundaries so we might need
                //to read some extra padding from the end of the header fields.
                bool aligned = ((Flags & HeaderFlags.Alignment32Bit) == HeaderFlags.Alignment32Bit);
                
                var retList = new List<PpiField> ();

                // create a binary reader that points to the memory immediately after the dtl
                var offset = header.Offset + PpiHeaderFields.FirstFieldPosition;
                var br = new BinaryReader (new MemoryStream (header.Bytes,
                                                       offset,
                                                       (int)(header.Length - offset)));
                int type = 0;
                int length = PpiHeaderFields.FirstFieldPosition;
                while(length < header.Length)
                {
                    type = br.ReadUInt16 ();
                    var fieldLength = br.ReadUInt16 ();
                    //add the length plus 4 for the type and length fields
                    length +=  fieldLength + 4; 
                    retList.Add (PpiField.Parse (type, br, fieldLength));
                    var paddingByteCount = GetDistanceTo32BitAlignment(fieldLength);
                    if(aligned && (paddingByteCount > 0))
                    {
                        br.ReadBytes(paddingByteCount);
                        length += paddingByteCount;
                    }
                }

                return retList;
            }

            /// <summary>
            /// Used by the Ieee80211PpiPacket constructor. 
            /// </summary>
            /// <param name="header">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            /// <param name="commonField">
            /// The <see cref="PpiCommon"/> object in the PPI packet or null if not available
            /// </param>
            /// <returns>
            /// A <see cref="PacketOrByteArraySegment"/>
            /// </returns>
            internal static PacketOrByteArraySegment ParseEncapsulatedBytes (ByteArraySegment header, PpiCommon commonField)
            {
                // slice off the payload
                var payload = header.EncapsulatedBytes ();
                var payloadPacketOrData = new PacketOrByteArraySegment ();
                MacFrame frame = null;
                
                if (commonField != null)
                {
                    bool fcsPresent = ((commonField.Flags & PpiCommon.CommonFlags.FcsIncludedInFrame) == PpiCommon.CommonFlags.FcsIncludedInFrame);
                    
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
   
            private int GetDistanceTo32BitAlignment(int length)
            {
                return ((length % 4) == 0) ? 0 : 4 - (length % 4);
            }
            
        #endregion Private Methods
        }
    }
}