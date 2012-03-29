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
using System.IO;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        /// <summary>
        /// Base class of all 802.11 frame types
        /// </summary>
        public abstract class MacFrame : Packet
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

                offset += MacFields.Address1Position + MacFields.AddressLength * addressIndex;

                // the 4th address is AFTER the sequence control field so we need to skip past that
                // field
                if (addressIndex == 4)
                    offset += MacFields.SequenceControlLength;

                return offset;
            }

            /// <summary>
            /// Frame control bytes are the first two bytes of the frame
            /// </summary>
            protected UInt16 FrameControlBytes
            {
                get
                {
                    if(header.Length >= (MacFields.FrameControlPosition + MacFields.FrameControlLength))
                    {
                        return EndianBitConverter.Big.ToUInt16 (header.Bytes,
                                                                header.Offset);
                    }
                    else
                    {
                       return 0;
                    }
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
            public FrameControlField FrameControl
            {
                get;
                set;
            }

            /// <summary>
            /// Duration bytes are the third and fourth bytes of the frame
            /// </summary>
            protected UInt16 DurationBytes
            {
                get
                {
                    if(header.Length >= (MacFields.DurationIDPosition + MacFields.DurationIDLength))
                    {
                        return EndianBitConverter.Little.ToUInt16(header.Bytes,
                                                                  header.Offset + MacFields.DurationIDPosition);
                    }
                    else
                    {
                        return 0;
                    }
                }

                set
                {
                    EndianBitConverter.Little.CopyBytes(value,
                                                     header.Bytes,
                                                     header.Offset + MacFields.DurationIDPosition);
                }
            }
   
            /// <summary>
            /// Gets or sets the duration value. The value represents the number of microseconds
            /// the the wireless medium is expected to remain busy.
            /// </summary>
            /// <value>
            /// The duration field value
            /// </value>
            public DurationField Duration {get; set;}

            /// <summary>
            /// Writes the address into the specified address position.
            /// </summary>
            /// <remarks>The number of valid address positions in a MAC frame is determined by
            /// the type of frame. There are between 1 and 4 address fields in MAC frames</remarks>
            /// <param name="addressIndex">Zero based address to look up</param>
            /// <param name="address"></param>
            protected void SetAddress(int addressIndex, PhysicalAddress address)
            {
                var offset = GetOffsetForAddress(addressIndex);
                SetAddressByOffset(offset, address);
            }
   
            
            /// <summary>
            /// Writes the provided address into the backing <see cref="PacketDotNet.Utils.ByteArraySegment"/>
            /// starting at the provided offset.
            /// </summary>
            /// <param name='offset'>
            /// The position where the address should start to be copied
            /// </param>
            /// <param name='address'>
            /// Address.
            /// </param>
            protected void SetAddressByOffset(int offset, PhysicalAddress address)
            {
				byte[] hwAddress = null;
				//We will replace no address with a MAC of all zer
				if(address == PhysicalAddress.None)
				{
					hwAddress = new byte[]{0, 0, 0, 0, 0, 0};
				}
				else
				{
					hwAddress = address.GetAddressBytes();
				}
				
				// using the offset, set the address
                if (hwAddress.Length != MacFields.AddressLength)
                {
                    throw new System.InvalidOperationException("address length " + hwAddress.Length
                                                               + " not equal to the expected length of "
                                                               + MacFields.AddressLength);
                }

                Array.Copy(hwAddress, 0, header.Bytes, offset,
                           hwAddress.Length);
            }
   
            /// <summary>
            /// Gets the address. There can be up to four addresses in a MacFrame depending on its type.
            /// </summary>
            /// <returns>
            /// The address.
            /// </returns>
            /// <param name='addressIndex'>
            /// Address index.
            /// </param>
            protected PhysicalAddress GetAddress(int addressIndex)
            {
                var offset = GetOffsetForAddress(addressIndex);
                return GetAddressByOffset(offset);
            }
   
            /// <summary>
            /// Gets an address by offset.
            /// </summary>
            /// <returns>
            /// The address as the specified index.
            /// </returns>
            /// <param name='offset'>
            /// The offset into the packet buffer at which to start parsing the address.
            /// </param>
            protected PhysicalAddress GetAddressByOffset(int offset)
            {
				if((header.Offset + header.Length) >= (offset + MacFields.AddressLength))
				{
        	        byte[] hwAddress = new byte[MacFields.AddressLength];
            	    Array.Copy(header.Bytes, offset,
                	           hwAddress, 0, hwAddress.Length);
                	return new PhysicalAddress(hwAddress);
				}
				else
				{
					return PhysicalAddress.None;
				}
            }

            /// <summary>
            /// Frame check sequence, the last thing in the 802.11 mac packet
            /// </summary>
            public UInt32 FrameCheckSequence { get; set; }
            
            /// <summary>
            /// Recalculates and updates the frame check sequence.
            /// </summary>
            /// <remarks>After calling this method the FCS will be valud regardless of what the packet contains.</remarks>
            public void UpdateFrameCheckSequence ()
            {
                var bytes = Bytes;
                var length = (AppendFcs) ? bytes.Length - 4 : bytes.Length;
                FrameCheckSequence = (uint) Crc32.Compute(Bytes, 0, length);
            }

            /// <summary>
            /// Length of the frame header.
            /// 
            /// This does not include the FCS, it represents only the header bytes that would
            /// would preceed any payload.
            /// </summary>
            public abstract int FrameSize { get; }
            
            /// <summary>
            /// Returns the number of bytes of payload data currently available in
            /// the buffer.
            /// </summary>
            /// <remarks>This method is used to work out how much space there is for the payload in the
            /// underlying ByteArraySegment. To find out the length of
            /// actual payload assigned to the packet use PayloadData.Length.</remarks>
            /// <value>
            /// The number of bytes of space available after the header for payload data.
            /// </value>
			protected int GetAvailablePayloadLength()
			{
				int payloadLength = header.BytesLength - (header.Offset + FrameSize);
				return (payloadLength > 0) ? payloadLength : 0;
			}
			
            /// <summary>
            /// Parses the <see cref="PacketDotNet.Utils.ByteArraySegment"/> into a MacFrame.
            /// </summary>
            /// <returns>
            /// The parsed MacFrame or null if it could not be parsed.
            /// </returns>
            /// <param name='bas'>
            /// The bytes of the packet. bas.Offset should point to the first byte in the mac frame.
            /// </param>
            /// <remarks>If the provided bytes dont contain the FCS then call <see cref="MacFrame.ParsePacket"/> instead. The presence of the 
            /// FCS is usually determined by configuration of the device used to capture the packets.</remarks>
            public static MacFrame ParsePacketWithFcs (ByteArraySegment bas)
            {
                if (bas.Length < (MacFields.FrameControlLength + MacFields.FrameCheckSequenceLength))
                {
                    //There isn't enough data for there to be an FCS and a packet
                    return null;
                }

                //remove the FCS from the buffer that we will pass to the packet parsers
                ByteArraySegment basWithoutFcs = new ByteArraySegment (bas.Bytes,
                                                                       bas.Offset,
                                                                       bas.Length - MacFields.FrameCheckSequenceLength,
                                                                       bas.BytesLength - MacFields.FrameCheckSequenceLength);
                
                UInt32 fcs = EndianBitConverter.Big.ToUInt32 (bas.Bytes,
                                                              (bas.Offset + bas.Length) - MacFields.FrameCheckSequenceLength);
                
                MacFrame frame = ParsePacket (basWithoutFcs);
                if (frame != null)
                {
                    frame.AppendFcs = true;
                    frame.FrameCheckSequence = fcs;
                }
                
                return frame;
            }
            
            /// <summary>
            /// Parses the <see cref="PacketDotNet.Utils.ByteArraySegment"/> into a MacFrame.
            /// </summary>
            /// <returns>
            /// The parsed MacFrame or null if it could not be parsed.
            /// </returns>
            /// <param name='bas'>
            /// The bytes of the packet. bas.Offset should point to the first byte in the mac frame.
            /// </param>
            /// <remarks>If the provided bytes contain the FCS then call <see cref="MacFrame.ParsePacketWithFcs"/> instead. The presence of the 
            /// FCS is usually determined by configuration of the device used to capture the packets.</remarks>
            public static MacFrame ParsePacket (ByteArraySegment bas)
            {
                if (bas.Length < MacFields.FrameControlLength)
                {
                    //there isn't enough data to even try and work out what type of packet it is
                    return null;
                }
                //this is a bit ugly as we will end up parsing the framecontrol field twice, once here and once
                //inside the packet constructor. Could create the framecontrol and pass it to the packet but I think that is equally ugly
                FrameControlField frameControl = new FrameControlField (
                    EndianBitConverter.Big.ToUInt16 (bas.Bytes, bas.Offset));

                MacFrame macFrame = null;

                switch (frameControl.SubType)
                {
                case FrameControlField.FrameSubTypes.ManagementAssociationRequest:
                    {
                        macFrame = new AssociationRequestFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementAssociationResponse:
                    {
                        macFrame = new AssociationResponseFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementReassociationRequest:
                    {
                        macFrame = new ReassociationRequestFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementReassociationResponse:
                    {
                        macFrame = new AssociationResponseFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementProbeRequest:
                    {
                        macFrame = new ProbeRequestFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementProbeResponse:
                    {
                        macFrame = new ProbeResponseFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementReserved0:
                    break; //TODO
                case FrameControlField.FrameSubTypes.ManagementReserved1:
                    break; //TODO
                case FrameControlField.FrameSubTypes.ManagementBeacon:
                    {
                        macFrame = new BeaconFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementATIM:
                    break; //TODO
                case FrameControlField.FrameSubTypes.ManagementDisassociation:
                    {
                        macFrame = new DisassociationFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementAuthentication:
                    {
                        macFrame = new AuthenticationFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementDeauthentication:
                    {
                        macFrame = new DeauthenticationFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementAction:
                    {
                        macFrame = new ActionFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ManagementReserved3:
                    break; //TODO
                case FrameControlField.FrameSubTypes.ControlBlockAcknowledgmentRequest:
                    {
                        macFrame = new BlockAcknowledgmentRequestFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ControlBlockAcknowledgment:
                    {
                        macFrame = new BlockAcknowledgmentFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ControlPSPoll:
                    break; //TODO
                case FrameControlField.FrameSubTypes.ControlRTS:
                    {
                        macFrame = new RtsFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ControlCTS:
					{
                        macFrame = new CtsFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ControlACK:
                    {
                        macFrame = new AckFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ControlCFEnd:
                    {
                        macFrame = new ContentionFreeEndFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.ControlCFEndCFACK:
                    break; //TODO
                case FrameControlField.FrameSubTypes.Data:
                case FrameControlField.FrameSubTypes.DataCFACK:
                case FrameControlField.FrameSubTypes.DataCFPoll:
                case FrameControlField.FrameSubTypes.DataCFAckCFPoll:
                    {
                        macFrame = new DataDataFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.DataNullFunctionNoData:
                case FrameControlField.FrameSubTypes.DataCFAckNoData:
                case FrameControlField.FrameSubTypes.DataCFPollNoData:
                case FrameControlField.FrameSubTypes.DataCFAckCFPollNoData:
                    {
                        macFrame = new NullDataFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.QosData:
                case FrameControlField.FrameSubTypes.QosDataAndCFAck:
                case FrameControlField.FrameSubTypes.QosDataAndCFPoll:
                case FrameControlField.FrameSubTypes.QosDataAndCFAckAndCFPoll:
                    {
                        macFrame = new QosDataFrame (bas);
                        break;
                    }
                case FrameControlField.FrameSubTypes.QosNullData:
                case FrameControlField.FrameSubTypes.QosCFAck:
                case FrameControlField.FrameSubTypes.QosCFPoll:
                case FrameControlField.FrameSubTypes.QosCFAckAndCFPoll:
                    {
                        macFrame = new QosNullDataFrame (bas);
                        break;
                    }
                default:
                        //this is an unsupported (and unknown) packet type
                    break;
                }

                return macFrame;
            }
            
            /// <summary>
            /// Calculates the FCS value for the provided bytes and compates it to the FCS value passed to the method.
            /// </summary>
            /// <returns>
            /// true if the FCS for the provided bytes matches the FCS passed in, false if not.
            /// </returns>
            /// <param name='data'>
            /// The byte array for which the FCS will be calculated.
            /// </param>
            /// <param name='offset'>
            /// The offset into data of the first byte to be covered by the FCS.
            /// </param>
            /// <param name='length'>
            /// The number of bytes to calculate the FCS for.
            /// </param>
            /// <param name='fcs'>
            /// The FCS to compare to the one calculated for the provided data.
            /// </param>
            /// <remarks>This method can be used to check the validity of a packet before attempting to parse it with either 
            /// <see cref="MacFrame.ParsePacket"/> or <see cref="MacFrame.ParsePacketWithFcs"/>. Attempting to parse a corrupted buffer
            /// using these methods could cause unexpected exceptions.</remarks>
            public static bool PerformFcsCheck (Byte[] data, int offset, int length, UInt32 fcs)
            {
                // Cast to uint for proper comparison to FrameCheckSequence
                var check = (uint)Crc32.Compute(data, offset, length);
                return check == fcs;
            }

            /// <summary>
            /// FCSs the valid.
            /// </summary>
            /// <returns>
            /// The valid.
            /// </returns>
            public bool FCSValid
            {
                get
                {
                    var packetBytes = Bytes;
                    var packetLength = (AppendFcs) ? packetBytes.Length - MacFields.FrameCheckSequenceLength : packetBytes.Length;
                    return PerformFcsCheck(packetBytes, 0, packetLength, FrameCheckSequence);
                }
            }
            
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="PacketDotNet.Ieee80211.MacFrame"/> should include an FCS at the end
            /// of the array returned by Bytes.
            /// </summary>
            /// <value>
            /// <c>true</c> if append FCS should be appended; otherwise, <c>false</c>.
            /// </value>
            public bool AppendFcs { get; set; }
            
            
            /// <value>
            /// The option to return a ByteArraySegment means that this method
            /// is higher performance as the data can start at an offset other than
            /// the first byte.
            /// </value>
            public override ByteArraySegment BytesHighPerformance
            {
                get
                {
                    log.Debug("");
    
                    // ensure calculated values are properly updated
                    RecursivelyUpdateCalculatedValues();
    
                    // if we share memory with all of our sub packets we can take a
                    // higher performance path to retrieve the bytes
                    var totalPacketLength = TotalPacketLength;
                    if(SharesMemoryWithSubPackets &&
                       ((!AppendFcs) || (header.Bytes.Length >= (header.Offset + totalPacketLength + MacFields.FrameCheckSequenceLength))))
                    {
                        var packetLength = totalPacketLength;
                        if(AppendFcs)
                        {
                            packetLength += MacFields.FrameCheckSequenceLength;
                            //We need to update the FCS field because this couldn't be done during 
                            //RecursivelyUpdateCalculatedValues because we didn't know where it would be
                            EndianBitConverter.Big.CopyBytes(FrameCheckSequence,
                                                                header.Bytes,
                                                                header.Offset + totalPacketLength);
                        }
                        
                        // The high performance path that is often taken because it is called on
                        // packets that have not had their header, or any of their sub packets, resized
                        var newByteArraySegment = new ByteArraySegment(header.Bytes,
                                                                       header.Offset,
                                                                       packetLength);
                        log.DebugFormat("SharesMemoryWithSubPackets, returning byte array {0}",
                                        newByteArraySegment.ToString());
                        return newByteArraySegment;
                    } else // need to rebuild things from scratch
                    {
                        log.Debug("rebuilding the byte array");
    
                        var ms = new MemoryStream();
    
                        // TODO: not sure if this is a performance gain or if
                        //       the compiler is smart enough to not call the get accessor for Header
                        //       twice, once when retrieving the header and again when retrieving the Length
                        var theHeader = Header;
                        ms.Write(theHeader, 0, theHeader.Length);
    
                        payloadPacketOrData.AppendToMemoryStream(ms);
                        
                        if(AppendFcs)
                        {     
                            var fcsBuffer = EndianBitConverter.Big.GetBytes(FrameCheckSequence);
                            ms.Write(fcsBuffer, 0, fcsBuffer.Length);
                        }
    
                        var newBytes = ms.ToArray();
    
                        return new ByteArraySegment(newBytes, 0, newBytes.Length);
                    }
                }
            }
            
            
            /// <summary>
            /// ToString() override
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/>
            /// </returns>
            public override String ToString()
            {
                return string.Format ("802.11 MacFrame: [{0}], {1} FCS {2}",
                                      FrameControl.ToString(),
                                      GetAddressString(),
                                      FrameCheckSequence);
            }
            
            /// <summary>
            /// Returns a string with a description of the addresses used in the packet.
            /// This is used as a compoent of the string returned by ToString().
            /// </summary>
            /// <returns>
            /// The address string.
            /// </returns>
            protected abstract String GetAddressString();
        } 
    }
}
