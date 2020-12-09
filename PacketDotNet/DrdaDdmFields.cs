/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

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
        /// Length of the CorrelationId field in bytes.
        /// </summary>
        public static readonly int CorrelationIdLength = 2;

        /// <summary>
        /// Position of the CorrelationId field
        /// </summary>
        public static readonly int CorrelationIdPosition;

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
            CorrelationIdPosition = FormatPosition + FormatLength;
            Length2Position = CorrelationIdPosition + CorrelationIdLength;
            CodePointPosition = Length2Position + Length2Length;
            DDMHeadTotalLength = CodePointPosition + CodePointLength;
        }
    }
}