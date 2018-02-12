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
 * Copyright 2011 David Thedens <dthedens@metageek.net>
 */

#endregion Header
using System;
using System.IO;

namespace PacketDotNet.Ieee80211
    {
        /// <summary>
        /// Contains information specific to 802.3 packets.
        /// </summary>
        public class Ppi802_3 : PpiField
        {
            /// <summary>
            /// 802.3 specific extension flags.
            /// </summary>
            [Flags]
            public enum StandardFlags : uint
            {
                /// <summary>
                /// FCS is present at the end of the packet
                /// </summary>
                FcsPresent = 1
            }
            
            /// <summary>
            /// Flags for errors detected at the time the packet was captured.
            /// </summary>
            [Flags]
            public enum ErrorFlags : uint
            {
                /// <summary>
                /// The frames FCS is invalid.
                /// </summary>
                InvalidFcs = 1,
                /// <summary>
                /// The frame has a sequence error.
                /// </summary>
                SequenceError = 2,
                /// <summary>
                /// The frame has a symbol error.
                /// </summary>
                SymbolError = 4,
                /// <summary>
                /// The frame has a data error.
                /// </summary>
                DataError = 8
            }
            
        #region Properties

            /// <summary>Type of the field</summary>
            public override PpiFieldType FieldType => PpiFieldType.Ppi802_3;

            /// <summary>
            /// Gets the length of the field data.
            /// </summary>
            /// <value>
            /// The length.
            /// </value>
            public override int Length => 8;

            /// <summary>
            /// Gets or sets the standard 802.2 flags.
            /// </summary>
            /// <value>
            /// The standard flags.
            /// </value>
            public StandardFlags Flags { get; set; }
            /// <summary>
            /// Gets or sets the 802.3 error flags.
            /// </summary>
            /// <value>
            /// The error flags.
            /// </value>
            public ErrorFlags Errors { get; set; }
            
            /// <summary>
            /// Gets the field bytes. This doesn't include the PPI field header.
            /// </summary>
            /// <value>
            /// The bytes.
            /// </value>
            public override byte[] Bytes
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.Write((uint) this.Flags);
                    writer.Write((uint) this.Errors);
                    return ms.ToArray();
                }
            }

        #endregion Properties

        #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.Ppi802_3"/> class from the 
            /// provided stream.
            /// </summary>
            /// <remarks>
            /// The position of the BinaryReader's underlying stream will be advanced to the end
            /// of the PPI field.
            /// </remarks>
            /// <param name='br'>
            /// The stream the field will be read from
            /// </param>
            public Ppi802_3(BinaryReader br)
            {
                this.Flags = (StandardFlags) br.ReadUInt32();
                this.Errors = (ErrorFlags) br.ReadUInt32();
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.Ppi802_3"/> class.
            /// </summary>
            /// <param name='Flags'>
            /// Standard Flags.
            /// </param>
            /// <param name='Errors'>
            /// Error Flags.
            /// </param>
            public Ppi802_3(StandardFlags Flags, ErrorFlags Errors)
            {
                this.Flags = Flags;
                this.Errors = Errors;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.Ppi802_3"/> class.
            /// </summary>
            public Ppi802_3()
            {
             
            }

        #endregion Constructors
        }
    }
