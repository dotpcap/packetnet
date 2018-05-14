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
 *  Copyright 2017 Andrew <pandipd@outlook.com>
 */

using System;

namespace PacketDotNet
{
    /// <summary>
    /// The DDM Parameter subsection field
    /// </summary>
    public class DrdaDDMParameter
    {
        /// <summary>
        /// The Other Data field
        /// </summary>
        public String Data { set; get; }

        /// <summary>
        /// The Drda Code Point Type field
        /// </summary>
        public DrdaCodepointType DrdaCodepoint { set; get; }

        /// <summary>
        /// The Length field
        /// </summary>
        public Int32 Length { set; get; }
    }
}