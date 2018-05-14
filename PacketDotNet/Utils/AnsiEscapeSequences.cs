// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************
/*
 * Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// String constants for color console output.
    /// <p>
    /// This file contains control sequences to print color text on a text
    /// console capable of interpreting and displaying control sequences.
    /// </p>
    /// <p>
    /// A capable console would be
    /// unix bash, os/2 shell, or command.com w/ ansi.sys loaded
    /// </p>
    /// </summary>
    /// <author>
    /// Chris Cheetham
    /// </author>
    public class AnsiEscapeSequences
    {
        /// <summary>
        /// Delimits the start of an ansi color sequence, the color code goes after this
        /// </summary>
        public static readonly String EscapeBegin;

        /// <summary>
        /// Delimits the stop of the ansi color sequence, the color code comes before this
        /// </summary>
        public static readonly String EscapeEnd = "m";

        static AnsiEscapeSequences()
        {
            EscapeBegin = "" + (Char) 27 + "[";
            Reset = BuildValue("0");
            Bold = BuildValue("0;1");
            Underline = BuildValue("0;4");
            Inverse = BuildValue("0;7");
            Black = BuildValue("0;30");
            Blue = BuildValue("0;34");
            Green = BuildValue("0;32");
            Cyan = BuildValue("0;36");
            Red = BuildValue("0;31");
            Purple = BuildValue("0;35");
            Brown = BuildValue("0;33");
            LightGray = BuildValue("0;37");
            DarkGray = BuildValue("1;30");
            LightBlue = BuildValue("1;34");
            LightGreen = BuildValue("1;32");
            LightCyan = BuildValue("1;36");
            LightRed = BuildValue("1;31");
            LightPurple = BuildValue("1;35");
            Yellow = BuildValue("1;33");
            White = BuildValue("1;37");
            RedBackground = BuildValue("0;41");
            GreenBackground = BuildValue("0;42");
            YellowBackground = BuildValue("0;43");
            BlueBackground = BuildValue("0;44");
            PurpleBackground = BuildValue("0;45");
            CyanBackground = BuildValue("0;46");
            LightGrayBackground = BuildValue("0;47");
        }

        private static String BuildValue(String colorCode)
        {
            return EscapeBegin + colorCode + EscapeEnd;
        }

#pragma warning disable 1591
        public static readonly String Reset;
        public static readonly String Bold;
        public static readonly String Underline;
        public static readonly String Inverse;
        public static readonly String Black;
        public static readonly String Blue;
        public static readonly String Green;
        public static readonly String Cyan;
        public static readonly String Red;
        public static readonly String Purple;
        public static readonly String Brown;
        public static readonly String LightGray;
        public static readonly String DarkGray;
        public static readonly String LightBlue;
        public static readonly String LightGreen;
        public static readonly String LightCyan;
        public static readonly String LightRed;
        public static readonly String LightPurple;
        public static readonly String Yellow;
        public static readonly String White;
        public static readonly String RedBackground;
        public static readonly String GreenBackground;
        public static readonly String YellowBackground;
        public static readonly String BlueBackground;
        public static readonly String PurpleBackground;
        public static readonly String CyanBackground;
        public static readonly String LightGrayBackground;
#pragma warning restore 1591
    }
}