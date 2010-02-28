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

namespace Test
{
    [TestFixture]
    public class PacketDotNet_vs_SharpPcapPerformance
    {
        private static int testRuns = 10000;
        private static string captureFile = "../../CaptureFiles/ipv6_http.pcap";
        private static int expectedSourcePort = 123;
        private static int expectedDestinationPort = 321;

        // delegate used for the implementation specific callback
        private delegate bool ProcessRawPacketDelegate(SharpPcap.Packets.RawPacket rawPacket);

        /// <summary>
        /// SharpPcap specific processing routine
        /// </summary>
        /// <param name="rawPacket">
        /// A <see cref="SharpPcap.Packets.RawPacket"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        private bool SharpPcapProcess(SharpPcap.Packets.RawPacket rawPacket)
        {
            var packet = SharpPcap.Packets.PacketFactory.dataToPacket(rawPacket.LinkLayerType,
                                                         rawPacket.Data);

            if(packet is SharpPcap.Packets.TCPPacket)
            {
                var tcpPacket = (SharpPcap.Packets.TCPPacket)packet;
                if((tcpPacket.SourcePort == expectedSourcePort) &&
                   (tcpPacket.DestinationPort == expectedDestinationPort))
                {
                    return true;
                }
            } else
            {
                Assert.Fail("Packet is not a TCPPacket but it should be");
            }

            return false;
        }

        /// <summary>
        /// Packet.Net specific packet processing routine
        /// </summary>
        /// <param name="rawPacket">
        /// A <see cref="SharpPcap.Packets.RawPacket"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        private bool PacketDotNetProcess(SharpPcap.Packets.RawPacket rawPacket)
        {
            var packet = SharpPcapRawPacketToPacket.RawPacketToPacket(rawPacket);

            var tcpPacket = TcpPacket.GetType(packet);
            if(tcpPacket != null)
            {
                if((tcpPacket.SourcePort == expectedSourcePort) &&
                   (tcpPacket.DestinationPort == expectedDestinationPort))
                {
                    return true;
                }
            } else
            {
                Assert.Fail("Packet is not a TcpPacket but it should be");
            }

            return false;
        }

        /// <summary>
        /// Common test routine
        /// </summary>
        /// <param name="processHandler">
        /// A <see cref="ProcessRawPacketDelegate"/>
        /// </param>
        private void PerformTest(ProcessRawPacketDelegate processHandler)
        {
            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            var startTime = DateTime.Now;

            int matchesFound = 0;
            int packetsProcessed = 0;
            for(int i = 0; i < testRuns; i++)
            {
                var pcapOfflineFile = new SharpPcap.OfflinePcapDevice(captureFile);
                pcapOfflineFile.Open();

                SharpPcap.Packets.RawPacket rawPacket = null;
                while((rawPacket = pcapOfflineFile.GetNextRawPacket()) != null)
                {
                    packetsProcessed++;

                    if(processHandler(rawPacket))
                        matchesFound++;
                }

                pcapOfflineFile.Close();
            }

            var endTime = DateTime.Now;

            var rate = new Rate(startTime, endTime, packetsProcessed, "packets processed");
            Console.WriteLine(rate.ToString());

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        [Test]
        public void SharpPcapTcpComparison()
        {
            PerformTest(new ProcessRawPacketDelegate(SharpPcapProcess));
        }

        [Test]
        public void PacketDotNetTcpComparison()
        {
            PerformTest(new ProcessRawPacketDelegate(PacketDotNetProcess));
        }
    }
}
