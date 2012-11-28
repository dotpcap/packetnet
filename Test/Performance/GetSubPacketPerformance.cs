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
 *  Copyright 2012 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using NUnit.Framework;
using log4net.Core;
using PacketDotNet;

namespace Test.Performance
{
    /// <summary>
    /// Measures the speed at which we can retrieve sub-packets either through
    /// the xxx.GetEncapsulated() methods or others
    /// </summary>
    [TestFixture]
    public class GetSubPacketPerformance
    {
        static UInt16 tcpSourcePort = 60;
        static UInt16 tcpDestinationPort = 90;

        private EthernetPacket BuildTCPPacket()
        {
            // build an ethernet packet
            var ethernetPacket = EthernetPacket.RandomPacket();

            // build an ip packet
            var ipPacket = IpPacket.RandomPacket(IpVersion.IPv6);

            var tcpPacket = TcpPacket.RandomPacket();
            tcpPacket.SourcePort = tcpSourcePort;
            tcpPacket.DestinationPort = tcpDestinationPort;

            ipPacket.PayloadPacket = tcpPacket;
            ethernetPacket.PayloadPacket = ipPacket;

            return ethernetPacket;
        }

        /// <summary>
        /// Tests the performance of TcpPacket.GetEncapsulated()
        /// </summary>
        [Test]
        public void TestGetEncapsulated()
        {
            var ethernetPacket = BuildTCPPacket();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            var startTime = DateTime.Now;
            var endTime = startTime.Add(new TimeSpan(0, 0, 15));
            int testRuns = 0;
            while(DateTime.Now < endTime)
            {
                // Disable CS0618 (use of obsolete method), because we are testing the now obsolete
                // methods for performance
#pragma warning disable 0618
                var tcpPacket = TcpPacket.GetEncapsulated(ethernetPacket);
#pragma warning restore 0618

                Assert.IsNotNull(tcpPacket);
                Assert.AreEqual(tcpPacket.SourcePort, tcpSourcePort);
                Assert.AreEqual(tcpPacket.DestinationPort, tcpDestinationPort);

                testRuns++;
            }

            // update the actual end of the loop
            endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            Console.WriteLine(rate.ToString());
        }

        /// <summary>
        /// Tests the performance of Packet.Extract(type)
        /// </summary>
        [Test]
        public void TestPacketExtract()
        {
            var ethernetPacket = BuildTCPPacket();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            var startTime = DateTime.Now;
            var endTime = startTime.Add(new TimeSpan(0, 0, 15));
            int testRuns = 0;
            while(DateTime.Now < endTime)
            {
                var tcpPacket = (TcpPacket)ethernetPacket.Extract(typeof(TcpPacket));

                Assert.IsNotNull(tcpPacket);
                Assert.AreEqual(tcpPacket.SourcePort, tcpSourcePort);
                Assert.AreEqual(tcpPacket.DestinationPort, tcpDestinationPort);

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
}
