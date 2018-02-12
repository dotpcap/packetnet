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
        private static CaptureFileReaderDevice _captureFileReader;

        private static FileAndPacketIndexes _currentFapi;

        private static String _currentPacketDescription;

        /// <summary>
        ///     Index into FileAndPacketIndexes.PacketIndex
        /// </summary>
        private static Int32 _currentPacketIndex;

        private static readonly Int32 ExpectedTotalPackets;

        private static Int32 _fileAndPacketIndex;

        // ReSharper disable once InconsistentNaming
        private static readonly List<FileAndPacketIndexes> _fileAndPacketIndexes;
        private static Int32 _indexIntoPacketFile;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static String _packetFileName;

        private static Int32 _totalPacketsReturned;

        static StringOutput()
        {
            _fileAndPacketIndexes = new List<FileAndPacketIndexes>();

            /*
             * TODO:
             * Need to add packets of these types:
             * igmpv2
             */

            var prefix = "../../CaptureFiles/";

            /////////////////////////
            // setup an array of file names and packet indexes

            // ethernet arp request and response
            _fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "arp_request_response.pcap",
                new List<Int32>(new[] {0, 1}),
                new List<String>(new[]
                {
                    "ethernet arp request",
                    "ethernet arp response"
                })));

            // linux cooked capture, ipv4, tcp
            _fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "LinuxCookedCapture.pcap",
                new List<Int32>(new[] {2}),
                new List<String>(new[] {"linux cooked capture, ipv4, tcp"})));

            // ethernet, ipv6, icmpv6
            _fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "ipv6_icmpv6_packet.pcap",
                new List<Int32>(new[] {0}),
                new List<String>(new[] {"ethernet, ipv6, icmpv6"})));

            // ethernet, PPPoE, PPP, ipv4, udp
            _fileAndPacketIndexes.Add(new FileAndPacketIndexes(prefix + "PPPoEPPP.pcap",
                new List<Int32>(new[] {1}),
                new List<String>(new[] {"ethernet, PPPoE, PPP, ipv4, udp"})));

            ExpectedTotalPackets = 5;
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
            if (_currentFapi != null)
            {
                Log.DebugFormat("currentFAPI.PacketIndexes.Count {0}," +
                                "currentPacketIndex {1}",
                    _currentFapi.PacketIndexes.Count,
                    _currentPacketIndex);
            }
            else
            {
                Log.Debug("currentFAPI is null");
            }

            // do we need to open a file up or are we done with the current file?
            if ((_packetFileName == null) ||
                (_currentFapi == null) ||
                (_currentFapi.PacketIndexes.Count == _currentPacketIndex))
            {
                Log.Debug("opening a new file up");

                // close the open device if there was one
                if (_captureFileReader != null)
                {
                    _captureFileReader.Close();
                    _captureFileReader = null;
                }

                // do we have any more files to process?
                if (_fileAndPacketIndex >= _fileAndPacketIndexes.Count)
                {
                    Log.DebugFormat("totalPacketsReturned {0}, expectedTotalPackets {1}",
                        _totalPacketsReturned,
                        ExpectedTotalPackets);

                    Assert.AreEqual(ExpectedTotalPackets, _totalPacketsReturned,
                        "expectedTotalPackets does not match totalPacketsReturned");

                    return null;
                }

                _currentFapi = _fileAndPacketIndexes[_fileAndPacketIndex];
                _currentPacketIndex = 0;
                _packetFileName = _currentFapi.Filename;

                // opening a new file, we are at the first index into the new file
                _indexIntoPacketFile = 0;

                try
                {
                    Log.DebugFormat("Opening {0}", _currentFapi.Filename);

                    _captureFileReader = new CaptureFileReaderDevice(_currentFapi.Filename);
                    _captureFileReader.Open();

                    _fileAndPacketIndex++;
                }
                catch (Exception e)
                {
                    Log.Error("caught exception", e);
                    throw;
                }
            }

            Packet p = null;

            do
            {
                Log.DebugFormat("currentPacketIndex {0}", _currentPacketIndex);
                Log.DebugFormat("indexIntoPacketFile {0}, currentFAPI.PacketIndexes[currentPacketIndex] {1}",
                    _indexIntoPacketFile,
                    _currentFapi.PacketIndexes[_currentPacketIndex]);

                Log.Debug("retrieving packet");

                // read the next packet
                var packet = _captureFileReader.GetNextPacket();
                Assert.IsNotNull(packet, "Expected a valid packet but it was null");

                p = Packet.ParsePacket(packet.LinkLayerType, packet.Data);

                _currentPacketDescription = _currentFapi.PacketDescription[_currentPacketIndex];

                // advance our index into the current packet file
                _indexIntoPacketFile++;
            } while ((_indexIntoPacketFile - 1) != _currentFapi.PacketIndexes[_currentPacketIndex]);
            // does the current index match the index of the packet we want?

            // and because we got a packet we advance our index into the FileAndPacketIndex class
            _currentPacketIndex++;

            Log.Debug("returning packet");
            _totalPacketsReturned++;
            return p;
        }

        private void OutputPacket(Packet p, StringOutputType outputType)
        {
            Console.WriteLine(_currentPacketDescription + " - " + outputType);
            Console.Write(p.ToString(outputType));
            if (outputType == StringOutputType.Verbose || outputType == StringOutputType.VerboseColored)
                Console.Write(p.PrintHex());
            Console.WriteLine();
        }

        private void ResetPacketPosition()
        {
            _fileAndPacketIndex = 0;
            _currentPacketIndex = 0;
            _packetFileName = null;

            _indexIntoPacketFile = 0;

            _totalPacketsReturned = 0;

            if (_captureFileReader != null)
            {
                _captureFileReader.Close();
                _captureFileReader = null;
            }
        }

        private class FileAndPacketIndexes
        {
            public readonly String Filename;
            public readonly List<String> PacketDescription;
            public readonly List<Int32> PacketIndexes;

            public FileAndPacketIndexes(String filename,
                List<Int32> packetIndexes,
                List<String> packetDescription)
            {
                this.Filename = filename;
                this.PacketIndexes = packetIndexes;
                this.PacketDescription = packetDescription;
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