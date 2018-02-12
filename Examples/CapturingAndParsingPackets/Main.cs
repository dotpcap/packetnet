using System;
using PacketDotNet;
using SharpPcap;
using Version = SharpPcap.Version;

namespace CapturingAndParsingPackets
{
    internal class MainClass
    {
        // used to stop the capture loop
        private static Boolean _stopCapturing;

        public static void Main(String[] args)
        {
            // Print SharpPcap version
            String ver = Version.VersionString;
            Console.WriteLine("PacketDotNet example using SharpPcap {0}", ver);

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            Int32 i = 0;

            // Print out the devices
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = Int32.Parse(Console.ReadLine());

            // Register a cancle handler that lets us break out of our capture loop
            Console.CancelKeyPress += HandleCancelKeyPress;

            var device = devices[i];

            // Open the device for capturing
            Int32 readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0}, hit 'ctrl-c' to stop...",
                device.Name);

            while (_stopCapturing == false)
            {
                var rawCapture = device.GetNextPacket();

                // null packets can be returned in the case where
                // the GetNextRawPacket() timed out, we should just attempt
                // to retrieve another packet by looping the while() again
                if (rawCapture == null)
                {
                    // go back to the start of the while()
                    continue;
                }

                // use PacketDotNet to parse this packet and print out
                // its high level information
                Packet p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                Console.WriteLine(p.ToString());
            }

            Console.WriteLine("-- Capture stopped");

            // Print out the device statistics
            Console.WriteLine(device.Statistics.ToString());

            // Close the pcap device
            device.Close();
        }

        private static void HandleCancelKeyPress(Object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("-- Stopping capture");
            _stopCapturing = true;

            // tell the handler that we are taking care of shutting down, don't
            // shut us down after we return because we need to do just a little
            // bit more processing to close the open capture device etc
            e.Cancel = true;
        }
    }
}