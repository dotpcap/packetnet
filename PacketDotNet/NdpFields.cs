/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    public struct NdpFields
    {
        public static int NeighborAdvertisementFlagsOffset = 4;
        public static int NeighborAdvertisementOptionsOffset = 24;
        public static int NeighborAdvertisementTargetAddressOffset = 8;

        public static int NeighborSolicitationOptionsAddressOffset = 24;
        public static int NeighborSolicitationTargetAddressOffset = 8;

        public static int RouterAdvertisementCurrentHopLimitOffset = 4;
        public static int RouterAdvertisementExtOffset = 5;
        public static int RouterAdvertisementOptionsOffset = 16;
        public static int RouterAdvertisementReachableTimeOffset = 8;
        public static int RouterAdvertisementRetransmitTimerOffset = 12;
        public static int RouterAdvertisementRouterLifetimeOffset = 6;

        public static int RouterSolicitationOptionsOffset = 8;

        public static int RedirectMessageTargetAddressOffset = 8;
        public static int RedirectMessageDestinationAddressOffset = 24;
        public static int RedirectMessageOptionsOffset = 40;
    }