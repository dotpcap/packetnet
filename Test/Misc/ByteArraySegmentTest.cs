﻿/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet.Utils;

namespace Test.Misc;

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
            ClassicAssert.AreEqual(byteArraySegment.Length, data.Length);
            for (var i = 0; i < 20; i++)
                ClassicAssert.AreEqual(byteArraySegment[i], i);

            ClassicAssert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = byteArraySegment[20];
            });

            ClassicAssert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = byteArraySegment[-1];
            });

            var t = 0;
            foreach (var c in byteArraySegment)
            {
                ClassicAssert.AreEqual(t++, c);
            }
        }
    }