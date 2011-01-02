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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    /// <summary>
    /// See http://www.ucertify.com/article/ieee-802-11-frame-format.html
    /// </summary>
    public class Ieee80211FrameControlField
    {
        /// <summary>
        /// Protocol version
        /// </summary>
        public byte ProtocolVersion
        {
            get
            {
                return (byte)(Field >> 14);
            }
        }

        /// <summary>
        /// Types of frames
        /// </summary>
        public enum FrameTypes
        {
            /// <summary>
            /// Association request
            /// </summary>
            ManagementAssociationRequest = 0x00,

            /// <summary>
            /// Association response
            /// </summary>
            ManagementAssociationResponse = 0x01,

            /// <summary>
            /// Reassociation request
            /// </summary>
            ManagementReassociationRequest = 0x02,

            /// <summary>
            /// Reassociation response
            /// </summary>
            ManagementReassociationResponse = 0x03,

            /// <summary>
            /// Probe request
            /// </summary>
            ManagementProbeRequest = 0x04,

            /// <summary>
            /// Probe response
            /// </summary>
            ManagementProbeResponse = 0x5,

            /// <summary>
            /// Reserved 0
            /// </summary>
            ManagementReserved0 = 0x6,

            /// <summary>
            /// Reserved 1
            /// </summary>
            ManagementReserved1 = 0x7,

            /// <summary>
            /// Becon
            /// </summary>
            ManagementBecon = 0x8,

            /// <summary>
            /// ATIM
            /// </summary>
            ManagementATIM = 0x9,

            /// <summary>
            /// Disassociation
            /// </summary>
            ManagementDisassociation = 0xA,

            /// <summary>
            /// Authentication
            /// </summary>
            ManagementAuthentication = 0xB,

            /// <summary>
            /// Deauthentication
            /// </summary>
            ManagementDeauthentication = 0xC,

            /// <summary>
            /// Reserved 2
            /// </summary>
            ManagementReserved2 = 0xD,

            /// <summary>
            /// Reserved 3
            /// </summary>
            ManagementReserved3 = 0xE,

            /// <summary>
            /// PS poll
            /// </summary>
            ControlPSPoll = 0x1A,

            /// <summary>
            /// RTS
            /// </summary>
            ControlRTS = 0x1B,

            /// <summary>
            /// CTS
            /// </summary>
            ControlCTS = 0x1C,

            /// <summary>
            /// ACK
            /// </summary>
            ControlACK = 0x1D,

            /// <summary>
            /// CF-End
            /// </summary>
            ControlCFEnd = 0x1E,
            /// <summary>
            /// CF-End CF-Ack
            /// </summary>
            ControlCFEndCFACK = 0x1F,

            /// <summary>
            /// Data
            /// </summary>
            Data = 0x20,

            /// <summary>
            /// CF-ACK
            /// </summary>
            DataCFACK = 0x21,

            /// <summary>
            /// CF-Poll
            /// </summary>
            DataCFPoll = 0x22,

            /// <summary>
            /// CF-Ack CF-Poll
            /// </summary>
            DataCFAckCFPoll = 0x23,

            /// <summary>
            /// Null function no data
            /// </summary>
            DataNullFunctionNoData = 0x24,

            /// <summary>
            /// CF-Ack No data
            /// </summary>
            DataCFAckNoData = 0x25,

            /// <summary>
            /// CF-Poll no data
            /// </summary>
            DataCFPollNoData = 0x26,

            /// <summary>
            /// CF-Ack CF-Poll no data
            /// </summary>
            DataCFAckCFPollNoData = 0x27
        };

        /// <summary>
        /// Helps to identify the type of WLAN frame, control data and management are
        /// the various frame types defined in IEEE 802.11
        /// </summary>
        public FrameTypes Types
        {
            get
            {
                return (FrameTypes)((Field >> 8) & 0x3F);
            }
        }

        /// <summary>
        /// Is set to 1 when the frame is sent to Distribution System (DS)
        /// </summary>
        public bool ToDS
        {
            get
            {
                return (((Field >> 7) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// Is set to 1 when the frame is received from the Distribution System (DS)
        /// </summary>
        public bool FromDS
        {
            get
            {
                return (((Field >> 6) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// More Fragment is set to 1 when there are more fragments belonging to the same
        /// frame following the current fragment
        /// </summary>
        public bool MoreFragments
        {
            get
            {
                return (((Field >> 5) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// Indicates that this fragment is a retransmission of a previously transmitted fragment.
        /// (For receiver to recognize duplicate transmissions of frames)
        /// </summary>
        public bool Retry
        {
            get
            {
                return (((Field >> 4) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        ///  Indicates the power management mode that the station will be in after the transmission of the frame
        /// </summary>
        public bool PowerManagement
        {
            get
            {
                return (((Field >> 3) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// Indicates that there are more frames buffered for this station
        /// </summary>
        public bool MoreData
        {
            get
            {
                return (((Field >> 2) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// Indicates that the frame body is encrypted according to the WEP (wired equivalent privacy) algorithm
        /// </summary>
        public bool Wep
        {
            get
            {
                return (((Field >> 1) & 0x1) == 1) ? true : false;
            }
        }

        /// <summary>
        /// Bit is set when the "strict ordering" delivery method is employed. Frames and
        /// fragments are not always sent in order as it causes a transmission performance penalty.
        /// </summary>
        public bool Order
        {
            get
            {
                return ((Field & 0x1) == 1) ? true : false;
            }
        }

        private UInt16 Field;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="UInt16"/>
        /// </param>
        public Ieee80211FrameControlField(UInt16 field)
        {
            this.Field = field;
        }
    }
}
