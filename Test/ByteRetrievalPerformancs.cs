using System;
using NUnit.Framework;
using log4net.Core;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class ByteRetrievalPerformancs
    {
        private EthernetPacket BuildNonContiguousEthernetPacket()
        {
            // build an ethernet packet
            var ethernetPacket = EthernetPacket.RandomPacket();

            // build an ip packet
            var ipPacket = IpPacket.RandomPacket(IpVersion.IPv6);

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
            var contiguousEthernetPacket = new EthernetPacket(contiguousBytes, 0);

            // used to make sure we get the same byte[] reference returned each time
            // because thats what we expect
            byte[] theByteArray = null;

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            // now benchmark retrieving the byte[] for several seconds
            var startTime = DateTime.Now;
            var endTime = startTime.Add(new TimeSpan(0, 0, 2));
            int testRuns = 0;
            while(DateTime.Now < endTime)
            {
                var theBytes = contiguousEthernetPacket.Bytes;

                // make sure that we always get back the same reference
                // for the byte[]
                if(theByteArray == null)
                {
                    theByteArray = theBytes;
                } else
                {
                    Assert.AreSame(theByteArray, theBytes);
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
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            // now benchmark retrieving the byte[] for several seconds
            var startTime = DateTime.Now;
            var endTime = startTime.Add(new TimeSpan(0, 0, 2));
            int testRuns = 0;
            while(DateTime.Now < endTime)
            {
                var theBytes = ethernetPacket.Bytes;

                // make sure we don't get back the same reference
                if(lastByteArray == null)
                {
                    lastByteArray = theBytes;
                } else
                {
                    Assert.AreNotSame(lastByteArray, theBytes);
                    lastByteArray = theBytes;
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
}
