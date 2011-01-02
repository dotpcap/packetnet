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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

namespace PacketDotNet
{
    /// <summary>
    /// Packet class has common fields, FrameControl and Duration and
    /// a specific object class that is set based on the type of
    /// the frame
    ///
    /// See http://www.ucertify.com/article/ieee-802-11-frame-format.html
    /// </summary>
    public class Ieee80211MacFrame : Packet
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        private int GetOffsetForAddress(int addressIndex)
        {
            int offset = header.Offset;

            offset += Ieee80211MacFields.Address1Position + Ieee80211MacFields.AddressLength * addressIndex;

            // the 4th address is AFTER the sequence control field so we need to skip past that
            // field
            if (addressIndex == 4)
                offset += Ieee80211MacFields.SequenceControlLength;

            return offset;
        }

        /// <summary>
        /// Frame control bytes are the first two bytes of the frame
        /// </summary>
        public UInt16 FrameControlBytes
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes,
                                                      header.Offset);
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset);
            }
        }

        /// <summary>
        /// Frame control field
        /// </summary>
        public Ieee80211FrameControlField FrameControl
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressIndex">Zero based address to look up</param>
        /// <param name="address"></param>
        private void SetAddress(int addressIndex, PhysicalAddress address)
        {
            var offset = GetOffsetForAddress(addressIndex);

            // using the offset, set the address
            byte[] hwAddress = address.GetAddressBytes();
            if (hwAddress.Length != Ieee80211MacFields.AddressLength)
            {
                throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                           + " not equal to the expected length of "
                                                           + Ieee80211MacFields.AddressLength);
            }

            Array.Copy(hwAddress, 0, header.Bytes, offset,
                       hwAddress.Length);
        }

        private PhysicalAddress GetAddress(int addressIndex)
        {
            var offset = GetOffsetForAddress(addressIndex);

            byte[] hwAddress = new byte[Ieee80211MacFields.AddressLength];
            Array.Copy(header.Bytes, offset,
                       hwAddress, 0, hwAddress.Length);
            return new PhysicalAddress(hwAddress);
        }

        /// <summary>
        /// Frame check sequence, the last thing in the 802.11 mac packet
        /// </summary>
        public UInt32 FrameCheckSequence
        {
            get
            {
                var offsetToEndOfData = payloadPacketOrData.TheByteArraySegment.Offset + payloadPacketOrData.TheByteArraySegment.Length;
                return EndianBitConverter.Big.ToUInt32(header.Bytes,
                                                       offsetToEndOfData);
            }

            set
            {
                var offsetToEndOfData = payloadPacketOrData.TheByteArraySegment.Offset + payloadPacketOrData.TheByteArraySegment.Length;
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 offsetToEndOfData);
            }
        }

        /// <summary>
        /// Interfaces for all inner frames
        /// </summary>
        public interface InnerFramePacket
        {
            /// <summary>
            /// Length of the frame
            /// </summary>
            int FrameSize { get; }
        }

        /// <summary>
        /// RTS Frame has a ReceiverAddress[6], TransmitterAddress[6] and a FrameCheckSequence[4],
        /// these fields follow the common FrameControl[2] and DurationId[2] fields
        /// </summary>
        public class RTSFrame : Packet, InnerFramePacket
        {
            /// <summary>
            /// ReceiverAddress
            /// </summary>
            public PhysicalAddress ReceiverAddress
            {
                get
                {
                    return parent.GetAddress(0);
                }

                set
                {
                    parent.SetAddress(0, value);
                }
            }

            /// <summary>
            /// TransmitterAddress
            /// </summary>
            public PhysicalAddress TransmitterAddress
            {
                get
                {
                    return parent.GetAddress(1);
                }

                set
                {
                    parent.SetAddress(1, value);
                }
            }

            /// <summary>
            /// Length of the frame
            /// </summary>
            public int FrameSize
            {
                get
                {
                    return Ieee80211MacFields.AddressLength * 2;
                }
            }

            private Ieee80211MacFrame parent;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parent">
            /// A <see cref="Ieee80211MacFrame"/>
            /// </param>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public RTSFrame(Ieee80211MacFrame parent, ByteArraySegment bas)
            {
                this.parent = parent;
                header = new ByteArraySegment(bas);
                header.Length = FrameSize;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("[RTSFrame RA {0}, TA {1}]",
                                     ReceiverAddress.ToString(),
                                     TransmitterAddress.ToString());
            }
        }

        /// <summary>
        /// Format of a CTS or an ACK frame
        /// </summary>
        public class CTSOrACKFrame : Packet, InnerFramePacket
        {
            /// <summary>
            /// Receiver address
            /// </summary>
            public PhysicalAddress ReceiverAddress
            {
                get
                {
                    return parent.GetAddress(0);
                }

                set
                {
                    parent.SetAddress(0, value);
                }
            }

            /// <summary>
            /// Length of the frame
            /// </summary>
            public int FrameSize
            {
                get
                {
                    return Ieee80211MacFields.AddressLength;
                }
            }

            private Ieee80211MacFrame parent;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parent">
            /// A <see cref="Ieee80211MacFrame"/>
            /// </param>
            /// <param name="bas">
            /// A <see cref="ByteArraySegment"/>
            /// </param>
            public CTSOrACKFrame(Ieee80211MacFrame parent, ByteArraySegment bas)
            {
                this.parent = parent;
                header = new ByteArraySegment(bas);
                header.Length = FrameSize;
            }

            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override string ToString()
            {
                return string.Format("[CTSOrACKFrame RA {0}]",
                                     ReceiverAddress.ToString());
            }
        }

        /// <summary>
        /// One of RTS, CTS etc frames
        /// </summary>
        public InnerFramePacket InnerFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bas">
        /// A <see cref="ByteArraySegment"/>
        /// </param>
        public Ieee80211MacFrame(ByteArraySegment bas)
        {
            log.Debug("");

            // slice off the header portion as our header
            header = new ByteArraySegment(bas);
            const int defaultLength = 4;
            header.Length = defaultLength;

            FrameControl = new Ieee80211FrameControlField(FrameControlBytes);

            // determine what kind of frame this is based on the type
            if (FrameControl.Types == Ieee80211FrameControlField.FrameTypes.ControlRTS)
            {
                InnerFrame = new RTSFrame(this, bas);
            }
            else if (FrameControl.Types == Ieee80211FrameControlField.FrameTypes.ControlCTS)
            {
                InnerFrame = new CTSOrACKFrame(this, bas);
            }
            else if (FrameControl.Types == Ieee80211FrameControlField.FrameTypes.ControlACK)
            {
                InnerFrame = new CTSOrACKFrame(this, bas);
            }
            else
            {
                throw new System.NotImplementedException("FrameControl.Types of " + FrameControl.Types + " not handled");
            }

            header.Length = InnerFrame.FrameSize;

            // store the payload, less the frame check sequence at the end
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes();
            payloadPacketOrData.TheByteArraySegment.Length -= Ieee80211MacFields.FrameCheckSequenceLength;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("FrameControl {0}, InnerFrame {1}, FrameCheckSequence {2}",
                                 FrameControl.ToString(),
                                 InnerFrame.ToString(),
                                 FrameCheckSequence);
        }
    }
}
