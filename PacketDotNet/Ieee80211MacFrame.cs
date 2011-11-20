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
    public abstract class Ieee80211MacFrame : Packet
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
        /// Duration bytes are the third and fourth bytes of the frame
        /// </summary>
        public UInt16 DurationBytes
        {
            get
            {
                return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                      header.Offset + Ieee80211MacFields.DurationIDPosition);
            }

            set
            {
                EndianBitConverter.Little.CopyBytes(value,
                                                 header.Bytes,
                                                 header.Offset + Ieee80211MacFields.DurationIDPosition);
            }
        }

        public Ieee80211DurationField Duration
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressIndex">Zero based address to look up</param>
        /// <param name="address"></param>
        protected void SetAddress(int addressIndex, PhysicalAddress address)
        {
            var offset = GetOffsetForAddress(addressIndex);
            SetAddressByOffset(offset, address);
        }

        protected void SetAddressByOffset(int offset, PhysicalAddress address)
        {
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

        protected PhysicalAddress GetAddress(int addressIndex)
        {
            var offset = GetOffsetForAddress(addressIndex);
            return GetAddressByOffset(offset);            
        }

        protected PhysicalAddress GetAddressByOffset(int offset)
        {
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
                return EndianBitConverter.Big.ToUInt32(header.Bytes,
                                                       (header.Offset + TotalPacketLength));
            }

            set
            {
                EndianBitConverter.Big.CopyBytes(value,
                                                 header.Bytes,
                                                 (header.Offset + TotalPacketLength));
            }
        }

        /// <summary>
        /// Length of the frame header.
        /// 
        /// This does not include the FCS, it represents on the header bytes that would
        /// would preceed any payload.
        /// </summary>
        public abstract int FrameSize { get; }


        public static Ieee80211MacFrame ParsePacket(ByteArraySegment bas)
        {
            //this is a bit ugly as we will end up parsing the framecontrol field twice, once here and once
            //inside the packet constructor. Could create the framecontrol and pass it to the packet but I think that is equally ugly
            Ieee80211FrameControlField frameControl = new Ieee80211FrameControlField(
                EndianBitConverter.Big.ToUInt16(bas.Bytes, bas.Offset));

            Ieee80211MacFrame macFrame = null;

            switch (frameControl.Type)
            {
                case Ieee80211FrameControlField.FrameTypes.ManagementAssociationRequest:
                    {
                        macFrame = new Ieee80211AssociationRequestFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementAssociationResponse:
                    {
                        macFrame = new Ieee80211AssociationResponseFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementReassociationRequest:
                    {
                        macFrame = new Ieee80211ReassociationRequestFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementReassociationResponse:
                    {
                        macFrame = new Ieee80211AssociationResponseFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementProbeRequest:
                    {
                        macFrame = new Ieee80211ProbeRequestFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementProbeResponse:
                    {
                        macFrame = new Ieee80211ProbeResponseFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementReserved0:
                    break; //TODO
                case Ieee80211FrameControlField.FrameTypes.ManagementReserved1:
                    break; //TODO
                case Ieee80211FrameControlField.FrameTypes.ManagementBeacon:
                    {
                        macFrame = new Ieee80211BeaconFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementATIM:
                    break; //TODO
                case Ieee80211FrameControlField.FrameTypes.ManagementDisassociation:
                    {
                        macFrame = new Ieee80211DisassociationFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementAuthentication:
                    {
                        macFrame = new Ieee80211AuthenticationFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementDeauthentication:
                    {
                        macFrame = new Ieee80211DeauthenticationFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementAction:
                    {
                        macFrame = new Ieee80211ActionFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ManagementReserved3:
                    break; //TODO
                case Ieee80211FrameControlField.FrameTypes.ControlBlockAcknowledgmentRequest:
                    {
                        macFrame = new Ieee80211BlockAcknowledgmentRequestFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ControlBlockAcknowledgment:
                    {
                        macFrame = new Ieee80211BlockAcknowledgmentFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ControlPSPoll:
                    break; //TODO
                case Ieee80211FrameControlField.FrameTypes.ControlRTS:
                    {
                        macFrame = new Ieee80211RtsFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ControlCTS:
                case Ieee80211FrameControlField.FrameTypes.ControlACK:
                    {
                        macFrame = new Ieee80211CtsOrAckFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ControlCFEnd:
                    {
                        macFrame = new Ieee80211ContentionFreeEndFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.ControlCFEndCFACK:
                    break; //TODO
                case Ieee80211FrameControlField.FrameTypes.Data:
                case Ieee80211FrameControlField.FrameTypes.DataCFACK:
                case Ieee80211FrameControlField.FrameTypes.DataCFPoll:
                case Ieee80211FrameControlField.FrameTypes.DataCFAckCFPoll:
                    {
                        macFrame = new Ieee80211DataDataFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.DataNullFunctionNoData:
                case Ieee80211FrameControlField.FrameTypes.DataCFAckNoData:
                case Ieee80211FrameControlField.FrameTypes.DataCFPollNoData:
                case Ieee80211FrameControlField.FrameTypes.DataCFAckCFPollNoData:
                    {
                        macFrame = new Ieee80211NullDataFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.QosData:
                case Ieee80211FrameControlField.FrameTypes.QosDataAndCFAck:
                case Ieee80211FrameControlField.FrameTypes.QosDataAndCFPoll:
                case Ieee80211FrameControlField.FrameTypes.QosDataAndCFAckAndCFPoll:
                    {
                        macFrame = new Ieee80211QosDataFrame(bas);
                        break;
                    }
                case Ieee80211FrameControlField.FrameTypes.QosNullData:
                case Ieee80211FrameControlField.FrameTypes.QosCFAck:
                case Ieee80211FrameControlField.FrameTypes.QosCFPoll:
                case Ieee80211FrameControlField.FrameTypes.QosCFAckAndCFPoll:
                    {
                        macFrame = new Ieee80211QosNullDataFrame(bas);
                        break;
                    }
                default:
                    //this is an unsupported (and unknown) packet type
                    break;
            }

            return macFrame;
        }
    }
}
