using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PacketDotNet;
using PacketDotNet.Utils;
using PacketDotNet.Ieee80211;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    namespace Ieee80211
    {
        [TestFixture]
        public class InformationElementListTest
        {
            
            [Test]
            public void Test_Constuctor_FromBeaconFrame ()
            {
                var dev = new CaptureFileReaderDevice ("../../CaptureFiles/80211_beacon_frame.pcap");
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                BeaconFrame beaconFrame = (BeaconFrame)p.PayloadPacket;
                
                List<InformationElement> infoElements = beaconFrame.InformationElements;
                Assert.AreEqual (InformationElement.ElementId.ServiceSetIdentity, infoElements [0].Id);
                Assert.AreEqual (InformationElement.ElementId.SupportedRates, infoElements [1].Id);
                Assert.AreEqual (InformationElement.ElementId.DsParameterSet, infoElements [2].Id);
                Assert.AreEqual (InformationElement.ElementId.TrafficIndicationMap, infoElements [3].Id);
                Assert.AreEqual (InformationElement.ElementId.ErpInformation, infoElements [4].Id);
                Assert.AreEqual (InformationElement.ElementId.ErpInformation2, infoElements [5].Id);
                Assert.AreEqual (InformationElement.ElementId.RobustSecurityNetwork, infoElements [6].Id);
                Assert.AreEqual (InformationElement.ElementId.ExtendedSupportedRates, infoElements [7].Id);
                Assert.AreEqual (InformationElement.ElementId.HighThroughputCapabilities, infoElements [8].Id);
                Assert.AreEqual (InformationElement.ElementId.HighThroughputInformation, infoElements [9].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [10].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [11].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [12].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [13].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [14].Id);
            }
            
            
            [Test]
            public void Test_Constructor_EmptyByteArray ()
            {
                ByteArraySegment bas = new ByteArraySegment (new Byte[0]);
                InformationElementList ieList = new InformationElementList (bas);

                Assert.AreEqual (0, ieList.Count);
                Assert.AreEqual (0, ieList.Length);
            }

            [Test]
            public void Test_Constructor_MultipleInfoElementByteArray ()
            {
                //create a dummy info element section for the test. The example here uses 
                //two arbitrary field types (0xD3 & 0xDD). The important information for the test is the length
                //field which is the second byte on each row (0x5 & 0x4). The actual values are meaningless.
                Byte[] ieBytes = new Byte[] { 0xD3, 0x05, 0x01, 0x02, 0x03, 0x04, 0x05,
                                              0xDD, 0x04, 0xFF, 0xFE, 0xFD, 0xFC };
                ByteArraySegment bas = new ByteArraySegment (ieBytes);

                InformationElementList ieList = new InformationElementList (bas);
                Assert.AreEqual (2, ieList.Count);
                Assert.AreEqual (13, ieList.Length);

                Assert.AreEqual (InformationElement.ElementId.WifiProtectedAccess, ieList [0].Id);
                Assert.AreEqual (5, ieList [0].ValueLength);
                Assert.IsTrue (ieList [0].Value.SequenceEqual (new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }));

                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, ieList [1].Id);
                Assert.AreEqual (4, ieList [1].ValueLength);
                Assert.IsTrue (ieList [1].Value.SequenceEqual (new Byte[] { 0xFF, 0xFE, 0xFD, 0xFC }));
            }

            [Test]
            public void Test_Constructor_EmptyList ()
            {             
                InformationElementList ieList = new InformationElementList ();

                Assert.AreEqual (0, ieList.Count);
                Assert.AreEqual (0, ieList.Length);
            }

            [Test]
            public void Test_Bytes_EmptySection ()
            {
                ByteArraySegment bas = new ByteArraySegment (new Byte[0]);
                InformationElementList ieList = new InformationElementList (bas);

                Assert.AreEqual (0, ieList.Bytes.Length);
            }

            [Test]
            public void Test_Bytes_MultipleInfoElements ()
            {
                InformationElement ie1 = new InformationElement (
                    InformationElement.ElementId.WifiProtectedAccess, new Byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 });

                InformationElement ie2 = new InformationElement (
                    InformationElement.ElementId.VendorSpecific, new Byte[] { 0xFF, 0xFE, 0xFD, 0xFC });
          
                InformationElementList ieList = new InformationElementList ();
                ieList.Add (ie1);
                ieList.Add (ie2);

                Byte[] expectedBytes = new Byte[] { 0xD3, 0x5, 0x1, 0x2, 0x3, 0x4, 0x5, 0xDD, 0x4, 0xFF, 0xFE, 0xFD, 0xFC };

                Byte[] b = ieList.Bytes;

                Assert.AreEqual (0xD3, b [0]);
                Assert.AreEqual (0x5, b [1]);
                Assert.AreEqual (0x1, b [2]);
                Assert.AreEqual (0x2, b [3]);
                Assert.AreEqual (0x3, b [4]);
                Assert.AreEqual (0x4, b [5]);
                Assert.AreEqual (0x5, b [6]);
                Assert.AreEqual (0xDD, b [7]);
                Assert.AreEqual (0x4, b [8]);
                Assert.AreEqual (0xFF, b [9]);
                Assert.AreEqual (0xFE, b [10]);
                Assert.AreEqual (0xFD, b [11]);
                Assert.AreEqual (0xFC, b [12]);

                Assert.IsTrue (ieList.Bytes.SequenceEqual (expectedBytes));
            }
            
            [Test]
            public void Test_FindFirstById_FindElement ()
            {
                BeaconFrame beaconFrame = LoadBeaconFrameFromFile("../../CaptureFiles/80211_beacon_frame.pcap");
                            
                InformationElement infoElement = beaconFrame.InformationElements.FindFirstById (InformationElement.ElementId.DsParameterSet);
                Assert.AreEqual (InformationElement.ElementId.DsParameterSet, infoElement.Id);
            }
            
            [Test]
            public void Test_FindFirstById_ElementNotPresent ()
            {
                BeaconFrame beaconFrame = LoadBeaconFrameFromFile ("../../CaptureFiles/80211_beacon_frame.pcap");
                            
                InformationElement infoElement = beaconFrame.InformationElements.FindFirstById (InformationElement.ElementId.ChallengeText);
                Assert.IsNull (infoElement);
            }
            
            [Test]
            public void Test_FindById_FindRepeatedElement ()
            {
                BeaconFrame beaconFrame = LoadBeaconFrameFromFile ("../../CaptureFiles/80211_beacon_frame.pcap");
                            
                InformationElement[] infoElements = beaconFrame.InformationElements.FindById (InformationElement.ElementId.VendorSpecific);
                
                Assert.AreEqual (5, infoElements.Length);              
                
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [0].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [1].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [2].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [3].Id);
                Assert.AreEqual (InformationElement.ElementId.VendorSpecific, infoElements [4].Id);
            }
            
            [Test]
            public void Test_FindById_ElementNotPresent ()
            {
                BeaconFrame beaconFrame = LoadBeaconFrameFromFile ("../../CaptureFiles/80211_beacon_frame.pcap");
                            
                InformationElement[] infoElements = beaconFrame.InformationElements.FindById (InformationElement.ElementId.ChallengeText);
                Assert.AreEqual (0, infoElements.Length);
            }
            
            private BeaconFrame LoadBeaconFrameFromFile (String file)
            {
                var dev = new CaptureFileReaderDevice (file);
                dev.Open ();
                var rawCapture = dev.GetNextPacket ();
                dev.Close ();

                Packet p = Packet.ParsePacket (rawCapture.LinkLayerType, rawCapture.Data);
                BeaconFrame beaconFrame = (BeaconFrame)p.PayloadPacket;
                return beaconFrame;
            }
        }
    }
}
