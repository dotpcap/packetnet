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
 *  Copyright 2018 Steven Haufe<haufes@hotmail.com>
 */
namespace PacketDotNet.L2TP
{
    /// <summary> L2TP protocol field encoding information. </summary>
    public class L2TPFields
    {

        /// <summary> Length of the Base Header in bytes.</summary>
        public readonly static int HeaderLength = 2;
        /// <summary> Length of the Flags in bytes.</summary>
        public readonly static int FlagsLength = 2;
        /// <summary> Length of the Length in bytes.</summary>
        public readonly static int LengthsLength = 2;
        /// <summary> Length of the Ns in bytes.</summary>
        public readonly static int NsLength = 2;
        /// <summary> Length of the Nr in bytes.</summary>
        public readonly static int NrLength = 2;
        /// <summary> Length of the Offset Size in bytes (Optional).</summary>      
        public readonly static int OffsetSizeLength = 2;
        /// <summary> Length of the Offset Pad in bytes (Optional).</summary>
        public readonly static int OffsetPadLength = 2;


    }
}
