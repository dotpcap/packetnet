using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap.AirPcap;
using PacketDotNet.Ieee80211;
using System.Net.NetworkInformation;
using SharpPcap;
using PacketDotNet;

namespace ConstructingWiFiPackets
{
    class Program
    {
        private static PhysicalAddress adapterAddress;
        private static bool stopCapturing = false;
        
        static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
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

            int i = 0;

            // Print out the devices
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse(Console.ReadLine());

            // Register a cancle handler that lets us break out of our capture loop
            // since we currently need to synchronously receive packets in order to get
            // raw packets. Future versions of SharpPcap are likely to
            // return ONLY raw packets at which time we can simplify this code and
            // use a PcapDevice.OnPacketArrival handler
            Console.CancelKeyPress += HandleCancelKeyPress;

            var device = (AirPcapDevice)devices[i];
            
            device.Open(DeviceMode.Normal);
            device.FcsValidation = AirPcapValidationType.ACCEPT_CORRECT_FRAMES;
            adapterAddress = device.MacAddress;
            device.AirPcapLinkType = AirPcapLinkTypes._802_11;

            PhysicalAddress broadcastAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

            
            Console.Write("Please enter the SSID to probe for (use empty string for broadcast probe): ");
            String ssid = Console.ReadLine();
            Console.WriteLine();



            //Make the probe packet to send
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            InformationElement ssidIe = new InformationElement(InformationElement.ElementId.ServiceSetIdentity, encoding.GetBytes(ssid));
            InformationElement supportedRatesIe = new InformationElement(InformationElement.ElementId.SupportedRates,
                new byte[] { 0x02, 0x04, 0x0b, 0x16, 0x0c, 0x12, 0x18, 0x24 });
            InformationElement extendedSupportedRatesIe = new InformationElement(InformationElement.ElementId.ExtendedSupportedRates,
                new byte[] { 0x30, 0x48, 0x60, 0x6c });
            //Create a broadcast probe
            ProbeRequestFrame probe = new ProbeRequestFrame(device.MacAddress,
                                                            broadcastAddress,
                                                            broadcastAddress,
                                                            new InformationElementList() {ssidIe, supportedRatesIe, extendedSupportedRatesIe});

            Byte[] probeBytes = probe.Bytes;
            device.SendPacket(probeBytes, probeBytes.Length - 4);


            while (stopCapturing == false)
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
                MacFrame p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data) as MacFrame;
                if (p.FrameControl.SubType == FrameControlField.FrameSubTypes.ManagementProbeResponse)
                {
                    ProbeResponseFrame probeResponse = p as ProbeResponseFrame;
                    if (probeResponse.DestinationAddress.Equals(adapterAddress))
                    {
                        var ie = probeResponse.InformationElements.FindFirstById(InformationElement.ElementId.ServiceSetIdentity);
                        Console.WriteLine("Response: {0}, SSID: {1}", probeResponse.SourceAddress, Encoding.UTF8.GetString(ie.Value)); 
                    }
                }
            }
        }

        static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            MacFrame p = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data) as MacFrame;

            if (p.FrameControl.SubType == FrameControlField.FrameSubTypes.ManagementProbeResponse)
            {
                ProbeResponseFrame probeResponse = p as ProbeResponseFrame;
                if (probeResponse.DestinationAddress == adapterAddress)
                {
                    Console.WriteLine(probeResponse.ToString());

                }
            }
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
