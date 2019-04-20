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
using NUnit.Framework;
using PacketDotNet;

namespace Test.Misc
{
    /// <summary>
    /// Unit test that tries to exercise the string output methods in all of the
    /// packet classes to make it easier to identify and fix printing issues
    /// </summary>
    [TestFixture]
    public class StringOutput
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private class FileAndPacketIndexes
        {
            public string Filename;
            public List<int> PacketIndexes;
            public List<string> PacketDescription;

            public FileAndPacketIndexes(string Filename,
                                        List<int> PacketIndexes,
                                        List<string> PacketDescription)
            {
                this.Filename = Filename;
                this.PacketIndexes = PacketIndexes;
                this.PacketDescription = PacketDescription;
            }
        }

        private static List<FileAndPacketIndexes> fileAndPacketIndexes;

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
                                                              new List<int>(new int[] {0, 1}),
                                                              new List<string>(new string[] { "ethernet arp request",
                                                                                              "ethernet arp response"})));

            // linux cooked capture, ipv4, tcp
            fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "LinuxCookedCapture.pcap",
                                                              new List<int>(new int[] {2}),
                                                              new List<string>(new string[] { "linux cooked capture, ipv4, tcp"})));

            // ethernet, ipv6, icmpv6
            fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "ipv6_icmpv6_packet.pcap",
                                                              new List<int>(new int[] {0}),
                                                              new List<string>(new string[] { "ethernet, ipv6, icmpv6"})));

            // ethernet, PPPoE, PPP, ipv4, udp
            fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "PPPoEPPP.pcap",
                                                              new List<int>(new int[] {1}),
                                                              new List<string>(new string[] { "ethernet, PPPoE, PPP, ipv4, udp"})));

            expectedTotalPackets = 5;
        }

        private static int fileAndPacketIndex;

        /// <summary>
        /// Index into FileAndPacketIndexes.PacketIndex
        /// </summary>
        private static int currentPacketIndex;

        private static string currentPacketDescription;

        private static string packetFileName;
        private static int indexIntoPacketFile;

        private static FileAndPacketIndexes currentFAPI;

        private static SharpPcap.LibPcap.CaptureFileReaderDevice captureFileReader;

        private static int totalPacketsReturned;
        private static int expectedTotalPackets;

        private static Packet GetNextPacket()
        {
            if(currentFAPI != null)
            {
                log.DebugFormat("currentFAPI.PacketIndexes.Count {0}," +
                                "currentPacketIndex {1}",
                                currentFAPI.PacketIndexes.Count,
                                currentPacketIndex);
            } else
            {
                log.Debug("currentFAPI is null");
            }
            // do we need to open a file up or are we done with the current file?
            if((packetFileName == null) ||
               (currentFAPI == null) ||
               (currentFAPI.PacketIndexes.Count == currentPacketIndex))
            {
                log.Debug("opening a new file up");

                // close the open device if there was one
                if(captureFileReader != null)
                {
                    captureFileReader.Close();
                    captureFileReader = null;
                }

                // do we have any more files to process?
                if(fileAndPacketIndex >= fileAndPacketIndexes.Count)
                {
                    log.DebugFormat("totalPacketsReturned {0}, expectedTotalPackets {1}",
                                    totalPacketsReturned,
                                    expectedTotalPackets);

                    Assert.AreEqual(expectedTotalPackets, totalPacketsReturned,
                                   "expectedTotalPackets does not match totalPacketsReturned");

                    return null;
                } else
                {
                    currentFAPI = fileAndPacketIndexes[fileAndPacketIndex];
                    currentPacketIndex = 0;
                    packetFileName = currentFAPI.Filename;

                    // opening a new file, we are at the first index into the new file
                    indexIntoPacketFile = 0;

                    try
                    {
                        log.DebugFormat("Opening {0}", currentFAPI.Filename);

                        captureFileReader = new SharpPcap.LibPcap.CaptureFileReaderDevice(currentFAPI.Filename);
                        captureFileReader.Open();

                        fileAndPacketIndex++;
                    } catch(System.Exception e)
                    {
                        log.Error("caught exception",e);
                        throw;
                    }
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
            } while((indexIntoPacketFile -1) != currentFAPI.PacketIndexes[currentPacketIndex]);
            // does the current index match the index of the packet we want?

            // and because we got a packet we advance our index into the FileAndPacketIndex class
            currentPacketIndex++;

            log.Debug("returning packet");
            totalPacketsReturned++;
            return p;
        }

        private void ResetPacketPosition()
        {
            fileAndPacketIndex = 0;
            currentPacketIndex = 0;
            packetFileName = null;

            indexIntoPacketFile = 0;

            totalPacketsReturned = 0;

            if(captureFileReader != null)
            {
                captureFileReader.Close();
                captureFileReader = null;
            }
        }

        enum OutputType
        {
            ToString,
            ToColoredString,
            ToColoredVerboseString
        }

        private void OutputPacket(Packet p, StringOutputType outputType)
        {
            Console.WriteLine(currentPacketDescription + " - " + outputType);
            Console.Write(p.ToString(outputType));
            if(outputType == StringOutputType.Verbose || outputType == StringOutputType.VerboseColored)
                Console.Write(p.PrintHex());
            Console.WriteLine();
        }

        [Test]
        public void _ToString()
        {
            ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            Packet p;
            while((p = GetNextPacket()) != null)
            {
                OutputPacket(p, StringOutputType.Normal);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        [Test]
        public void ToColoredString()
        {
            ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            Packet p;
            while((p = GetNextPacket()) != null)
            {
                OutputPacket(p, StringOutputType.Colored);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        [Test]
        public void ToVerboseString()
        {
            ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            Packet p;
            while((p = GetNextPacket()) != null)
            {
                OutputPacket(p, StringOutputType.Verbose);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }

        [Test]
        public void ToColoredVerboseString()
        {
            ResetPacketPosition();

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to reduce console output
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            Packet p;
            while((p = GetNextPacket()) != null)
            {
                OutputPacket(p, StringOutputType.VerboseColored);
            }

            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;
        }
    }
}