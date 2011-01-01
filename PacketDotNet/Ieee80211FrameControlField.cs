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

        public enum FrameTypes
        {
            ManagementAssociationRequest = 0x00,
            ManagementAssociationResponse = 0x01,
            ManagementReassociationRequest = 0x02,
            ManagementReassociationResponse = 0x03,
            ManagementProbeRequest = 0x04,
            ManagementProbeResponse = 0x5,
            ManagementReserved0 = 0x6,
            ManagementReserved1 = 0x7,
            ManagementBecon = 0x8,
            ManagementATIM = 0x9,
            ManagementDisassociation = 0xA,
            ManagementAuthentication = 0xB,
            ManagementDeauthentication = 0xC,
            ManagementReserved2 = 0xD,
            ManagementReserved3 = 0xE,
            ControlPSPoll = 0x1A,
            ControlRTS = 0x1B,
            ControlCTS = 0x1C,
            ControlACK = 0x1D,
            ControlCFEnd = 0x1E,
            ControlCFEndCFACK = 0x1F,
            Data = 0x20,
            DataCFACK = 0x21,
            DataCFPoll = 0x22,
            DataCFAckCFPoll = 0x23,
            DataNullFunctionNoData = 0x24,
            DataCFAckNoData = 0x25,
            DataCFPollNoData = 0x26,
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

        public bool ToDS
        {
            get
            {
                return (((Field >> 7) & 0x1) == 1) ? true : false;
            }
        }

        public bool FromDS
        {
            get
            {
                return (((Field >> 6) & 0x1) == 1) ? true : false;
            }
        }

        public bool MoreFragments
        {
            get
            {
                return (((Field >> 5) & 0x1) == 1) ? true : false;
            }
        }

        public bool Retry
        {
            get
            {
                return (((Field >> 4) & 0x1) == 1) ? true : false;
            }
        }

        public bool PowerManagement
        {
            get
            {
                return (((Field >> 3) & 0x1) == 1) ? true : false;
            }
        }

        public bool MoreData
        {
            get
            {
                return (((Field >> 2) & 0x1) == 1) ? true : false;
            }
        }

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

        public Ieee80211FrameControlField(UInt16 field)
        {
            this.Field = field;
        }
    }
}
