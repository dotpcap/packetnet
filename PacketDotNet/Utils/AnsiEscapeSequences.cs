// ************************************************************************
// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
// Distributed under the Mozilla Public License                            *
// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
// *************************************************************************
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
        public readonly static String ESCAPE_BEGIN;
        public readonly static String ESCAPE_END = "m";
        public readonly static String RESET;
        public readonly static String BOLD;
        public readonly static String UNDERLINE;
        public readonly static String INVERSE;
        public readonly static String BLACK;
        public readonly static String BLUE;
        public readonly static String GREEN;
        public readonly static String CYAN;
        public readonly static String RED;
        public readonly static String PURPLE;
        public readonly static String BROWN;
        public readonly static String LIGHT_GRAY;
        public readonly static String DARK_GRAY;
        public readonly static String LIGHT_BLUE;
        public readonly static String LIGHT_GREEN;
        public readonly static String LIGHT_CYAN;
        public readonly static String LIGHT_RED;
        public readonly static String LIGHT_PURPLE;
        public readonly static String YELLOW;
        public readonly static String WHITE;
        public readonly static String RED_BACKGROUND;
        public readonly static String GREEN_BACKGROUND;
        public readonly static String YELLOW_BACKGROUND;
        public readonly static String BLUE_BACKGROUND;
        public readonly static String PURPLE_BACKGROUND;
        public readonly static String CYAN_BACKGROUND;
        public readonly static String LIGHT_GRAY_BACKGROUND;

        private static string BuildValue(string ColorCode)
        {
            return ESCAPE_BEGIN + ColorCode + ESCAPE_END;
        }

        static AnsiEscapeSequences()
        {
            ESCAPE_BEGIN = "" + (char) 27 + "[";
            RESET = BuildValue("0");
            BOLD = BuildValue("0;1");
            UNDERLINE = BuildValue("0;4");
            INVERSE = BuildValue("0;7");
            BLACK = BuildValue("0;30");
            BLUE = BuildValue("0;34");
            GREEN = BuildValue("0;32");
            CYAN = BuildValue("0;36");
            RED = BuildValue("0;31");
            PURPLE = BuildValue("0;35");
            BROWN = BuildValue("0;33");
            LIGHT_GRAY = BuildValue("0;37");
            DARK_GRAY = BuildValue("1;30");
            LIGHT_BLUE = BuildValue("1;34");
            LIGHT_GREEN = BuildValue("1;32");
            LIGHT_CYAN = BuildValue("1;36");
            LIGHT_RED = BuildValue("1;31");
            LIGHT_PURPLE = BuildValue("1;35");
            YELLOW = BuildValue("1;33");
            WHITE = BuildValue("1;37");
            RED_BACKGROUND = BuildValue("0;41");
            GREEN_BACKGROUND = BuildValue("0;42");
            YELLOW_BACKGROUND = BuildValue("0;43");
            BLUE_BACKGROUND = BuildValue("0;44");
            PURPLE_BACKGROUND = BuildValue("0;45");
            CYAN_BACKGROUND = BuildValue("0;46");
            LIGHT_GRAY_BACKGROUND = BuildValue("0;47");
        }
    }
}