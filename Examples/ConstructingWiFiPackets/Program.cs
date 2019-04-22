using System;
using System.Net.NetworkInformation;
using System.Text;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using SharpPcap;
using SharpPcap.AirPcap;

namespace ConstructingWiFiPackets
{
    class Program
    {
        private static PhysicalAddress _adapterAddress;
        private static bool _stopCapturing;

        private static void Main()
        {
            // Print SharpPcap version
            var ver = SharpPcap.Version.VersionString;
            Console.WriteLine("PacketDotNet example using SharpPcap {0}", ver);

            // Retrieve the device list
            var devices = AirPcapDeviceList.Instance;

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

            var i = 0;

            // Print out the devices
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = Int32.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

            // Register a cancel handler that lets us break out of our capture loop
            // since we currently need to synchronously receive packets in order to get
            // raw packets. Future versions of SharpPcap are likely to
            // return ONLY raw packets at which time we can simplify this code and
            // use a PcapDevice.OnPacketArrival handler
            Console.CancelKeyPress += HandleCancelKeyPress;

            var device = (AirPcapDevice) devices[i];

            device.Open(DeviceMode.Normal);
            device.FcsValidation = AirPcapValidationType.ACCEPT_CORRECT_FRAMES;
            _adapterAddress = device.MacAddress;
            device.AirPcapLinkType = AirPcapLinkTypes._802_11;

            var broadcastAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

            Console.Write("Please enter the SSID to probe for (use empty string for broadcast probe): ");
            var ssid = Console.ReadLine();
            Console.WriteLine();

            //Make the probe packet to send
            var encoding = new ASCIIEncoding();
            var ssidIe = new InformationElement(InformationElement.ElementId.ServiceSetIdentity, encoding.GetBytes(ssid ?? throw new InvalidOperationException()));
            var supportedRatesIe = new InformationElement(InformationElement.ElementId.SupportedRates,
                                                          new byte[] { 0x02, 0x04, 0x0b, 0x16, 0x0c, 0x12, 0x18, 0x24 });

            var extendedSupportedRatesIe = new InformationElement(InformationElement.ElementId.ExtendedSupportedRates,
                                                                  new byte[] { 0x30, 0x48, 0x60, 0x6c });

            //Create a broadcast probe
            var probe = new ProbeRequestFrame(device.MacAddress,
                                              broadcastAddress,
                                              broadcastAddress,
                                              new InformationElementList { ssidIe, supportedRatesIe, extendedSupportedRatesIe });

            var probeBytes = probe.Bytes;
            device.SendPacket(probeBytes, probeBytes.Length - 4);

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
                if (Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) is MacFrame p && p.FrameControl.SubType == FrameControlField.FrameSubTypes.ManagementProbeResponse)
                {
                    if (p is ProbeResponseFrame probeResponse && probeResponse.DestinationAddress.Equals(_adapterAddress))
                    {
                        var ie = probeResponse.InformationElements.FindFirstById(InformationElement.ElementId.ServiceSetIdentity);
                        Console.WriteLine("Response: {0}, SSID: {1}", probeResponse.SourceAddress, Encoding.UTF8.GetString(ie.Value));
                    }
                }
            }
        }

        static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
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