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
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Qos data frames are like regualr data frames except they contain a quality of service
    ///     field as defined in the 802.11e standard.
    /// </summary>
    public class QosDataFrame : DataFrame
    {
#if DEBUG
        private static readonly log4net.ILog log =
 log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        private class QosDataField
        {
            public static readonly Int32 QosControlLength = 2;

            public static readonly Int32 QosControlPosition;

            static QosDataField()
            {
                QosControlPosition = MacFields.SequenceControlPosition + MacFields.SequenceControlLength;
            }
        }

        /// <summary>
        ///     Gets or sets the qos control field.
        /// </summary>
        /// <value>
        ///     The qos control field.
        /// </value>
        public UInt16 QosControl { get; set; }

        private UInt16 QosControlBytes
        {
            get
            {
                if (this.HeaderByteArraySegment.Length >=
                    (QosDataField.QosControlPosition + QosDataField.QosControlLength))
                {
                    return EndianBitConverter.Little.ToUInt16(this.HeaderByteArraySegment.Bytes,
                        this.HeaderByteArraySegment.Offset + QosDataField.QosControlPosition);
                }

                return 0;
            }

            set => EndianBitConverter.Little.CopyBytes(value, this.HeaderByteArraySegment.Bytes,
                this.HeaderByteArraySegment.Offset + QosDataField.QosControlPosition);
        }

        /// <summary>
        ///     Length of the frame header.
        ///     This does not include the FCS, it represents only the header bytes that would
        ///     would preceed any payload.
        /// </summary>
        public override Int32 FrameSize
        {
            get
            {
                //if we are in WDS mode then there are 4 addresses (normally it is just 3)
                Int32 numOfAddressFields = (this.FrameControl.ToDS && this.FrameControl.FromDS) ? 4 : 3;

                return (MacFields.FrameControlLength +
                        MacFields.DurationIDLength +
                        (MacFields.AddressLength * numOfAddressFields) +
                        MacFields.SequenceControlLength +
                        QosDataField.QosControlLength);
            }
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.QosDataFrame" /> class.
        /// </summary>
        /// <param name='bas'>
        ///     A <see cref="ByteArraySegment" />
        /// </param>
        public QosDataFrame(ByteArraySegment bas)
        {
            log.Debug("");

            this.HeaderByteArraySegment = new ByteArraySegment(bas);

            this.FrameControl = new FrameControlField(this.FrameControlBytes);
            this.Duration = new DurationField(this.DurationBytes);
            this.SequenceControl = new SequenceControlField(this.SequenceControlBytes);
            this.QosControl = this.QosControlBytes;
            this.ReadAddresses();

            this.HeaderByteArraySegment.Length = this.FrameSize;
            var availablePayloadLength = this.GetAvailablePayloadLength();
            if (availablePayloadLength > 0)
            {
                // if data is protected we have no visibility into it, otherwise it is a LLC packet and we
                // should parse it
                if (this.FrameControl.Protected)
                {
                    this.PayloadPacketOrData.TheByteArraySegment =
                        this.HeaderByteArraySegment.EncapsulatedBytes(availablePayloadLength);
                }
                else
                {
                    this.PayloadPacketOrData.ThePacket =
                        new LogicalLinkControl(this.HeaderByteArraySegment.EncapsulatedBytes());
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.QosDataFrame" /> class.
        /// </summary>
        public QosDataFrame()
        {
            this.FrameControl = new FrameControlField();
            this.Duration = new DurationField();
            this.SequenceControl = new SequenceControlField();
            this.AssignDefaultAddresses();

            this.FrameControl.SubType = FrameControlField.FrameSubTypes.QosData;
        }

        /// <summary>
        ///     Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if ((this.HeaderByteArraySegment == null) ||
                (this.HeaderByteArraySegment.Length >
                 (this.HeaderByteArraySegment.BytesLength - this.HeaderByteArraySegment.Offset)) ||
                (this.HeaderByteArraySegment.Length < this.FrameSize))
            {
                this.HeaderByteArraySegment = new ByteArraySegment(new Byte[this.FrameSize]);
            }

            this.FrameControlBytes = this.FrameControl.Field;
            this.DurationBytes = this.Duration.Field;
            this.SequenceControlBytes = this.SequenceControl.Field;
            this.QosControlBytes = this.QosControl;
            this.WriteAddressBytes();
        }
    }
}