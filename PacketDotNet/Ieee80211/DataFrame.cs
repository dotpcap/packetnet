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
using PacketDotNet.Utils;
using PacketDotNet.Utils.Conversion;

namespace PacketDotNet.Ieee80211
    {
        /// <summary>
        /// Data frame.
        /// </summary>
        public abstract class DataFrame : MacFrame
        {
            /// <summary>
            /// SourceAddress
            /// </summary>
            public PhysicalAddress SourceAddress { get; set; }

            /// <summary>
            /// DestinationAddress
            /// </summary>
            public PhysicalAddress DestinationAddress { get; set; }

            /// <summary>
            /// ReceiverAddress
            /// </summary>
            public PhysicalAddress ReceiverAddress { get; set; }

            /// <summary>
            /// TransmitterAddress
            /// </summary>
            public PhysicalAddress TransmitterAddress { get; set; }


            /// <summary>
            /// BssID
            /// </summary>
            public PhysicalAddress BssId { get; set; }                
            
            /// <summary>
            /// Assigns the default MAC address of 00-00-00-00-00-00 to all address fields.
            /// </summary>
            protected void AssignDefaultAddresses ()
            {
                PhysicalAddress zeroAddress = PhysicalAddress.Parse ("000000000000");

                this.SourceAddress = zeroAddress;
                this.DestinationAddress = zeroAddress;
                this.TransmitterAddress = zeroAddress;
                this.ReceiverAddress = zeroAddress;
                this.BssId = zeroAddress;
            }
            
            /// <summary>
            /// Reads the addresses from the backing ByteArraySegment into the the address properties.
            /// </summary>
            /// <remarks>
            /// The <see cref="PacketDotNet.Ieee80211.FrameControlField"/> ToDS and FromDS properties dictate
            /// which of the 4 possible address fields is read into which address property.
            /// </remarks>
            protected void ReadAddresses ()
            {   
                if ((!this.FrameControl.ToDS) && (!this.FrameControl.FromDS))
                {
                    this.DestinationAddress = this.GetAddress (0);
                    this.SourceAddress = this.GetAddress (1);
                    this.BssId = this.GetAddress (2);
                }
                else if ((this.FrameControl.ToDS) && (!this.FrameControl.FromDS))
                {
                    this.BssId = this.GetAddress (0);
                    this.SourceAddress = this.GetAddress (1);
                    this.DestinationAddress = this.GetAddress (2);
                }
                else if ((!this.FrameControl.ToDS) && (this.FrameControl.FromDS))
                {
                    this.DestinationAddress = this.GetAddress (0);
                    this.BssId = this.GetAddress (1);
                    this.SourceAddress = this.GetAddress (2);
                }
                else
                {
                    //both are true so we are in WDS mode again. BSSID is not valid in this mode
                    this.ReceiverAddress = this.GetAddress (0);
                    this.TransmitterAddress = this.GetAddress (1);
                    this.DestinationAddress = this.GetAddress (2);
                    this.SourceAddress = this.GetAddress (3);
                }
            }
            
            /// <summary>
            /// Writes the address properties into the backing <see cref="ByteArraySegment"/>.
            /// </summary>
            /// <remarks>
            /// The address position into which a particular address property is written is determined by the 
            /// value of <see cref="PacketDotNet.Ieee80211.FrameControlField"/> ToDS and FromDS properties.
            /// </remarks>
            protected void WriteAddressBytes ()
            {
                if ((!this.FrameControl.ToDS) && (!this.FrameControl.FromDS))
                {
                    this.SetAddress (0, this.DestinationAddress);
                    this.SetAddress (1, this.SourceAddress);
                    this.SetAddress (2, this.BssId);
                }
                else if ((this.FrameControl.ToDS) && (!this.FrameControl.FromDS))
                {
                    this.SetAddress (0, this.BssId);
                    this.SetAddress (1, this.SourceAddress);
                    this.SetAddress (2, this.DestinationAddress);
                }
                else if ((!this.FrameControl.ToDS) && (this.FrameControl.FromDS))
                {
                    this.SetAddress (0, this.DestinationAddress);
                    this.SetAddress (1, this.BssId);
                    this.SetAddress (2, this.SourceAddress);
                }
                else
                {
                    this.SetAddress (0, this.ReceiverAddress);
                    this.SetAddress (1, this.TransmitterAddress);
                    this.SetAddress (2, this.DestinationAddress);
                    this.SetAddress (3, this.SourceAddress);
                }
            }
            
            /// <summary>
            /// Frame control bytes are the first two bytes of the frame
            /// </summary>
            protected UInt16 SequenceControlBytes
            {
                get
                {
					if(this.HeaderByteArraySegment.Length >= (MacFields.SequenceControlPosition + MacFields.SequenceControlLength))
					{
						return EndianBitConverter.Little.ToUInt16 (this.HeaderByteArraySegment.Bytes,
						                                           (this.HeaderByteArraySegment.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3)));
					}
					else
					{
						return 0;
					}
                }

                set => EndianBitConverter.Little.CopyBytes (value, this.HeaderByteArraySegment.Bytes,
                    (this.HeaderByteArraySegment.Offset + MacFields.Address1Position + (MacFields.AddressLength * 3)));
            }

            /// <summary>
            /// Sequence control field
            /// </summary>
            public SequenceControlField SequenceControl
            {
                get;
                set;
            }
            
            /// <summary>
            /// Returns a string with a description of the addresses used in the packet.
            /// This is used as a compoent of the string returned by ToString().
            /// </summary>
            /// <returns>
            /// The address string.
            /// </returns>
            protected override String GetAddressString()
            {
                String addresses = null;
                if (this.FrameControl.ToDS && this.FrameControl.FromDS)
                {
                    addresses = $"SA {this.SourceAddress} DA {this.DestinationAddress} TA {this.TransmitterAddress} RA {this.ReceiverAddress}";
                }
                else
                {
                    addresses = $"SA {this.SourceAddress} DA {this.DestinationAddress} BSSID {this.BssId}";
                }
                return addresses;
            }
        } 
    }

