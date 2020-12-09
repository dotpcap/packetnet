/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using NUnit.Framework;

namespace Test.Misc
{
    [TestFixture]
    public class ChecksumUtils
    {
        [Test]
        public void OnesSum()
        {
            var bytes = new byte[4096];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 0x7f;
            }

            var result = PacketDotNet.Utils.ChecksumUtils.OnesSum(bytes);
            Console.WriteLine("result: {0}", result);
        }
    }
}