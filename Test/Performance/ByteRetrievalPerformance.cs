/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System;
using log4net.Core;
using NUnit.Framework;
using PacketDotNet;

namespace Test.Performance;

    /// <summary>
    /// Compares retrieving a byte[] from a packet that is built from contiguous memory
    /// vs. one that is built from several byte[]. This evaluates the performance cost
    /// of having to build a contiguous byte[] from non-continguous packets
    /// </summary>
    [TestFixture]
    public class ByteRetrievalPerformance
    {
        private EthernetPacket BuildNonContiguousEthernetPacket()
        {
            // build an ethernet packet
            var ethernetPacket = EthernetPacket.RandomPacket();

            // build an ip packet
            var ipPacket = IPPacket.RandomPacket(IPVersion.IPv6);

            ethernetPacket.PayloadPacket = ipPacket;

            return ethernetPacket;
        }

        [Test]
        public void TestOptimalByteRetrieval()
        {
            var ethernetPacket = BuildNonContiguousEthernetPacket();

            // now extract a contiguous series of bytes
            var contiguousBytes = ethernetPacket.Bytes;

            // and re-parse the packet
            var contiguousEthernetPacket = new EthernetPacket(new PacketDotNet.Utils.ByteArraySegment(contiguousBytes));

            // used to make sure we get the same byte[] reference returned each time
            // because thats what we expect
            byte[] bArray = null;

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            // now benchmark retrieving the byte[] for several seconds
            var startTime = DateTime.Now;
            var endTime = startTime.Add(new TimeSpan(0, 0, 2));
            var testRuns = 0;
            while (DateTime.Now < endTime)
            {
                var bs = contiguousEthernetPacket.Bytes;

                // make sure that we always get back the same reference
                // for the byte[]
                if (bArray == null)
                {
                    bArray = bs;
                }
                else
                {
                    Assert.AreSame(bArray, bs);
                }

                testRuns++;
            }

            // update the actual end of the loop
            endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            Console.WriteLine(rate.ToString());
        }

        [Test]
        public void TestSubOptimalByteRetrieval()
        {
            var ethernetPacket = BuildNonContiguousEthernetPacket();

            byte[] lastByteArray = null;

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            // now benchmark retrieving the byte[] for several seconds
            var startTime = DateTime.Now;
            var endTime = startTime.Add(new TimeSpan(0, 0, 2));
            var testRuns = 0;
            while (DateTime.Now < endTime)
            {
                var bs = ethernetPacket.Bytes;

                // make sure we don't get back the same reference
                if (lastByteArray == null)
                {
                    lastByteArray = bs;
                }
                else
                {
                    Assert.AreNotSame(lastByteArray, bs);
                    lastByteArray = bs;
                }

                testRuns++;
            }

            // update the actual end of the loop
            endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            Console.WriteLine(rate.ToString());
        }
    }