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
using PacketDotNet.Utils;

namespace Test.Misc
{
    [TestFixture]
    public class ByteArraySegmentTest
    {
        [Test]
        public void TestByteArraySegment()
        {
            var data = new byte[20];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte) i;
            }

            var byteArraySegment = new ByteArraySegment(data);
            Assert.AreEqual(byteArraySegment.Length, data.Length);
            for (var i = 0; i < 20; i++)
                Assert.AreEqual(byteArraySegment[i], i);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = byteArraySegment[20];
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = byteArraySegment[-1];
            });

            var t = 0;
            foreach (var c in byteArraySegment)
            {
                Assert.AreEqual(t++, c);
            }
        }
    }
}