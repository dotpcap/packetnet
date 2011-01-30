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

namespace Test.Misc
{
    [TestFixture]
    public class ChecksumUtils
    {
        [Test]
        public void OnesSum()
        {
            var bytes = new Byte[4096];
            for(int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 0x7f;
            }

            var result = PacketDotNet.Utils.ChecksumUtils.OnesSum(bytes);
            Console.WriteLine("result: {0}", result);
        }
    }
}
