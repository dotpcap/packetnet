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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
using System;

namespace PacketDotNet
{
    /// <summary>
    /// Lengths and offsets to the fields in the LinuxSLL packet
    /// </summary>
    public class LinuxSLLFields
    {
        public readonly static int PacketTypeLength = 2;
        public readonly static int LinuxARPHRDLength = 2;
        public readonly static int LinkLayerAddressLengthLength = 2;
        public readonly static int LinkLayerHeaderMaximumLength = 8;
        public readonly static int EthernetProtocolTypeLength = 2;

        public readonly static int PacketTypePosition = 0;
        public readonly static int LinuxARPHRDPosition;
        public readonly static int LinkLayerAddressLengthPosition;
        public readonly static int LinkLayerHeaderPosition;
        public readonly static int EthernetProtocolTypePosition;

        static LinuxSLLFields()
        {
            LinuxARPHRDPosition = PacketTypePosition + PacketTypeLength;
            LinkLayerAddressLengthPosition = LinuxARPHRDPosition + LinuxARPHRDLength;
            LinkLayerHeaderPosition = LinkLayerAddressLengthPosition + LinkLayerAddressLengthLength;
            EthernetProtocolTypePosition = LinkLayerHeaderPosition + LinkLayerHeaderMaximumLength;
        }
    }
}
