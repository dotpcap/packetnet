/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    public struct NdpFields
    {
        public static int NeighborAdvertisementFlagsOffset = 0;
        public static int NeighborAdvertisementOptionsOffset = 20;
        public static int NeighborAdvertisementTargetAddressOffset = 4;

        public static int NeighborSolicitationOptionsAddressOffset = 20;
        public static int NeighborSolicitationTargetAddressOffset = 4;

        public static int RouterAdvertisementCurrentHopLimitOffset = 0;
        public static int RouterAdvertisementExtOffset = 1;
        public static int RouterAdvertisementOptionsOffset = 12;
        public static int RouterAdvertisementReachableTimeOffset = 4;
        public static int RouterAdvertisementRetransmitTimerOffset = 8;
        public static int RouterAdvertisementRouterLifetimeOffset = 2;

        public static int RouterSolicitationOptionsOffset = 4;

        public static int RedirectMessageTargetAddressOffset = 4;
        public static int RedirectMessageDestinationAddressOffset = 20;
        public static int RedirectMessageOptionsOffset = 36;
    }