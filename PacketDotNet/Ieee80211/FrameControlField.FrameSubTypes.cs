/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System.Diagnostics.CodeAnalysis;

namespace PacketDotNet.Ieee80211
{
    public partial class FrameControlField
    {
        /// <summary>
        /// Specifies the frame types down to the sub type level.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum FrameSubTypes
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
            /// Beacon
            /// </summary>
            ManagementBeacon = 0x8,

            /// <summary>
            /// ATIM
            /// </summary>
            ManagementAtim = 0x9,

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
            ManagementAction = 0xD,

            /// <summary>
            /// Reserved 3
            /// </summary>
            ManagementReserved3 = 0xE,

            /// <summary>
            /// Block Acknowledgment Request (QOS)
            /// </summary>
            ControlBlockAcknowledgmentRequest = 0x18,

            /// <summary>
            /// Block Acknowledgment (QOS)
            /// </summary>
            ControlBlockAcknowledgment = 0x19,

            /// <summary>
            /// PS poll
            /// </summary>
            ControlPSPoll = 0x1A,

            /// <summary>
            /// RTS
            /// </summary>
            ControlRts = 0x1B,

            /// <summary>
            /// CTS
            /// </summary>
            ControlCts = 0x1C,

            /// <summary>
            /// ACK
            /// </summary>
            ControlAck = 0x1D,

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
            DataCFAck = 0x21,

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
            DataCFAckCFPollNoData = 0x27,

            /// <summary>
            /// Constant qos data.
            /// </summary>
            QosData = 0x28,

            /// <summary>
            /// Constant qos data and CF ack.
            /// </summary>
            QosDataAndCFAck = 0x29,

            /// <summary>
            /// Constant qos data and CF poll.
            /// </summary>
            QosDataAndCFPoll = 0x2A,

            /// <summary>
            /// Constant qos data and CF ack and CF poll.
            /// </summary>
            QosDataAndCFAckAndCFPoll = 0x2B,

            /// <summary>
            /// Constant qos null data.
            /// </summary>
            QosNullData = 0x2C,

            /// <summary>
            /// Constant qos CF ack.
            /// </summary>
            QosCFAck = 0x2D,

            /// <summary>
            /// Constant qos CF poll.
            /// </summary>
            QosCFPoll = 0x2E,

            /// <summary>
            /// Constant qos CF ack and CF poll.
            /// </summary>
            QosCFAckAndCFPoll = 0x2F
        }
    }
}