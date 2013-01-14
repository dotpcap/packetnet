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
 *  Copyright 2013 Chris Morgan <chmorgan@gmail.com>
 */
namespace PacketDotNet
{
    /// <summary> 802.1Q fields </summary>
    public class Ieee8021QFields
    {
        /// <summary> Length of the ethertype value in bytes.</summary>
        public readonly static int TypeLength = 2;
        /// <summary> Length of the tag control information in bytes. </summary>
        public readonly static int TagControlInformationLength = 2;
        /// <summary> Position of the tag control information </summary>
        public readonly static int TagControlInformationPosition = 0;
        /// <summary> Position of the type field </summary>
        public readonly static int TypePosition;
        /// <summary> Length in bytes of a Ieee8021Q header.</summary>
        public readonly static int HeaderLength; // 4

        static Ieee8021QFields()
        {
            TypePosition = TagControlInformationPosition + TagControlInformationLength;
            HeaderLength = TypePosition + TypeLength;
        }
    }
}
