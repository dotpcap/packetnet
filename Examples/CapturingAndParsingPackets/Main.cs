using System;
using SharpPcap;
using PacketDotNet;

namespace CapturingAndParsingPackets
{
    class MainClass
    {
        // used to stop the capture loop
        private static bool stopCapturing = false;

        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("PacketDotNet example using SharpPcap {0}", ver);

            // Retrieve the device list
            var devices = LivePcapDeviceList.Instance;

            // If no devices were found print an error
            if(devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the devices
            foreach(LivePcapDevice dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );


            // Register a cancle handler that lets us break out of our capture loop
            // since we currently need to synchronously receive packets in order to get
            // raw packets. Future versions of SharpPcap are likely to
            // return ONLY raw packets at which time we can simplify this code and
            // use a PcapDevice.OnPacketArrival handler
            Console.CancelKeyPress += HandleCancelKeyPress;

            LivePcapDevice device = devices[i];

#if false
            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival += 
                new PacketArrivalEventHandler( device_OnPacketArrival );
#endif

            // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0}, hit 'ctrl-c' to stop...",
                device.Description);

#if false
            // Start the capturing process
            device.StartCapture();

            // Wait for 'Enter' from the user.
            Console.ReadLine();

            // Stop the capturing process
            device.StopCapture();
#else
            while(stopCapturing == false)
            {
                var rawPacket = device.GetNextRawPacket();

                // null packets can be returned in the case where
                // the GetNextRawPacket() timed out, we should just attempt
                // to retrieve another packet by looping the while() again
                if(rawPacket == null)
                {
                    // go back to the start of the while()
                    continue;
                }

                // use PacketDotNet to parse this packet and print out
                // its high level information

                // NOTE: Please feel free to copy this code, its a pain to convert the types
                //       but future versions of SharpPcap may come bundled with PacketDotNet
                //       so SharpPcap can provide packets in the PacketDotNet format, avoiding
                //       this odd looking conversion
                Packet p = Packet.ParsePacket((LinkLayers)rawPacket.LinkLayerType,
                                          new PosixTimeval(rawPacket.Timeval.Seconds,
                                                           rawPacket.Timeval.MicroSeconds),
                                          rawPacket.Data);

                Console.WriteLine(p.ToString());
            }
#endif

            Console.WriteLine("-- Capture stopped");

            // Print out the device statistics
            Console.WriteLine(device.Statistics().ToString());

            // Close the pcap device
            device.Close();
        }

        static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("-- Stopping capture");
            stopCapturing = true;

            // tell the handler that we are taking care of shutting down, don't
            // shut us down after we return because we need to do just a little
            // bit more processing to close the open capture device etc
            e.Cancel = true;
        }

#if false
        /// <summary>
        /// Prints the time and length of each received packet
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            DateTime time = e.Packet.PcapHeader.Date;
            uint len = e.Packet.PcapHeader.PacketLength;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}", 
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(e.Packet.ToString());
        }
#endif
    }
}