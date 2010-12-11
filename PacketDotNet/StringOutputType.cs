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
using System;
namespace PacketDotNet
{
    /// <summary>
    /// The available types of strings that the ToString(StringOutputType) can handle.
    /// </summary>
    public enum StringOutputType
    {
        /// <summary>
        /// Outputs the packet info on a single line
        /// </summary>
        Normal,
        /// <summary>
        /// Outputs the packet info on a single line with coloring
        /// </summary>
        Colored,
        /// <summary>
        /// Outputs the detailed packet info
        /// </summary>
        Verbose,
        /// <summary>
        /// Outputs the detailed packet info with coloring
        /// </summary>
        VerboseColored
    }
}