/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet;

    public enum DhcpV4MessageType : byte
    {
        Discover = 1,
        Offer = 2,
        Request = 3,
        Decline = 4,
        Ack = 5,
        Nak = 6,
        Release = 7,
        Inform = 8,
        ForceRenew = 9,
        LeaseQuery = 10,
        LeaseUnassigned = 11,
        LeaseUnknown = 12,
        LeaseActive = 13,
        BulkLeaseQuery = 14,
        LeaseQueryDone = 15,
        ActiveLeaseQuery = 16,
        LeaseQueryStatus = 17,
        Tls = 18,
        End = 255
    }