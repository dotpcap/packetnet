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
    [Serializable]
    public class DrdaDDMFields
    {
        /// <summary>
        /// Length of the Length number in bytes.
        /// </summary>
        public readonly static int LengthLength = 2;

        /// <summary>
        /// Length of the Magic field in bytes.
        /// </summary>
        public readonly static int MagicLength = 1;

        /// <summary>
        /// Length of the Format field in bytes.
        /// </summary>
        public readonly static int FormatLength = 1;

        /// <summary>
        /// Length of the CorrelId field in bytes.
        /// </summary>
        public readonly static int CorrelIdLength = 2;

        /// <summary>
        /// Length of the Length2 number in bytes.
        /// </summary>
        public readonly static int Length2Length = 2;

        /// <summary>
        /// Length of the Code Point field in bytes.
        /// </summary>
        public readonly static int CodePointLength = 2;

        /// <summary>
        /// Position of the Length field
        /// </summary>
        public readonly static int LengthPosition = 0;

        /// <summary>
        /// Position of the Magic field
        /// </summary>
        public readonly static int MagicPosition;

        /// <summary>
        /// Position of the Format field
        /// </summary>
        public readonly static int FormatPosition;

        /// <summary>
        /// Position of the CorrlId field
        /// </summary>
        public readonly static int CorrelIdPosition;

        /// <summary>
        /// Position of the Length2 field
        /// </summary>
        public readonly static int Length2Position;

        /// <summary>
        /// Position of the Code Point field
        /// </summary>
        public readonly static int CodePointPosition;

        /// <summary>
        /// Total Length for DDM Head
        /// </summary>
        public readonly static int DDMHeadTotalLength;

        /// <summary>
        /// Length of the Parameter Length number in bytes.
        /// </summary>
        public readonly static int ParameterLengthLength = 2;

        /// <summary>
        /// Length of the Parameter Code Point field in bytes.
        /// </summary>
        public readonly static int ParameterCodePointLength = 2;

        static DrdaDDMFields()
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
