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
    /// <summary> String constants for color console output.
    /// <p>
    /// This file contains control sequences to print color text on a text
    /// console capable of interpreting and displaying control sequences.
    /// </p>
    /// <p>
    /// A capable console would be
    /// unix bash, os/2 shell, or command.com w/ ansi.sys loaded
    /// </p>
    /// </summary>
    /// <author>  Chris Cheetham
    /// </author>
    public class AnsiEscapeSequences
    {
        /// <summary>
        /// Delimits the start of an ansi color sequence, the color code goes after this
        /// </summary>
        public readonly static String EscapeBegin;
        /// <summary>
        /// Delimits the stop of the ansi color sequence, the color code comes before this
        /// </summary>
        public readonly static String EscapeEnd = "m";

#pragma warning disable 1591
        public readonly static String Reset;
        public readonly static String Bold;
        public readonly static String Underline;
        public readonly static String Inverse;
        public readonly static String Black;
        public readonly static String Blue;
        public readonly static String Green;
        public readonly static String Cyan;
        public readonly static String Red;
        public readonly static String Purple;
        public readonly static String Brown;
        public readonly static String LightGray;
        public readonly static String DarkGray;
        public readonly static String LightBlue;
        public readonly static String LightGreen;
        public readonly static String LightCyan;
        public readonly static String LightRed;
        public readonly static String LightPurple;
        public readonly static String Yellow;
        public readonly static String White;
        public readonly static String RedBackground;
        public readonly static String GreenBackground;
        public readonly static String YellowBackground;
        public readonly static String BlueBackground;
        public readonly static String PurpleBackground;
        public readonly static String CyanBackground;
        public readonly static String LightGrayBackground;
#pragma warning restore 1591

        private static string BuildValue(string ColorCode)
        {
            return EscapeBegin + ColorCode + EscapeEnd;
        }

        static AnsiEscapeSequences()
        {
            EscapeBegin = "" + (char) 27 + "[";
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
    }
}