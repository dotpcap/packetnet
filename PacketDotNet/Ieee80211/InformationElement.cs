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
 *  Copyright 2012 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Information element, a variable-length component of management frames
        /// </summary>
        /// <exception cref='ArgumentException'>
        /// Is thrown when an argument passed to a method is invalid.
        /// </exception>
        public class InformationElement
        {     

            public readonly static int ElementIdLength = 1;
            public readonly static int ElementLengthLength = 1;
            public readonly static int ElementIdPosition = 0;
            public readonly static int ElementLengthPosition;
            public readonly static int ElementValuePosition;

            static InformationElement ()
            {
                ElementLengthPosition = ElementIdPosition + ElementIdLength;
                ElementValuePosition = ElementLengthPosition + ElementLengthLength;
            }

            /// <summary>
            /// Types of information elements
            /// </summary>
            public enum ElementId
            {
                /// <summary>
                /// Assign an identifier to the service set
                /// </summary>
                ServiceSetIdentity = 0x00,

                /// <summary>
                /// Specifies the data rates supported by the network
                /// </summary>
                SupportedRates = 0x01,

                /// <summary>
                /// Provides the parameters necessary to join a frequency-hopping 802.11 network
                /// </summary>
                FhParamterSet = 0x02,

                /// <summary>
                /// Direct-sequence 802.11 networks have one parameter, the channel number of the network
                /// </summary>
                DsParameterSet = 0x03,

                /// <summary>
                /// Contention-free parameter. Transmitted in Becons by access points that support
                /// contention-free operation.
                /// </summary>
                CfParameterSet = 0x04,

                /// <summary>
                /// Indicates which stations have buffered traffic waiting to be picked up
                /// </summary>
                TrafficIndicationMap = 0x05,

                IbssParameterSet = 0x06,
                Country = 0x07,
                HoppingParametersPattern = 0x08,
                HoppingPatternTable = 0x09,
                Request = 0x0A,
                ChallengeText = 0x10,
                PowerContstraint = 0x20,
                PowerCapability = 0x21,
                TransmitPowerControlRequest = 0x22,
                TransmitPowerControlReport = 0x23,
                SupportedChannels = 0x24,
                ChannelSwitchAnnouncement = 0x25,
                MeasurementRequest = 0x26,
                MeasurementReport = 0x27,
                Quiet = 0x28,
                IbssDfs = 0x29,
                ErpInformation = 0x2A,
                HighThroughputCapabilities = 0x2d,
                ErpInformation2 = 0x2F,
                RobustSecurityNetwork = 0x30,
                ExtendedSupportedRates = 0x32,
                HighThroughputInformation = 0x3d,
                WifiProtectedAccess = 0xD3,
                VendorSpecific = 0xDD
            }
            
            private ByteArraySegment bytes;

            public InformationElement (ByteArraySegment bas)
            {
                bytes = bas;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.InformationElement"/> class.
            /// </summary>
            /// <param name='id'>
            /// Identifier.
            /// </param>
            /// <param name='value'>
            /// Value.
            /// </param>
            /// <exception cref='ArgumentException'>
            /// Is thrown when an argument passed to a method is invalid.
            /// </exception>
            public InformationElement(ElementId id, Byte[] value)
            {
                var ie = new Byte[ElementIdLength + ElementLengthLength + value.Length];
                bytes = new ByteArraySegment (ie);
                Id = id;
                Value = value;
            }
   
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>
            /// The identifier.
            /// </value>
            public ElementId Id
            { 
                get
                {
                    return (ElementId)bytes.Bytes [bytes.Offset + ElementIdPosition];
                }
                set
                {
                    bytes.Bytes [bytes.Offset + ElementIdPosition] = (byte)value;
                }
            }

            /// <summary>
            /// Gets the length.
            /// </summary>
            /// <value>
            /// The length.
            /// </value>
            public int ValueLength
            {
                get
                {
                    return bytes.Bytes [bytes.Offset + ElementLengthPosition];
                }
                //no set Length method as we dont want to allow a mismatch between
                //the length field and the actual length of the value
            }
            
            public byte ElementLength
            {
                get
                {
                    return (byte)(ElementIdLength + ElementLengthLength + ValueLength);
                }
                //no set Length method as we dont want to allow a mismatch between
                //the length field and the actual length of the value
            }

            public Byte[] Value
            {
                get
                {
                    var valueArray = new Byte[ValueLength];
                    Array.Copy (bytes.Bytes,
                        bytes.Offset + ElementValuePosition,
                        valueArray, 0, ValueLength);
                    return valueArray;
                }
                
                set
                {
                    if (value.Length > byte.MaxValue)
                    {
                        throw new ArgumentException ("The provided value is too long. Maximum allowed length is 255 bytes.");
                    }
                    //Decide if the current ByteArraySegement is big enough to hold the new info element
                    int newIeLength = ElementIdLength + ElementLengthLength + value.Length;
                    if (bytes.Length < newIeLength)
                    {
                        var newIe = new Byte[newIeLength];
                        newIe [ElementIdPosition] = bytes.Bytes [bytes.Offset + ElementIdPosition];
                        bytes = new ByteArraySegment (newIe);
                    }
                    
                    Array.Copy (value, 0, bytes.Bytes, bytes.Offset + ElementValuePosition, value.Length);
                    bytes.Length = newIeLength;
                    bytes.Bytes [bytes.Offset + ElementLengthPosition] = (byte)value.Length;
                    
                }
            }
            
            /// <summary>
            /// Gets the bytes.
            /// </summary>
            /// <value>
            /// The bytes.
            /// </value>
            public Byte[] Bytes
            {
                get
                {
                    return bytes.ActualBytes();
                }
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PacketDotNet.Ieee80211.InformationElement"/>.
            /// </summary>
            /// <param name='obj'>
            /// The <see cref="System.Object"/> to compare with the current <see cref="PacketDotNet.Ieee80211.InformationElement"/>.
            /// </param>
            /// <returns>
            /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
            /// <see cref="PacketDotNet.Ieee80211.InformationElement"/>; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                InformationElement ie = obj as InformationElement;
                return ((Id == ie.Id) && (Value.SequenceEqual(ie.Value)));
            }

            /// <summary>
            /// Serves as a hash function for a <see cref="PacketDotNet.Ieee80211.InformationElement"/> object.
            /// </summary>
            /// <returns>
            /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as
            /// a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                return Id.GetHashCode() ^ Value.GetHashCode();
            }
            
        } 
    }
}
