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

namespace PacketDotNet
{
    namespace Ieee80211
    {
        
        /// <summary>
        /// .Net analog of a PpiHeader.h from airpcap
        /// </summary>
        public class PpiPacket : InternetLinkLayerPacket
        {
        #region Properties

            /// <summary>
            /// Length of the whole header in bytes
            /// </summary>
            public UInt16 Length
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
            /// Array of PPI fields
            /// </summary>
            public List<PpiField> PpiFields
            {
                get
                {
                    var retList = new List<PpiField> ();

                    // create a binary reader that points to the memory immediately after the dtl
                    var offset = header.Offset + PpiHeaderFields.FirstFieldPosition;
                    var br = new BinaryReader (new MemoryStream (header.Bytes,
                                                           offset,
                                                           (int)(Length - offset)));
                    int type = 0;
                    int length = PpiHeaderFields.FirstFieldPosition;
                    do
                    {
                        type = br.ReadUInt16 ();
                        //add the length plus 4 for the type and length fields
                        length += br.ReadUInt16 () + 4; 
                        retList.Add (PpiField.Parse (type, br));
                    }
                    while (length < Length );

                    return retList;
                }
            }

            /// <summary>
            /// returns a Ieee80211FrameControlField.FrameTypes
            /// The type of 802.11 Mac packet that this PpiPacket packet encapsulates
            /// </summary>
            public virtual FrameControlField.FrameTypes Type
            {
                get
                {
                    UInt16 val = (UInt16)EndianBitConverter.Little.ToUInt16 (header.Bytes,
                                                                    header.Offset + MacFields.FrameControlPosition);
                    val = (UInt16)(((val >> 4) & 0xf) | (((val >> 2) & 0x3) << 4));
                    return (FrameControlField.FrameTypes)val;
                }
                set
                {
                    Int16 val = (Int16)value;
                    EndianBitConverter.Little.CopyBytes (val,
                                                 header.Bytes,
                                                 header.Offset + MacFields.FrameControlPosition);
                }
            }

            /// <summary>
            /// Version 0. Only increases for drastic changes, introduction of compatible
            /// new fields does not count.
            /// </summary>
            public byte Version
            {
                get
                {
                    return header.Bytes [header.Offset + PpiHeaderFields.VersionPosition];
                }

                internal set
                {
                    header.Bytes [header.Offset + PpiHeaderFields.VersionPosition] = value;
                }
            }

        #endregion Properties

        #region Constructors

            public PpiPacket (ByteArraySegment bas)
            {
                // slice off the header portion
                header = new ByteArraySegment (bas);
                
                // update the header size based on the headers packet length
                header.Length = Length;
    
                PpiCommon commonField = FindPpiField (PpiFieldType.IEEE80211_PPI_COMMON) as PpiCommon;
                
                // parse the encapsulated bytes
                payloadPacketOrData = ParseEncapsulatedBytes (header, commonField);
            }

        #endregion Constructors

        #region Public Methods

            public PpiField FindPpiField (PpiFieldType type)
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
            /// Returns the Ieee80211MacFrame inside of the Packet p or null if
            /// there is no encapsulated packet
            /// </summary>
            /// <param name="p">
            /// A <see cref="Packet"/>
            /// </param>
            /// <returns>
            /// A <see cref="MacFrame"/>
            /// </returns>
            public static MacFrame GetEncapsulated (Packet p)
            {
                if (p is PpiPacket)
                {
                    var payload = p.payloadPacketOrData.ThePacket;
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

        #endregion Public Methods

        #region Private Methods

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
                    bool fcsValid = !((commonField.Flags & PpiCommon.CommonFlags.FailedFcsCheck) == PpiCommon.CommonFlags.FailedFcsCheck);
                    bool fcsPresent = ((commonField.Flags & PpiCommon.CommonFlags.FcsIncludedInFrame) == PpiCommon.CommonFlags.FcsIncludedInFrame);
                    
                    if (fcsValid)
                    {
                        if (fcsPresent)
                        {
                            frame = MacFrame.ParsePacketWithFcs (payload);
                        }
                        else
                        {
                            frame = MacFrame.ParsePacket (payload);
                        }
                    }
                }
                else
                {
                    int fcsPosition = payload.Offset + payload.Length - MacFields.FrameCheckSequenceLength;
                    
                    UInt32 potentialFcs = EndianBitConverter.Big.ToUInt32 (payload.Bytes, fcsPosition);
                    if (MacFrame.PerformFcsCheck (payload.Bytes,
                                                  payload.Offset,
                                                  payload.Length - MacFields.FrameCheckSequenceLength,
                                                  potentialFcs))
                    {
                        //We will assume that if it passes the FCS check the last four bytes are the FCS.
                        //It is very unlikely that we would get a false positive
                        frame = MacFrame.ParsePacketWithFcs (payload);
                    }
                }
                
                // always create a MacFrame.  The MacFrame constructor will determine which type of
                // packet to build based upon the FrameControl bits.
                payloadPacketOrData.ThePacket = MacFrame.ParsePacketWithFcs (payload);
                
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

        #endregion Private Methods
        }
    }
}