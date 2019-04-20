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
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
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
            byte[] data = new byte[10];
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }

            // NOTE: we use TcpPacket because it has a simple constructor. We can't
            //       create a Packet() instance because Packet is an abstract class
            var p = new TcpPacket(10, 10);
            p.PayloadData = data;
        }
    }
}
