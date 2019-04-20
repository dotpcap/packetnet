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
    /// Drda protocol DDM field encoding information.
    /// </summary>
    public struct DrdaDdmFields
    {
        /// <summary>
        /// Length of the Code Point field in bytes.
        /// </summary>
        public static readonly int CodePointLength = 2;

        /// <summary>
        /// Position of the Code Point field
        /// </summary>
        public static readonly int CodePointPosition;

        /// <summary>
        /// Length of the CorrelId field in bytes.
        /// </summary>
        public static readonly int CorrelIdLength = 2;

        /// <summary>
        /// Position of the CorrlId field
        /// </summary>
        public static readonly int CorrelIdPosition;

        /// <summary>
        /// Total Length for DDM Head
        /// </summary>
        public static readonly int DDMHeadTotalLength;

        /// <summary>
        /// Length of the Format field in bytes.
        /// </summary>
        public static readonly int FormatLength = 1;

        /// <summary>
        /// Position of the Format field
        /// </summary>
        public static readonly int FormatPosition;

        /// <summary>
        /// Length of the Length2 number in bytes.
        /// </summary>
        public static readonly int Length2Length = 2;

        /// <summary>
        /// Position of the Length2 field
        /// </summary>
        public static readonly int Length2Position;

        /// <summary>
        /// Length of the Length number in bytes.
        /// </summary>
        public static readonly int LengthLength = 2;

        /// <summary>
        /// Position of the Length field
        /// </summary>
        public static readonly int LengthPosition = 0;

        /// <summary>
        /// Length of the Magic field in bytes.
        /// </summary>
        public static readonly int MagicLength = 1;

        /// <summary>
        /// Position of the Magic field
        /// </summary>
        public static readonly int MagicPosition;

        /// <summary>
        /// Length of the Parameter Code Point field in bytes.
        /// </summary>
        public static readonly int ParameterCodePointLength = 2;

        /// <summary>
        /// Length of the Parameter Length number in bytes.
        /// </summary>
        public static readonly int ParameterLengthLength = 2;

        static DrdaDdmFields()
        {
            MagicPosition = LengthPosition + LengthLength;
            FormatPosition = MagicPosition + MagicLength;
            CorrelIdPosition = FormatPosition + FormatLength;
            Length2Position = CorrelIdPosition + CorrelIdLength;
            CodePointPosition = Length2Position + Length2Length;
            DDMHeadTotalLength = CodePointPosition + CodePointLength;
        }
    }
}