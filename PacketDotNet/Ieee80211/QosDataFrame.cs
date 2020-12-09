/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 * Copyright 2017 Chris Morgan <chmorgan@gmail.com>
 */

using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

#if DEBUG
using log4net;
using System.Reflection;
#endif

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
        public ushort QosControl { get; set; }

        private ushort QosControlBytes
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
        /// would proceed any payload.
        /// </summary>
        public override int FrameSize
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
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public QosDataFrame(ByteArraySegment byteArraySegment)
        {
            Log.Debug("");

            Header = new ByteArraySegment(byteArraySegment);

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
                    PayloadPacketOrData.Value.ByteArraySegment = Header.NextSegment(availablePayloadLength);
                }
                else
                {
                    PayloadPacketOrData.Value.Packet = new LogicalLinkControl(Header.NextSegment());
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
                Header = new ByteArraySegment(new byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SequenceControlBytes = SequenceControl.Field;
            QosControlBytes = QosControl;
            WriteAddressBytes();
        }
    }
}