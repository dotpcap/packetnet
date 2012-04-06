using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using SharpPcap.AirPcap;

namespace CapturingAndParsingWiFiPackets
{
    class MainClass
    {
        // used to stop the capture loop
        private static bool stopCapturing = false;

        public static void Main (string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine ("PacketDotNet example using SharpPcap {0}", ver);

            // Retrieve the device list
            var devices = AirPcapDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine ("No devices were found on this machine");
                return;
            }

            Console.WriteLine ();
            Console.WriteLine ("The following devices are available on this machine:");
            Console.WriteLine ("----------------------------------------------------");
            Console.WriteLine ();

            int i = 0;

            // Print out the devices
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine ("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine ();
            Console.Write ("-- Please choose a device to capture: ");
            i = int.Parse (Console.ReadLine ());

            // Register a cancle handler that lets us break out of our capture loop
            // since we currently need to synchronously receive packets in order to get
            // raw packets. Future versions of SharpPcap are likely to
            // return ONLY raw packets at which time we can simplify this code and
            // use a PcapDevice.OnPacketArrival handler
            Console.CancelKeyPress += HandleCancelKeyPress;

            var device = (AirPcapDevice)devices [i];

            // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open (DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device.FcsValidation = AirPcapValidationType.ACCEPT_CORRECT_FRAMES;

            Console.WriteLine ();
            Console.WriteLine ("-- Listening on {0}, hit 'ctrl-c' to stop...",
                device.Name);

            while (stopCapturing == false)
            {
                var rawCapture = device.GetNextPacket ();

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
                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                MacFrame macFrame = (MacFrame)p.PayloadPacket;
                if ((macFrame != null) && 
                    (macFrame.FrameControl.SubType == FrameControlField.FrameSubTypes.ManagementBeacon))
                {
                    BeaconFrame beaconFrame = (BeaconFrame)macFrame;
                    var ie = beaconFrame.InformationElements.FindFirstById(InformationElement.ElementId.ServiceSetIdentity);
                    Console.WriteLine ("Network: {0}, Access Point Address: {1}", Encoding.UTF8.GetString (ie.Value), beaconFrame.SourceAddress);       
                }
            }

            Console.WriteLine ("-- Capture stopped");

            // Print out the device statistics
            Console.WriteLine (device.Statistics.ToString ());

            // Close the pcap device
            device.Close ();
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
    }
}
