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
using System.Collections.Generic;
using System.Reflection;
using log4net;
using log4net.Core;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap.LibPcap;

namespace Test.Misc
{
    /// <summary>
    ///     Unit test that tries to exercise the string output methods in all of the
    ///     packet classes to make it easier to identify and fix printing issues
    /// </summary>
    [TestFixture]
    public class StringOutput
    {
        private static CaptureFileReaderDevice captureFileReader;

        private static FileAndPacketIndexes currentFAPI;

        private static String currentPacketDescription;

        /// <summary>
        ///     Index into FileAndPacketIndexes.PacketIndex
        /// </summary>
        private static Int32 currentPacketIndex;

        private static readonly Int32 expectedTotalPackets;

        private static Int32 fileAndPacketIndex;

        private static readonly List<FileAndPacketIndexes> fileAndPacketIndexes;
        private static Int32 indexIntoPacketFile;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static String packetFileName;

        private static Int32 totalPacketsReturned;

        static StringOutput()
        {
            fileAndPacketIndexes = new List<FileAndPacketIndexes>();

            /*
             * TODO:
             * Need to add packets of these types:
             * igmpv2
             */

            var prefix = "../../CaptureFiles/";

            /////////////////////////
            // setup an array of file names and packet indexes

            // ethernet arp request and response
            fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "arp_request_response.pcap",
                new List<Int32>(new[] {0, 1}),
                new List<String>(new[]
                {
                    "ethernet arp request",
                    "ethernet arp response"
                })));

            // linux cooked capture, ipv4, tcp
            fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "LinuxCookedCapture.pcap",
                new List<Int32>(new[] {2}),
                new List<String>(new[] {"linux cooked capture, ipv4, tcp"})));

            // ethernet, ipv6, icmpv6
            fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "ipv6_icmpv6_packet.pcap",
                new List<Int32>(new[] {0}),
                new List<String>(new[] {"ethernet, ipv6, icmpv6"})));

            // ethernet, PPPoE, PPP, ipv4, udp
            fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "PPPoEPPP.pcap",
                new List<Int32>(new[] {1}),
                new List<String>(new[] {"ethernet, PPPoE, PPP, ipv4, udp"})));

            expectedTotalPackets = 5;
        }

        [Test]
        public void _ToString()
        {
            this.ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            Packet p;
            while ((p = GetNextPacket()) != null)
            {
                this.OutputPacket(p, StringOutputType.Normal);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        [Test]
        public void ToColoredString()
        {
            this.ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            Packet p;
            while ((p = GetNextPacket()) != null)
            {
                this.OutputPacket(p, StringOutputType.Colored);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        [Test]
        public void ToColoredVerboseString()
        {
            this.ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            Packet p;
            while ((p = GetNextPacket()) != null)
            {
                this.OutputPacket(p, StringOutputType.VerboseColored);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        [Test]
        public void ToVerboseString()
        {
            this.ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = Level.Off;

            Packet p;
            while ((p = GetNextPacket()) != null)
            {
                this.OutputPacket(p, StringOutputType.Verbose);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        private static Packet GetNextPacket()
        {
            if (currentFAPI != null)
            {
                log.DebugFormat("currentFAPI.PacketIndexes.Count {0}," +
                                "currentPacketIndex {1}",
                    currentFAPI.PacketIndexes.Count,
                    currentPacketIndex);
            }
            else
            {
                log.Debug("currentFAPI is null");
            }

            // do we need to open a file up or are we done with the current file?
            if ((packetFileName == null) ||
                (currentFAPI == null) ||
                (currentFAPI.PacketIndexes.Count == currentPacketIndex))
            {
                log.Debug("opening a new file up");

                // close the open device if there was one
                if (captureFileReader != null)
                {
                    captureFileReader.Close();
                    captureFileReader = null;
                }

                // do we have any more files to process?
                if (fileAndPacketIndex >= fileAndPacketIndexes.Count)
                {
                    log.DebugFormat("totalPacketsReturned {0}, expectedTotalPackets {1}",
                        totalPacketsReturned,
                        expectedTotalPackets);

                    Assert.AreEqual(expectedTotalPackets, totalPacketsReturned,
                        "expectedTotalPackets does not match totalPacketsReturned");

                    return null;
                }

                currentFAPI = fileAndPacketIndexes[fileAndPacketIndex];
                currentPacketIndex = 0;
                packetFileName = currentFAPI.Filename;

                // opening a new file, we are at the first index into the new file
                indexIntoPacketFile = 0;

                try
                {
                    log.DebugFormat("Opening {0}", currentFAPI.Filename);

                    captureFileReader = new CaptureFileReaderDevice(currentFAPI.Filename);
                    captureFileReader.Open();

                    fileAndPacketIndex++;
                }
                catch (Exception e)
                {
                    log.Error("caught exception", e);
                    throw;
                }
            }

            Packet p = null;

            do
            {
                log.DebugFormat("currentPacketIndex {0}", currentPacketIndex);
                log.DebugFormat("indexIntoPacketFile {0}, currentFAPI.PacketIndexes[currentPacketIndex] {1}",
                    indexIntoPacketFile,
                    currentFAPI.PacketIndexes[currentPacketIndex]);

                log.Debug("retrieving packet");

                // read the next packet
                var packet = captureFileReader.GetNextPacket();
                Assert.IsNotNull(packet, "Expected a valid packet but it was null");

                p = Packet.ParsePacket(packet.LinkLayerType, packet.Data);

                currentPacketDescription = currentFAPI.PacketDescription[currentPacketIndex];

                // advance our index into the current packet file
                indexIntoPacketFile++;
            } while ((indexIntoPacketFile - 1) != currentFAPI.PacketIndexes[currentPacketIndex]);
            // does the current index match the index of the packet we want?

            // and because we got a packet we advance our index into the FileAndPacketIndex class
            currentPacketIndex++;

            log.Debug("returning packet");
            totalPacketsReturned++;
            return p;
        }

        private void OutputPacket(Packet p, StringOutputType outputType)
        {
            Console.WriteLine(currentPacketDescription + " - " + outputType);
            Console.Write(p.ToString(outputType));
            if (outputType == StringOutputType.Verbose || outputType == StringOutputType.VerboseColored)
                Console.Write(p.PrintHex());
            Console.WriteLine();
        }

        private void ResetPacketPosition()
        {
            fileAndPacketIndex = 0;
            currentPacketIndex = 0;
            packetFileName = null;

            indexIntoPacketFile = 0;

            totalPacketsReturned = 0;

            if (captureFileReader != null)
            {
                captureFileReader.Close();
                captureFileReader = null;
            }
        }

        private class FileAndPacketIndexes
        {
            public readonly String Filename;
            public readonly List<String> PacketDescription;
            public readonly List<Int32> PacketIndexes;

            public FileAndPacketIndexes(String Filename,
                List<Int32> PacketIndexes,
                List<String> PacketDescription)
            {
                this.Filename = Filename;
                this.PacketIndexes = PacketIndexes;
                this.PacketDescription = PacketDescription;
            }
        }

        private enum OutputType
        {
            ToString,
            ToColoredString,
            ToColoredVerboseString
        }
    }
}