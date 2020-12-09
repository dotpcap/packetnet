/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using NUnit.Framework;
using PacketDotNet;

namespace Test.Misc
{
    [TestFixture]
    public class PacketTest
    {
        [Test]
        public void TestSettingPayloadData()
        {
            var data = new byte[10];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte) i;
            }

            // NOTE: we use TcpPacket because it has a simple constructor. We can't
            //       create a Packet() instance because Packet is an abstract class
            var _ = new TcpPacket(10, 10) { PayloadData = data };
        }
    }
}