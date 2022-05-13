/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace PacketDotNet.Ndp
{
    /// <summary>
    /// The different types fields that could be found in the Options field
    /// </summary>
    /// <remarks>
    /// References:
    /// https://datatracker.ietf.org/doc/html/rfc4861
    /// </remarks>
    public enum OptionTypes : byte
    {
        SourceLinkLayerAddress = 1,
        TargetLinkLayerAddress = 2,
        PrefixInformation = 3,
        RedirectedHeader = 4,
        Mtu = 5
    }
}