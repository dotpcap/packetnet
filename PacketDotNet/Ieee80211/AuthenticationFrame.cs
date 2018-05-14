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
using System.Net.NetworkInformation;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Format of an 802.11 management authentication frame.
    /// </summary>
    public sealed class AuthenticationFrame : ManagementFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public AuthenticationFrame(ByteArraySegment bas)
        {
            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            DestinationAddress = GetAddress(0);
            SourceAddress = GetAddress(1);
            BssId = GetAddress(2);
            SequenceControl = new SequenceControlField(SequenceControlBytes);
            AuthenticationAlgorithmNumber = AuthenticationAlgorithmNumberBytes;
            AuthenticationAlgorithmTransactionSequenceNumber = AuthenticationAlgorithmTransactionSequenceNumberBytes;

            if (bas.Length > AuthenticationFields.InformationElement1Position)
            {
                //create a segment that just refers to the info element section
                var infoElementsSegment = new ByteArraySegment(bas.Bytes,
                                                               bas.Offset + AuthenticationFields.InformationElement1Position,
                                                               bas.Length - AuthenticationFields.InformationElement1Position);

                InformationElements = new InformationElementList(infoElementsSegment);
            }
            else
            {
                InformationElements = new InformationElementList();
            }


            //cant set length until after we have handled the information elements
            //as they vary in length
            Header.Length = FrameSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFrame" /> class.
        /// </summary>
        /// <param name='sourceAddress'>
        /// Source address.
        /// </param>
        /// <param name='destinationAddress'>
        /// Destination address.
        /// </param>
        /// <param name='bssId'>
        /// Bss identifier (MAC Address of Access Point).
        /// </param>
        /// <param name='informationElements'>
        /// Information elements.
        /// </param>
        public AuthenticationFrame
        (
            PhysicalAddress sourceAddress,
            PhysicalAddress destinationAddress,
            PhysicalAddress bssId,
            InformationElementList informationElements)
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            DestinationAddress = destinationAddress;
            SourceAddress = sourceAddress;
            BssId = bssId;
            SequenceControl = new SequenceControlField();
            InformationElements = new InformationElementList(informationElements);

            FrameControl.SubType = FrameControlField.FrameSubTypes.ManagementAuthentication;
        }

        /// <summary>
        /// Number used for selection of authentication algorithm
        /// </summary>
        public UInt16 AuthenticationAlgorithmNumber { get; set; }

        /// <summary>
        /// Sequence number to define the step of the authentication algorithm
        /// </summary>
        public UInt16 AuthenticationAlgorithmTransactionSequenceNumber { get; set; }

        /// <summary>
        /// Gets the size of the frame.
        /// </summary>
        /// <value>
        /// The size of the frame.
        /// </value>
        public override Int32 FrameSize => MacFields.FrameControlLength +
                                           MacFields.DurationIDLength +
                                           (MacFields.AddressLength * 3) +
                                           MacFields.SequenceControlLength +
                                           AuthenticationFields.AuthAlgorithmNumLength +
                                           AuthenticationFields.AuthAlgorithmTransactionSequenceNumLength +
                                           AuthenticationFields.StatusCodeLength +
                                           InformationElements.Length;

        /// <summary>
        /// The information elements included in the frame
        /// </summary>
        public InformationElementList InformationElements { get; set; }

        /// <summary>
        /// Indicates the success or failure of the authentication operation
        /// </summary>
        public AuthenticationStatusCode StatusCode { get; set; }

        private UInt16 AuthenticationAlgorithmNumberBytes
        {
            get
            {
                if (Header.Length >=
                    AuthenticationFields.AuthAlgorithmNumPosition + AuthenticationFields.AuthAlgorithmNumLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + AuthenticationFields.AuthAlgorithmNumPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + AuthenticationFields.AuthAlgorithmNumPosition);
        }

        private UInt16 AuthenticationAlgorithmTransactionSequenceNumberBytes
        {
            get
            {
                if (Header.Length >=
                    AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition + AuthenticationFields.AuthAlgorithmTransactionSequenceNumLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + AuthenticationFields.AuthAlgorithmTransactionSequenceNumPosition);
        }

        private AuthenticationStatusCode StatusCodeBytes
        {
            set => EndianBitConverter.Little.CopyBytes((UInt16) value,
                                                       Header.Bytes,
                                                       Header.Offset + AuthenticationFields.StatusCodePosition);
        }

        /// <summary>
        /// Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (Header == null || Header.Length > Header.BytesLength - Header.Offset || Header.Length < FrameSize)
            {
                Header = new ByteArraySegment(new Byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SetAddress(0, DestinationAddress);
            SetAddress(1, SourceAddress);
            SetAddress(2, BssId);
            SequenceControlBytes = SequenceControl.Field;
            AuthenticationAlgorithmNumberBytes = AuthenticationAlgorithmNumber;
            AuthenticationAlgorithmTransactionSequenceNumberBytes = AuthenticationAlgorithmTransactionSequenceNumber;
            StatusCodeBytes = StatusCode;
            //we now know the backing buffer is big enough to contain the info elements so we can safely copy them in
            InformationElements.CopyTo(Header, Header.Offset + AuthenticationFields.InformationElement1Position);

            Header.Length = FrameSize;
        }
    }
}