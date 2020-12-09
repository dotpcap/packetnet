/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  This file is licensed under the Apache License, Version 2.0.
 */

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PacketDotNet
{
    public enum DhcpV4Operation : byte
    {
        BootRequest = 1,
        BootReply = 2
    }
}