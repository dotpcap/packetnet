﻿/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;

namespace PacketDotNet.Ieee80211;

    /// <summary>
    /// Format of the 802.11 block acknowledgment frame.
    /// http://en.wikipedia.org/wiki/Block_acknowledgement
    /// </summary>
    public sealed class BlockAcknowledgmentFrame : MacFrame
    {
        private byte[] _blockAckBitmap;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public BlockAcknowledgmentFrame(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            ReceiverAddress = GetAddress(0);
            TransmitterAddress = GetAddress(1);
            BlockAcknowledgmentControl = new BlockAcknowledgmentControlField(BlockAckRequestControlBytes);
            BlockAckStartingSequenceControl = BlockAckStartingSequenceControlBytes;
            BlockAckBitmap = BlockAckBitmapBytes;
            Header.Length = FrameSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAcknowledgmentFrame" /> class.
        /// </summary>
        /// <param name='transmitterAddress'>
        /// Transmitter address.
        /// </param>
        /// <param name='receiverAddress'>
        /// Receiver address.
        /// </param>
        /// <param name='blockAckBitmap'>
        /// The Block ack bitmap signalling the receive status of the MSDUs.
        /// </param>
        public BlockAcknowledgmentFrame
        (
            PhysicalAddress transmitterAddress,
            PhysicalAddress receiverAddress,
            byte[] blockAckBitmap)
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            ReceiverAddress = receiverAddress;
            TransmitterAddress = transmitterAddress;
            BlockAcknowledgmentControl = new BlockAcknowledgmentControlField();
            BlockAckBitmap = blockAckBitmap;

            FrameControl.SubType = FrameControlField.FrameSubTypes.ControlBlockAcknowledgment;
        }

        /// <summary>
        /// Gets or sets the block ack bitmap used to indicate the receive status of the MPDUs.
        /// </summary>
        /// <value>
        /// The block ack bitmap.
        /// </value>
        /// <exception cref='ArgumentException'>
        /// Is thrown when the bitmap is of an incorrect length. The bitmap must be either 8 or 64 bytes longs depending on whether or not
        /// it is compressed.
        /// </exception>
        public byte[] BlockAckBitmap
        {
            get => _blockAckBitmap;
            set
            {
                if (value.Length == 8)
                {
                    BlockAcknowledgmentControl.CompressedBitmap = true;
                }
                else if (value.Length == 64)
                {
                    BlockAcknowledgmentControl.CompressedBitmap = false;
                }
                else
                {
                    throw new ArgumentException("Invalid BlockAckBitmap size. Must be either 8 or 64 bytes long.");
                }

                _blockAckBitmap = value;
            }
        }

        /// <summary>
        /// Block acknowledgment control field
        /// </summary>
        public BlockAcknowledgmentControlField BlockAcknowledgmentControl { get; set; }

        /// <summary>
        /// Gets or sets the block ack starting sequence control.
        /// </summary>
        /// <value>
        /// The block ack starting sequence control.
        /// </value>
        public ushort BlockAckStartingSequenceControl { get; set; }

        /// <summary>
        /// Length of the frame
        /// </summary>
        public override int FrameSize => MacFields.FrameControlLength +
                                         MacFields.DurationIDLength +
                                         (MacFields.AddressLength * 2) +
                                         BlockAcknowledgmentFields.BlockAckRequestControlLength +
                                         BlockAcknowledgmentFields.BlockAckStartingSequenceControlLength +
                                         GetBitmapLength();

        /// <summary>
        /// Receiver address
        /// </summary>
        public PhysicalAddress ReceiverAddress { get; set; }

        /// <summary>
        /// Transmitter address
        /// </summary>
        public PhysicalAddress TransmitterAddress { get; set; }

        private byte[] BlockAckBitmapBytes
        {
            get
            {
                var bitmap = new byte[GetBitmapLength()];
                if (Header.Length >= BlockAcknowledgmentFields.BlockAckBitmapPosition + GetBitmapLength())
                {
                    Array.Copy(Header.Bytes,
                               BlockAcknowledgmentFields.BlockAckBitmapPosition,
                               bitmap,
                               0,
                               GetBitmapLength());
                }

                return bitmap;
            }

            // ReSharper disable once ValueParameterNotUsed
            set => Array.Copy(BlockAckBitmap,
                              0,
                              Header.Bytes,
                              BlockAcknowledgmentFields.BlockAckBitmapPosition,
                              GetBitmapLength());
        }

        /// <summary>
        /// Gets or sets the block ack request control bytes.
        /// </summary>
        /// <value>
        /// The block ack request control bytes.
        /// </value>
        private ushort BlockAckRequestControlBytes
        {
            get
            {
                if (Header.Length >=
                    BlockAcknowledgmentFields.BlockAckRequestControlPosition + BlockAcknowledgmentFields.BlockAckRequestControlLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + BlockAcknowledgmentFields.BlockAckRequestControlPosition);
                }

                return 0;
            }
            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + BlockAcknowledgmentFields.BlockAckRequestControlPosition);
        }

        private ushort BlockAckStartingSequenceControlBytes
        {
            get
            {
                if (Header.Length >=
                    BlockAcknowledgmentFields.BlockAckStartingSequenceControlPosition + BlockAcknowledgmentFields.BlockAckStartingSequenceControlLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + BlockAcknowledgmentFields.BlockAckStartingSequenceControlPosition);
                }

                return 0;
            }
            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + BlockAcknowledgmentFields.BlockAckStartingSequenceControlPosition);
        }

        private int GetBitmapLength()
        {
            return BlockAcknowledgmentControl.CompressedBitmap ? 8 : 64;
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
            SetAddress(0, ReceiverAddress);
            SetAddress(1, TransmitterAddress);

            BlockAckRequestControlBytes = BlockAcknowledgmentControl.Field;
            BlockAckStartingSequenceControlBytes = BlockAckStartingSequenceControl;
            BlockAckBitmapBytes = BlockAckBitmap;

            Header.Length = FrameSize;
        }

        /// <summary>
        /// Returns a string with a description of the addresses used in the packet.
        /// This is used as a component of the string returned by ToString().
        /// </summary>
        /// <returns>
        /// The address string.
        /// </returns>
        protected override string GetAddressString()
        {
            return $"RA {ReceiverAddress} TA {TransmitterAddress}";
        }
    }