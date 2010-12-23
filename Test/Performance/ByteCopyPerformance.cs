using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;
using NUnit.Framework;
using PacketDotNet;

namespace Test.Performance
{
    [TestFixture]
    public class ByteCopyPerformance
    {
        // The number of times the test is run
        int testRuns = 40000;

        [Test]
        public void ArrayCopyPerformance()
        {
            // create a realistic packet for testing
            var ethernetPacket = EthernetPacket.RandomPacket();
            // create the array to store the copy result
            byte[] hwAddress = new byte[EthernetFields.MacAddressLength];

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            // Store the time before the processing starts
            var startTime = DateTime.Now;

            // run the test
            for (int i = 0; i < testRuns; i++)
            {
                Array.Copy(ethernetPacket.Bytes, EthernetFields.SourceMacPosition,
                    hwAddress, 0, EthernetFields.MacAddressLength);
            }

            // store the time after the processing is finished
            var endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            // calculate the statistics
            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            // output the statistics to the console
            Console.WriteLine(rate.ToString());
        }

        [Test]
        public void BufferCopyPerformance()
        {
            // create a realistic packet for testing
            var ethernetPacket = EthernetPacket.RandomPacket();
            // create the array to store the copy result
            byte[] hwAddress = new byte[EthernetFields.MacAddressLength];

            // store the logging value
            var oldThreshold = LoggingConfiguration.GlobalLoggingLevel;

            // disable logging to improve performance
            LoggingConfiguration.GlobalLoggingLevel = log4net.Core.Level.Off;

            // Store the time before the processing starts
            var startTime = DateTime.Now;

            // run the test
            for (int i = 0; i < testRuns; i++)
            {
                Buffer.BlockCopy(ethernetPacket.Bytes, EthernetFields.SourceMacPosition,
                    hwAddress, 0, EthernetFields.MacAddressLength);
            }

            // store the time after the processing is finished
            var endTime = DateTime.Now;

            // restore logging
            LoggingConfiguration.GlobalLoggingLevel = oldThreshold;

            // calculate the statistics
            var rate = new Rate(startTime, endTime, testRuns, "Test runs");

            // output the statistics to the console
            Console.WriteLine(rate.ToString());
        }

    }
}
