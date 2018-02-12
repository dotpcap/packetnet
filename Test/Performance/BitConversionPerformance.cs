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
using System.IO;
using System.Net;
using log4net.Core;
using NUnit.Framework;
using PacketDotNet.Utils.Conversion;
using PacketDotNet.Utils.IO;

namespace Test.Performance
{
    [TestFixture]
    public class BitConversionPerformance
    {
        [Test]
        public void ArrayCopyBitConverterIpAddressPerformance()
        {
            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;


            Byte[] bytes;
            Int32 testRuns;
            Int32 startIndex;
            Int32 expectedValue;
            ByteSetupMethods.Setup(out bytes, out testRuns, out startIndex,
                out expectedValue);

            var startTime = DateTime.Now;

            for (Int32 i = 0; i < testRuns; i++)
            {
                var actualValue = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, startIndex));

                // NOTE: Assert.AreEqual() significantly slows, by a factor of ~6x
                //       the execution of this loop, so we perform ourself and
                //       then call Assert.AreEqual() if the comparison fails.
                //       This doesn't reduce performance by a noticable amount
                if (actualValue != expectedValue)
                {
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            var endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            Console.WriteLine(rate.ToString());
        }

        [Test]
        public void EndianBitConverterPerformance()
        {
            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            Byte[] bytes;
            Int32 testRuns;
            Int32 startIndex;
            Int32 expectedValue;
            ByteSetupMethods.Setup(out bytes, out testRuns, out startIndex,
                out expectedValue);

            var startTime = DateTime.Now;

            for (Int32 i = 0; i < testRuns; i++)
            {
                var actualValue = EndianBitConverter.Big.ToInt32(bytes, startIndex);

                // NOTE: Assert.AreEqual() significantly slows, by a factor of ~6x
                //       the execution of this loop, so we perform ourself and
                //       then call Assert.AreEqual() if the comparison fails.
                //       This doesn't reduce performance by a noticable amount
                if (actualValue != expectedValue)
                {
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            var endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            Console.WriteLine(rate.ToString());
        }

        [Test]
        public void EndianReaderWriterPerformance()
        {
            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            Byte[] bytes;
            Int32 testRuns;
            Int32 startIndex;
            Int32 expectedValue;
            ByteSetupMethods.Setup(out bytes, out testRuns, out startIndex,
                out expectedValue);

            var memStream = new MemoryStream(bytes);
            var endianReader = new EndianBinaryReader(EndianBitConverter.Big, memStream);

            var startTime = DateTime.Now;

            for (Int32 i = 0; i < testRuns; i++)
            {
                endianReader.Seek(startIndex, SeekOrigin.Begin);
                var actualValue = endianReader.ReadInt32();

                // NOTE: Assert.AreEqual() significantly slows, by a factor of ~6x
                //       the execution of this loop, so we perform ourself and
                //       then call Assert.AreEqual() if the comparison fails.
                //       This doesn't reduce performance by a noticable amount
                if (actualValue != expectedValue)
                {
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            var endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            Console.WriteLine(rate.ToString());
        }
    }
}