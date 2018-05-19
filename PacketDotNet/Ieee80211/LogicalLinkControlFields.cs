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
 *  Copyright 2017 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet.Ieee80211
{
    class LogicalLinkControlFields
    {
        public static readonly Int32 ControlOrganizationLength = 4;
        public static readonly Int32 SsapLength = 1;
        public static readonly Int32 DsapLength = 1;
        public static readonly Int32 DsapPosition = 0;
        public static readonly Int32 SsapPosition = DsapPosition + DsapLength;
        public static readonly Int32 ControlOrganizationPosition = SsapPosition + SsapLength;


        public static readonly Int32 TypeLength = 2;
        public static readonly Int32 TypePosition = ControlOrganizationPosition + ControlOrganizationLength;
        public static readonly Int32 HeaderLength = TypePosition + TypeLength;


        static LogicalLinkControlFields()
        { }
    }
}