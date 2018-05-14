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
 * Copyright 2017 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Reflection;
using log4net;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Qos data frames are like regualr data frames except they contain a quality of service
    /// field as defined in the 802.11e standard.
    /// </summary>
    public sealed class QosDataFrame : DataFrame
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
        /// Gets or sets the qos control field.
        /// </summary>
        /// <value>
        /// The qos control field.
        /// </value>
        public UInt16 QosControl { get; set; }

        private UInt16 QosControlBytes
        {
            get
            {
                if (Header.Length >= QosDataFields.QosControlPosition + QosDataFields.QosControlLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + QosDataFields.QosControlPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + QosDataFields.QosControlPosition);
        }

        /// <summary>
        /// Length of the frame header.
        /// This does not include the FCS, it represents only the header bytes that would
        /// would preceed any payload.
        /// </summary>
        public override Int32 FrameSize
        {
            get
            {
                //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                var numOfAddressFields = FrameControl.ToDS && FrameControl.FromDS ? 4 : 3;

                return MacFields.FrameControlLength +
                       MacFields.DurationIDLength +
                       (MacFields.AddressLength * numOfAddressFields) +
                       MacFields.SequenceControlLength +
                       QosDataFields.QosControlLength;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="QosDataFrame" /> class.
        /// </summary>
        /// <param name='bas'>
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public QosDataFrame(ByteArraySegment bas)
        {
            Log.Debug("");

            Header = new ByteArraySegment(bas);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            SequenceControl = new SequenceControlField(SequenceControlBytes);
            QosControl = QosControlBytes;
            ReadAddresses();

            Header.Length = FrameSize;
            var availablePayloadLength = GetAvailablePayloadLength();
            if (availablePayloadLength > 0)
            {
                // if data is protected we have no visibility into it, otherwise it is a LLC packet and we
                // should parse it
                if (FrameControl.Protected)
                {
                    PayloadPacketOrData.Value.ByteArraySegment = Header.EncapsulatedBytes(availablePayloadLength);
                }
                else
                {
                    PayloadPacketOrData.Value.Packet = new LogicalLinkControl(Header.EncapsulatedBytes());
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QosDataFrame" /> class.
        /// </summary>
        public QosDataFrame()
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            SequenceControl = new SequenceControlField();
            AssignDefaultAddresses();

            FrameControl.SubType = FrameControlField.FrameSubTypes.QosData;
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
            SequenceControlBytes = SequenceControl.Field;
            QosControlBytes = QosControl;
            WriteAddressBytes();
        }
    }
}