/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  This file is licensed under the Apache License, Version 2.0.
 */

namespace PacketDotNet
{
    public struct DhcpV4Fields
    {
        /// <summary>
        /// The broadcast mask.
        /// </summary>
        public const int BroadcastMask = 0b10000000;

        /// <summary>
        /// The ch addr position.
        /// </summary>
        public const int ChAddrPosition = 28;

        /// <summary>
        /// The ci addr position.
        /// </summary>
        public const int CiAddrPosition = 12;

        /// <summary>
        /// The client port.
        /// </summary>
        public const int ClientPort = 68;

        /// <summary>
        /// The flags position.
        /// </summary>
        public const int FlagsPosition = 10;

        /// <summary>
        /// The gi addr position.
        /// </summary>
        public const int GiAddrPosition = 24;

        /// <summary>
        /// The hardware length position.
        /// </summary>
        public const int HardwareLengthPosition = 2;

        /// <summary>
        /// The hardware type position.
        /// </summary>
        public const int HardwareTypePosition = 1;

        /// <summary>
        /// The hops position.
        /// </summary>
        public const int HopsPosition = 3;

        /// <summary>
        /// The magic number.
        /// </summary>
        public const int MagicNumber = 0x63825363;

        /// <summary>
        /// The magic number position.
        /// </summary>
        public const int MagicNumberPosition = 236;

        /// <summary>
        /// The minimum size.
        /// </summary>
        public const int MinimumSize = 240;

        /// <summary>
        /// The operation position.
        /// </summary>
        public const int OperationPosition = 0;

        /// <summary>
        /// The options position.
        /// </summary>
        public const int OptionsPosition = 240;

        /// <summary>
        /// The secs position.
        /// </summary>
        public const int SecsPosition = 8;

        /// <summary>
        /// The server port.
        /// </summary>
        public const int ServerPort = 67;

        /// <summary>
        /// The si addr position.
        /// </summary>
        public const int SiAddrPosition = 20;

        /// <summary>
        /// The xid position.
        /// </summary>
        public const int XidPosition = 4;

        /// <summary>
        /// The yi addr position.
        /// </summary>
        public const int YiAddrPosition = 16;
    }
}