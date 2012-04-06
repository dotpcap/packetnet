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
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
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
            
            private UInt16 AuthenticationAlgorithmNumberBytes
            {
                get
                {
                    if(header.Length >= 
                       (AuthenticationFields.AuthAlgorithmNumPosition + AuthenticationFields.AuthAlgorithmNumLength))
                    {
                        return EndianBitConverter.Little.ToUInt16 (header.Bytes,
                                                                   header.Offset + AuthenticationFields.AuthAlgorithmNumPosition);
                    }
                    else
                    {
                        return 0;
                    }
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
            
            private UInt16 AuthenticationAlgorithmTransactionSequenceNumberBytes
            {
                get
                {
                    if(header.Length >= 
                       (AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition + AuthenticationFields.AuthAlgorithmTransactionSequenceNumLength))
                    {
                        return EndianBitConverter.Little.ToUInt16 (header.Bytes,
                                                                   header.Offset + AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
                    }
                    else
                    {
                        return 0;
                    }
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
            
            private AuthenticationStatusCode StatusCodeBytes
            {
                get
                {
					if(header.Length >= (AuthenticationFields.StatusCodePosition + AuthenticationFields.StatusCodeLength))
					{
						return (AuthenticationStatusCode)EndianBitConverter.Little.ToUInt16 (header.Bytes,
						                                                                     header.Offset + AuthenticationFields.StatusCodePosition);
					}
					else
					{
						//This seems the most sensible value to return when it is not possible
						//to extract a meaningful value
						return AuthenticationStatusCode.UnspecifiedFailure;
					}
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
                
				if(bas.Length > AuthenticationFields.InformationElement1Position)
				{
                	//create a segment that just refers to the info element section
                	ByteArraySegment infoElementsSegment = new ByteArraySegment (bas.Bytes,
                   		(bas.Offset + AuthenticationFields.InformationElement1Position),
                    	(bas.Length - AuthenticationFields.InformationElement1Position));
					
					InformationElements = new InformationElementList (infoElementsSegment);
				}
				else
				{
					InformationElements = new InformationElementList();
				}
				

                //cant set length until after we have handled the information elements
                //as they vary in length
                header.Length = FrameSize;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.AuthenticationFrame"/> class.
            /// </summary>
            /// <param name='SourceAddress'>
            /// Source address.
            /// </param>
            /// <param name='DestinationAddress'>
            /// Destination address.
            /// </param>
            /// <param name='BssId'>
            /// Bss identifier (MAC Address of Access Point).
            /// </param>
            /// <param name='InformationElements'>
            /// Information elements.
            /// </param>
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
                
                this.FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementAuthentication;
            }
            
            /// <summary>
            /// Writes the current packet properties to the backing ByteArraySegment.
            /// </summary>
            public override void UpdateCalculatedValues ()
            {
                if ((header == null) || (header.Length > (header.BytesLength - header.Offset)) || (header.Length < FrameSize))
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

        } 
    }
}
