/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2017 Chris Morgan <chmorgan@gmail.com>
 */

namespace PacketDotNet.Ieee80211
{
    public struct LogicalLinkControlFields
    {
        public static readonly int ControlOrganizationLength = 4;
        public static readonly int SsapLength = 1;
        public static readonly int DsapLength = 1;
        public static readonly int DsapPosition = 0;
        public static readonly int SsapPosition = DsapPosition + DsapLength;
        public static readonly int ControlOrganizationPosition = SsapPosition + SsapLength;
        public static readonly int TypeLength = 2;
        public static readonly int TypePosition = ControlOrganizationPosition + ControlOrganizationLength;
        public static readonly int HeaderLength = TypePosition + TypeLength;

        static LogicalLinkControlFields()
        { }
    }
}