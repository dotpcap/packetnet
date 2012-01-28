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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;
using MiscUtil.Conversion;
using System.Net.NetworkInformation;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Format of an 802.11 management authentication frame.
        /// </summary>
        public class AuthenticationFrame : ManagementFrame
        {
            private class AuthenticationFields
            {
                public readonly static int AuthAlgorithmNumLength = 2;
                public readonly static int AuthAlgorithmTransactionSequenceNumLength = 2;
                public readonly static int StatusCodeLength = 2;
                public readonly static int AuthAlgorithmNumPosition;
                public readonly static int AuthAlgorithmTransactionSequenceNumPosition;
                public readonly static int StatusCodePosition;
                public readonly static int InformationElement1Position;

                static AuthenticationFields ()
                {
                    AuthAlgorithmNumPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
                    AuthAlgorithmTransactionSequenceNumPosition = AuthAlgorithmNumPosition + AuthAlgorithmNumLength;
                    StatusCodePosition = AuthAlgorithmTransactionSequenceNumPosition + AuthAlgorithmTransactionSequenceNumLength;
                    InformationElement1Position = StatusCodePosition + StatusCodeLength;
                }
            }

            /// <summary>
            /// Number used for selection of authentication algorithm
            /// </summary>
            public UInt16 AuthenticationAlgorithmNumber { get; set; }
            
            public UInt16 AuthenticationAlgorithmNumberBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16 (header.Bytes,
                        header.Offset + AuthenticationFields.AuthAlgorithmNumPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes (value,
                                                     header.Bytes,
                                                     header.Offset + AuthenticationFields.AuthAlgorithmNumPosition);
                }
            }

            /// <summary>
            /// Sequence number to define the step of the authentication algorithm
            /// </summary>
            public UInt16 AuthenticationAlgorithmTransactionSequenceNumber { get; set; }
            
            public UInt16 AuthenticationAlgorithmTransactionSequenceNumberBytes
            {
                get
                {
                    return EndianBitConverter.Little.ToUInt16 (header.Bytes,
                        header.Offset + AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes (value,
                                                     header.Bytes,
                                                     header.Offset + AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
                }
            }

            /// <summary>
            /// Indicates the success or failure of the authentication operation
            /// </summary>
            public AuthenticationStatusCode StatusCode { get; set; }
            
            public AuthenticationStatusCode StatusCodeBytes
            {
                get
                {
                    return (AuthenticationStatusCode)EndianBitConverter.Little.ToUInt16 (header.Bytes,
                        header.Offset + AuthenticationFields.StatusCodePosition);
                }
                
                set
                {
                    EndianBitConverter.Little.CopyBytes ((UInt16)value,
                        header.Bytes,
                        header.Offset + AuthenticationFields.StatusCodePosition);
                }
            }

            /// <summary>
            /// The information elements included in the frame
            /// </summary>
            public InformationElementList InformationElements { get; set; }

            /// <summary>
            /// Gets the size of the frame.
            /// </summary>
            /// <value>
            /// The size of the frame.
            /// </value>
            public override int FrameSize
            {
                get
                {
                    return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * 3) +
                        MacFields.SequenceControlLength +
                        AuthenticationFields.AuthAlgorithmNumLength +
                        AuthenticationFields.AuthAlgorithmTransactionSequenceNumLength +
                        AuthenticationFields.StatusCodeLength +
                        InformationElements.Length);
                }
            }


            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public AuthenticationFrame (ByteArraySegment bas)
            {
                header = new ByteArraySegment (bas);

                FrameControl = new FrameControlField (FrameControlBytes);
                Duration = new DurationField (DurationBytes);
                DestinationAddress = GetAddress (0);
                SourceAddress = GetAddress (1);
                BssId = GetAddress (2);
                SequenceControl = new SequenceControlField (SequenceControlBytes);
                AuthenticationAlgorithmNumber = AuthenticationAlgorithmNumberBytes;
                AuthenticationAlgorithmTransactionSequenceNumber = AuthenticationAlgorithmTransactionSequenceNumberBytes;
                
                //create a segment that just refers to the info element section
                ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                    (bas.Offset + AuthenticationFields.InformationElement1Position),
                    (bas.Length - AuthenticationFields.InformationElement1Position));

                InformationElements = new InformationElementList (infoElementsSegment);

                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
            }
            
            public AuthenticationFrame (PhysicalAddress SourceAddress,
                                        PhysicalAddress DestinationAddress,
                                        PhysicalAddress BssId,
                                        InformationElementList InformationElements)
            {
                this.FrameControl = new FrameControlField ();
                this.Duration = new DurationField ();
                this.DestinationAddress = DestinationAddress;
                this.SourceAddress = SourceAddress;
                this.BssId = BssId;
                this.SequenceControl = new SequenceControlField ();
                this.InformationElements = new InformationElementList (InformationElements);
                
                this.FrameControl.Type = FrameControlField.FrameTypes.ManagementAuthentication;
            }
            
            public override void UpdateCalculatedValues ()
            {
                if ((header == null) || (header.Length < FrameSize))
                {
                    header = new ByteArraySegment (new Byte[FrameSize]);
                }
                
                this.FrameControlBytes = this.FrameControl.Field;
                this.DurationBytes = this.Duration.Field;
                SetAddress (0, DestinationAddress);
                SetAddress (1, SourceAddress);
                SetAddress (2, BssId);
                this.SequenceControlBytes = this.SequenceControl.Field;
                this.AuthenticationAlgorithmNumberBytes = this.AuthenticationAlgorithmNumber;
                this.AuthenticationAlgorithmTransactionSequenceNumberBytes = this.AuthenticationAlgorithmTransactionSequenceNumber;
                this.StatusCodeBytes = this.StatusCode;
                //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
                this.InformationElements.CopyTo (header, header.Offset + AuthenticationFields.InformationElement1Position);
                
                header.Length = FrameSize;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString ()
            {
                return string.Format ("FrameControl {0}, FrameCheckSequence {1}, [AuthenticationFrame RA {2} TA {3} BSSID {4}]",
                                     FrameControl.ToString (),
                                     FrameCheckSequence,
                                     DestinationAddress.ToString (),
                                     SourceAddress.ToString (),
                                     BssId.ToString ());
            }

        } 
    }
}
