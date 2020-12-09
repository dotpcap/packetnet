/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
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