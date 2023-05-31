/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet;

    /// <summary>
    /// Point to Point Protocol
    /// See http://tools.ietf.org/html/rfc2516
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public struct PppoeFields
    {
        /// <summary>Size in bytes of the code field.</summary>
        public static readonly int CodeLength = 1;

        /// <summary>Offset from the start of the header to the Code field.</summary>
        public static readonly int CodePosition;

        /// <summary>Length of the overall PPPoe header.</summary>
        public static readonly int HeaderLength;

        /// <summary>Size in bytes of the Length field.</summary>
        public static readonly int LengthLength = 2;

        /// <summary>Offset from the start of the header to the Length field.</summary>
        public static readonly int LengthPosition;

        /// <summary>Size in bytes of the SessionId field.</summary>
        public static readonly int SessionIdLength = 2;

        /// <summary>Offset from the start of the header to the SessionId field.</summary>
        public static readonly int SessionIdPosition;

        /// <summary>Size in bytes of the version/type field.</summary>
        public static readonly int VersionTypeLength = 1;

        /// <summary>Offset from the start of the header to the version/type field.</summary>
        public static readonly int VersionTypePosition = 0;

        static PppoeFields()
        {
            CodePosition = VersionTypePosition + VersionTypeLength;
            SessionIdPosition = CodePosition + CodeLength;
            LengthPosition = SessionIdPosition + SessionIdLength;

            HeaderLength = LengthPosition + LengthLength;
        }
    }