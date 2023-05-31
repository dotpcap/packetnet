/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using System.IO;
using System.Net;
using log4net.Core;
using NUnit.Framework;
using PacketDotNet.Utils.Converters;

namespace Test.Performance;

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

            ByteSetupMethods.Setup(out var bytes,
                                   out var testRuns,
                                   out var startIndex,
                                   out var expectedValue);

            var startTime = DateTime.Now;

            for (var i = 0; i < testRuns; i++)
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

            ByteSetupMethods.Setup(out var bytes,
                                   out var testRuns,
                                   out var startIndex,
                                   out var expectedValue);

            var startTime = DateTime.Now;

            for (var i = 0; i < testRuns; i++)
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

            ByteSetupMethods.Setup(out var bytes,
                                   out var testRuns,
                                   out var startIndex,
                                   out var expectedValue);

            var memStream = new MemoryStream(bytes);
            var endianReader = new EndianBinaryReader(EndianBitConverter.Big, memStream);

            var startTime = DateTime.Now;

            for (var i = 0; i < testRuns; i++)
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