using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet;
using PacketDotNet.Lsa;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType;

    [TestFixture]
    public class OspfV2PacketTest
    {
        [SetUp]
        public void Init()
        {
            if (_packetsLoaded)
                return;


            PacketCapture c;
            GetPacketStatus status;
            var packetIndex = 0;
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ospfv2.pcap");
            dev.Open();

            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead)
            {
                var raw = c.GetPacket();
                var p = Packet.ParsePacket(raw.GetLinkLayers(), raw.Data).Extract<OspfV2Packet>();

                switch (packetIndex)
                {
                    case OspfHelloIndex:
                    {
                        _helloPacket = p;
                        break;
                    }
                    case OspfDbdIndex:
                    {
                        _ddPacket = p;
                        break;
                    }
                    case OspfDbdLsaIndex:
                    {
                        _ddLSAPacket = p;
                        break;
                    }
                    case OspfLsuIndex:
                    {
                        _lsuPacket = p;
                        break;
                    }
                    case OspfLsaIndex:
                    {
                        _lsaPacket = p;
                        break;
                    }
                    case OspfLsrIndex:
                    {
                        _lsrPacket = p;
                        break;
                    }
                    case OspfLsuLsaIndex:
                    {
                        _lsaHolder = (OspfV2LinkStateUpdatePacket) p;
                        break;
                    }
                }

                packetIndex++;
            }

            dev.Close();

            _packetsLoaded = true;
        }

        private const int OspfHelloIndex = 0;
        private const int OspfDbdIndex = 9;
        private const int OspfDbdLsaIndex = 11;
        private const int OspfLsrIndex = 17;
        private const int OspfLsuIndex = 18;
        private const int OspfLsaIndex = 23;
        private const int OspfLsuLsaIndex = 31;

        private OspfV2Packet _helloPacket;
        private OspfV2Packet _ddPacket;
        private OspfV2Packet _ddLSAPacket;
        private OspfV2Packet _lsrPacket;
        private OspfV2Packet _lsuPacket;
        private OspfV2Packet _lsaPacket;

        private OspfV2LinkStateUpdatePacket _lsaHolder;

        private bool _packetsLoaded;

        [Test]
        public void TestASExternalLSAConstruction()
        {
            //ctor 1
            var al = new ASExternalLinkAdvertisement
            {
                Age = 90,
                Options = 0x22,
                Type = LinkStateAdvertisementType.ASExternal,
                Id = System.Net.IPAddress.Parse("1.1.1.1"),
                AdvertisingRouter = System.Net.IPAddress.Parse("3.3.3.3"),
                SequenceNumber = 0x8000000F,
                Checksum = 0x3333,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.252")
            };

            ClassicAssert.AreEqual(90, al.Age);
            ClassicAssert.AreEqual(0x22, al.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.ASExternal, al.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("1.1.1.1"), al.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("3.3.3.3"), al.AdvertisingRouter);
            ClassicAssert.AreEqual(0x8000000F, al.SequenceNumber);
            ClassicAssert.AreEqual(0x3333, al.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.252"), al.NetworkMask);
            ClassicAssert.AreEqual(0, al.ASExternalLinks.Count);
            ClassicAssert.AreEqual(24, al.Length);

            //ctor 2;
            var aslink = new ASExternalLink
            {
                EBit = 1,
                Metric = 100,
                ExternalRouteTag = 0,
                TypeOfService = 0,
                ForwardingAddress = System.Net.IPAddress.Parse("0.0.0.0")
            };

            var links = new List<ASExternalLink> { aslink };

            al = new ASExternalLinkAdvertisement(links)
            {
                Age = 197,
                Options = 0x20,
                Type = LinkStateAdvertisementType.ASExternal,
                Id = System.Net.IPAddress.Parse("172.16.2.0"),
                AdvertisingRouter = System.Net.IPAddress.Parse("2.2.2.2"),
                SequenceNumber = 0x80000001,
                Checksum = 0x3356,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.0")
            };

            ClassicAssert.AreEqual(197, al.Age);
            ClassicAssert.AreEqual(0x20, al.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.ASExternal, al.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("172.16.2.0"), al.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("2.2.2.2"), al.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, al.SequenceNumber);
            ClassicAssert.AreEqual(0x3356, al.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), al.NetworkMask);
            ClassicAssert.AreEqual(1, al.ASExternalLinks.Count);
            ClassicAssert.AreEqual(36, al.Length);

            aslink = al.ASExternalLinks[0];
            ClassicAssert.AreEqual(1, aslink.EBit);
            ClassicAssert.AreEqual(100, aslink.Metric);
            ClassicAssert.AreEqual(0, aslink.ExternalRouteTag);
            ClassicAssert.AreEqual(0, aslink.TypeOfService);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("0.0.0.0"), aslink.ForwardingAddress);
        }

        [Test]
        public void TestDbdPacket()
        {
            ClassicAssert.IsNotNull(_ddPacket);
            ClassicAssert.AreEqual(OspfVersion.OspfV2, _ddPacket.Version);
            ClassicAssert.AreEqual(OspfPacketType.DatabaseDescription, _ddPacket.Type);
            ClassicAssert.AreEqual(_ddPacket is OspfV2DatabaseDescriptorPacket, true);
            ClassicAssert.AreEqual(0xa052, _ddPacket.Checksum);
            var dp = (OspfV2DatabaseDescriptorPacket) _ddPacket;
            ClassicAssert.AreEqual(1098361214, dp.DDSequence);
            ClassicAssert.AreEqual(0, dp.Headers.Count);
        }

        [Test]
        public void TestDDConstruction()
        {
            //test ctor 1
            var d = new OspfV2DatabaseDescriptorPacket { InterfaceMtu = 1500, DDSequence = 1098361214, DescriptionOptions = 0x02 };

            ClassicAssert.AreEqual(OspfPacketType.DatabaseDescription, d.Type);
            ClassicAssert.AreEqual(1098361214, d.DDSequence);
            ClassicAssert.AreEqual(1500, d.InterfaceMtu);
            ClassicAssert.AreEqual(0x02, d.DescriptionOptions);

            //test re-creation
            var bytes = d.Bytes;
            var dp = new OspfV2DatabaseDescriptorPacket(new ByteArraySegment(bytes));

            ClassicAssert.AreEqual(OspfPacketType.DatabaseDescription, d.Type);
            ClassicAssert.AreEqual(1098361214, dp.DDSequence);
            ClassicAssert.AreEqual(1500, dp.InterfaceMtu);
            ClassicAssert.AreEqual(0x02, dp.DescriptionOptions);

            //test ctor 2
            var lsas = new List<LinkStateAdvertisement>();

            var l = new LinkStateAdvertisement
            {
                AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.3"),
                Id = System.Net.IPAddress.Parse("192.168.170.3"),
                Age = 1,
                SequenceNumber = 0x80000001,
                Type = LinkStateAdvertisementType.Router,
                Options = 0x02
            };

            lsas.Add(l);

            l = new LinkStateAdvertisement
            {
                AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.2"),
                Id = System.Net.IPAddress.Parse("80.212.16.0"),
                Age = 2,
                SequenceNumber = 0x80000001,
                Type = LinkStateAdvertisementType.ASExternal,
                Options = 0x02
            };

            lsas.Add(l);

            l = new LinkStateAdvertisement
            {
                AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.2"),
                Id = System.Net.IPAddress.Parse("148.121.171.0"),
                Age = 2,
                SequenceNumber = 0x80000001,
                Type = LinkStateAdvertisementType.ASExternal,
                Options = 0x02
            };

            lsas.Add(l);

            var ddl = new OspfV2DatabaseDescriptorPacket(lsas) { InterfaceMtu = 1400, DDSequence = 123456789, DescriptionOptions = 0x03 };

            ClassicAssert.AreEqual(OspfPacketType.DatabaseDescription, ddl.Type);
            ClassicAssert.AreEqual(123456789, ddl.DDSequence);
            ClassicAssert.AreEqual(1400, ddl.InterfaceMtu);
            ClassicAssert.AreEqual(0x03, ddl.DescriptionOptions);

            ClassicAssert.AreEqual(3, ddl.Headers.Count);

            ClassicAssert.AreEqual(ddl.Headers[0].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(ddl.Headers[0].Id, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(ddl.Headers[0].Age, 1);
            ClassicAssert.AreEqual(ddl.Headers[0].SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(ddl.Headers[0].Type, LinkStateAdvertisementType.Router);
            ClassicAssert.AreEqual(ddl.Headers[0].Options, 0x02);

            ClassicAssert.AreEqual(ddl.Headers[1].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(ddl.Headers[1].Id, System.Net.IPAddress.Parse("80.212.16.0"));
            ClassicAssert.AreEqual(ddl.Headers[1].Age, 2);
            ClassicAssert.AreEqual(ddl.Headers[1].SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(ddl.Headers[1].Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(ddl.Headers[1].Options, 0x02);

            ClassicAssert.AreEqual(ddl.Headers[2].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(ddl.Headers[2].Id, System.Net.IPAddress.Parse("148.121.171.0"));
            ClassicAssert.AreEqual(ddl.Headers[2].Age, 2);
            ClassicAssert.AreEqual(ddl.Headers[2].SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(ddl.Headers[2].Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(ddl.Headers[2].Options, 0x02);
        }

        [Test]
        public void TestDDWithLSA()
        {
            ClassicAssert.IsNotNull(_ddLSAPacket);
            ClassicAssert.AreEqual(OspfVersion.OspfV2, _ddLSAPacket.Version);
            ClassicAssert.AreEqual(OspfPacketType.DatabaseDescription, _ddLSAPacket.Type);
            ClassicAssert.AreEqual(0xf067, _ddLSAPacket.Checksum);
            var dp = (OspfV2DatabaseDescriptorPacket) _ddLSAPacket;
            ClassicAssert.AreEqual(1098361214, dp.DDSequence);
            ClassicAssert.AreEqual(172, _ddLSAPacket.PacketLength);

            var lsas = dp.Headers;
            ClassicAssert.AreEqual(7, lsas.Count);
            var l = lsas[0];
            Console.WriteLine(l);
            ClassicAssert.AreEqual(l.Checksum, 0x3a9c);
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.Length, 48);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.Age, 1);
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.Router);
            ClassicAssert.AreEqual(l.Options, 0x02);

            l = lsas[1];
            ClassicAssert.AreEqual(l.Checksum, 0x2a49);
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.Length, 36);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("80.212.16.0"));
            ClassicAssert.AreEqual(l.Age, 2);
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Options, 0x02);

            l = lsas[2];
            ClassicAssert.AreEqual(l.Checksum, 0x34a5);
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.Length, 36);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("148.121.171.0"));
            ClassicAssert.AreEqual(l.Age, 2);
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Options, 0x02);

            l = lsas[3];
            ClassicAssert.AreEqual(l.Checksum, 0xd319);
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.Length, 36);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.130.120.0"));
            ClassicAssert.AreEqual(l.Age, 2);
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Options, 0x02);

            l = lsas[4];
            ClassicAssert.AreEqual(l.Checksum, 0x3708);
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.Length, 36);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.0.0"));
            ClassicAssert.AreEqual(l.Age, 2);
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Options, 0x02);

            l = lsas[5];
            ClassicAssert.AreEqual(l.Checksum, 0x2c12);
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.Length, 36);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.1.0"));
            ClassicAssert.AreEqual(l.Age, 2);
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Options, 0x02);

            l = lsas[6];
            ClassicAssert.AreEqual(l.Checksum, 0x3341);
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.Length, 36);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.172.0"));
            ClassicAssert.AreEqual(l.Age, 2);
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Options, 0x02);
        }

        [Test]
        public void TestHelloConstruction()
        {
            //test ctor 1
            var p = new OspfV2HelloPacket(System.Net.IPAddress.Parse("255.255.255.0"), 2, 2)
            {
                RouterId = System.Net.IPAddress.Parse("192.168.255.255"),
                AreaId = System.Net.IPAddress.Parse("192.168.255.252"),
                HelloOptions = 0x02,
                DesignatedRouterId = System.Net.IPAddress.Parse("192.168.1.1"),
                BackupRouterId = System.Net.IPAddress.Parse("10.1.1.2")
            };

            ClassicAssert.AreEqual(OspfVersion.OspfV2, p.Version);
            ClassicAssert.AreEqual(OspfPacketType.Hello, p.Type);
            ClassicAssert.AreEqual(0x02, p.HelloOptions);
            ClassicAssert.AreEqual(2, p.HelloInterval);
            ClassicAssert.AreEqual(2, p.RouterDeadInterval);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.255"), p.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.252"), p.AreaId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.1.1"), p.DesignatedRouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.1.1.2"), p.BackupRouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), p.NetworkMask);
            ClassicAssert.AreEqual(0, p.NeighborIds.Count);

            //test re-creation
            var bytes = p.Bytes;
            var hp = new OspfV2HelloPacket(new ByteArraySegment(bytes));

            ClassicAssert.AreEqual(OspfVersion.OspfV2, hp.Version);
            ClassicAssert.AreEqual(OspfPacketType.Hello, hp.Type);
            ClassicAssert.AreEqual(0x02, p.HelloOptions);
            ClassicAssert.AreEqual(2, hp.HelloInterval);
            ClassicAssert.AreEqual(2, hp.RouterDeadInterval);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.255"), hp.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.252"), hp.AreaId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.1.1"), hp.DesignatedRouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.1.1.2"), hp.BackupRouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), hp.NetworkMask);
            ClassicAssert.AreEqual(0, hp.NeighborIds.Count);

            //test ctor 2
            var neighbors = new List<System.Net.IPAddress>
            {
                System.Net.IPAddress.Parse("172.168.144.1"), System.Net.IPAddress.Parse("123.192.133.255"), System.Net.IPAddress.Parse("61.72.84.3"), System.Net.IPAddress.Parse("127.0.0.1")
            };

            var p2 = new OspfV2HelloPacket(System.Net.IPAddress.Parse("255.255.255.0"), 3, 4, neighbors);
            ClassicAssert.AreEqual(3, p2.HelloInterval);
            ClassicAssert.AreEqual(4, p2.RouterDeadInterval);
            ClassicAssert.AreEqual(4, p2.NeighborIds.Count);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("172.168.144.1"), p2.NeighborIds[0]);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("123.192.133.255"), p2.NeighborIds[1]);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("61.72.84.3"), p2.NeighborIds[2]);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("127.0.0.1"), p2.NeighborIds[3]);
        }

        [Test]
        public void TestHelloPacket()
        {
            ClassicAssert.IsNotNull(_helloPacket);
            ClassicAssert.AreEqual(_helloPacket is OspfV2HelloPacket, true);
            var hp = (OspfV2HelloPacket) _helloPacket;
            ClassicAssert.AreEqual(OspfVersion.OspfV2, _helloPacket.Version);
            ClassicAssert.AreEqual(OspfPacketType.Hello, _helloPacket.Type);
            ClassicAssert.AreEqual(0x273b, _helloPacket.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), hp.NetworkMask);
            ClassicAssert.AreEqual(0x02, hp.HelloOptions);
            ClassicAssert.AreEqual(0, hp.NeighborIds.Count);
        }

        [Test]
        public void TestLinkStateRequests()
        {
            var p = (OspfV2LinkStateRequestPacket) _lsrPacket;
            var requests = p.Requests;
            ClassicAssert.AreEqual(requests.Count, 7);

            var r = requests[0];
            ClassicAssert.AreEqual(r.LSType, LinkStateAdvertisementType.Router);
            ClassicAssert.AreEqual(r.LinkStateID, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(r.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));

            r = requests[1];
            ClassicAssert.AreEqual(r.LSType, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(r.LinkStateID, System.Net.IPAddress.Parse("80.212.16.0"));
            ClassicAssert.AreEqual(r.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));

            r = requests[2];
            ClassicAssert.AreEqual(r.LSType, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(r.LinkStateID, System.Net.IPAddress.Parse("148.121.171.0"));
            ClassicAssert.AreEqual(r.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));

            r = requests[3];
            ClassicAssert.AreEqual(r.LSType, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(r.LinkStateID, System.Net.IPAddress.Parse("192.130.120.0"));
            ClassicAssert.AreEqual(r.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));

            r = requests[4];
            ClassicAssert.AreEqual(r.LSType, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(r.LinkStateID, System.Net.IPAddress.Parse("192.168.0.0"));
            ClassicAssert.AreEqual(r.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));

            r = requests[5];
            ClassicAssert.AreEqual(r.LSType, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(r.LinkStateID, System.Net.IPAddress.Parse("192.168.1.0"));
            ClassicAssert.AreEqual(r.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));

            r = requests[6];
            ClassicAssert.AreEqual(r.LSType, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(r.LinkStateID, System.Net.IPAddress.Parse("192.168.172.0"));
            ClassicAssert.AreEqual(r.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
        }

        [Test]
        public void TestLSA()
        {
            ClassicAssert.AreNotEqual(null, _lsaHolder);
            ClassicAssert.AreEqual(11, _lsaHolder.LsaNumber);
            ClassicAssert.AreEqual(11, _lsaHolder.Updates.Count);

            var l = _lsaHolder.Updates[0];
            ClassicAssert.AreEqual(typeof(RouterLinksAdvertisement), l.GetType());
            var rl = (RouterLinksAdvertisement) l;
            ClassicAssert.AreEqual(446, rl.Age);
            ClassicAssert.AreEqual(0x22, rl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Router, rl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), rl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), rl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000004, rl.SequenceNumber);
            ClassicAssert.AreEqual(0x7caa, rl.Checksum);
            ClassicAssert.AreEqual(48, rl.Length);
            ClassicAssert.AreEqual(0, rl.VBit);
            ClassicAssert.AreEqual(0, rl.EBit);
            ClassicAssert.AreEqual(0, rl.BBit);
            ClassicAssert.AreEqual(2, rl.RouterLinks.Count);

            var rlink = rl.RouterLinks[0];
            ClassicAssert.AreEqual(3, rlink.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.20.0"), rlink.LinkId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), rlink.LinkData);
            ClassicAssert.AreEqual(0, rlink.TosNumber);
            ClassicAssert.AreEqual(10, rlink.Metric);

            rlink = rl.RouterLinks[1];
            ClassicAssert.AreEqual(2, rlink.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), rlink.LinkId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), rlink.LinkData);
            ClassicAssert.AreEqual(0, rlink.TosNumber);
            ClassicAssert.AreEqual(10, rlink.Metric);

            l = _lsaHolder.Updates[1];
            ClassicAssert.AreEqual(typeof(RouterLinksAdvertisement), l.GetType());
            rl = (RouterLinksAdvertisement) l;
            ClassicAssert.AreEqual(10, rl.Age);
            ClassicAssert.AreEqual(0x22, rl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Router, rl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), rl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), rl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000006, rl.SequenceNumber);
            ClassicAssert.AreEqual(0x36b1, rl.Checksum);
            ClassicAssert.AreEqual(36, rl.Length);
            ClassicAssert.AreEqual(0, rl.VBit);
            ClassicAssert.AreEqual(0, rl.EBit);
            ClassicAssert.AreEqual(1, rl.BBit);
            ClassicAssert.AreEqual(1, rl.RouterLinks.Count);

            rlink = rl.RouterLinks[0];
            ClassicAssert.AreEqual(3, rlink.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.0"), rlink.LinkId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.252"), rlink.LinkData);
            ClassicAssert.AreEqual(0, rlink.TosNumber);
            ClassicAssert.AreEqual(10, rlink.Metric);

            l = _lsaHolder.Updates[2];
            ClassicAssert.AreEqual(typeof(NetworkLinksAdvertisement), l.GetType());
            var nl = (NetworkLinksAdvertisement) l;
            ClassicAssert.AreEqual(446, nl.Age);
            ClassicAssert.AreEqual(0x22, nl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Network, nl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), nl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), nl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, nl.SequenceNumber);
            ClassicAssert.AreEqual(0xf6ed, nl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.252"), nl.NetworkMask);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), nl.AttachedRouters[0]);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), nl.AttachedRouters[1]);
            ClassicAssert.AreEqual(32, nl.Length);

            l = _lsaHolder.Updates[3];
            ClassicAssert.AreEqual(typeof(SummaryLinkAdvertisement), l.GetType());
            var sl = (SummaryLinkAdvertisement) l;
            ClassicAssert.AreEqual(11, sl.Age);
            ClassicAssert.AreEqual(0x22, sl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Summary, sl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.10.0"), sl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), sl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, sl.SequenceNumber);
            ClassicAssert.AreEqual(0x1e7d, sl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), sl.NetworkMask);
            ClassicAssert.AreEqual(30, sl.Metric);
            ClassicAssert.AreEqual(28, sl.Length);

            l = _lsaHolder.Updates[4];
            ClassicAssert.AreEqual(typeof(SummaryLinkAdvertisement), l.GetType());
            sl = (SummaryLinkAdvertisement) l;
            ClassicAssert.AreEqual(11, sl.Age);
            ClassicAssert.AreEqual(0x22, sl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Summary, sl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.10.0"), sl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), sl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, sl.SequenceNumber);
            ClassicAssert.AreEqual(0xd631, sl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.252"), sl.NetworkMask);
            ClassicAssert.AreEqual(20, sl.Metric);
            ClassicAssert.AreEqual(28, sl.Length);

            l = _lsaHolder.Updates[6];
            ClassicAssert.AreEqual(typeof(SummaryLinkAdvertisement), l.GetType());
            sl = (SummaryLinkAdvertisement) l;
            ClassicAssert.AreEqual(11, sl.Age);
            ClassicAssert.AreEqual(0x22, sl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.SummaryASBR, sl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("2.2.2.2"), sl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), sl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, sl.SequenceNumber);
            ClassicAssert.AreEqual(0x6fa0, sl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("0.0.0.0"), sl.NetworkMask);
            ClassicAssert.AreEqual(20, sl.Metric);
            ClassicAssert.AreEqual(28, sl.Length);

            l = _lsaHolder.Updates[7];
            ClassicAssert.AreEqual(typeof(ASExternalLinkAdvertisement), l.GetType());
            var al = (ASExternalLinkAdvertisement) l;
            ClassicAssert.AreEqual(197, al.Age);
            ClassicAssert.AreEqual(0x20, al.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.ASExternal, al.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("172.16.3.0"), al.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("2.2.2.2"), al.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, al.SequenceNumber);
            ClassicAssert.AreEqual(0x2860, al.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), al.NetworkMask);
            ClassicAssert.AreEqual(1, al.ASExternalLinks.Count);
            ClassicAssert.AreEqual(36, al.Length);

            var aslink = al.ASExternalLinks[0];
            ClassicAssert.AreEqual(aslink.EBit, 1);
            ClassicAssert.AreEqual(100, aslink.Metric);
            ClassicAssert.AreEqual(0, aslink.ExternalRouteTag);
            ClassicAssert.AreEqual(0, aslink.TypeOfService);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("0.0.0.0"), aslink.ForwardingAddress);

            l = _lsaHolder.Updates[8];
            ClassicAssert.AreEqual(typeof(ASExternalLinkAdvertisement), l.GetType());
            al = (ASExternalLinkAdvertisement) l;
            ClassicAssert.AreEqual(197, al.Age);
            ClassicAssert.AreEqual(0x20, al.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.ASExternal, al.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("172.16.2.0"), al.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("2.2.2.2"), al.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, al.SequenceNumber);
            ClassicAssert.AreEqual(0x3356, al.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), al.NetworkMask);
            ClassicAssert.AreEqual(1, al.ASExternalLinks.Count);
            ClassicAssert.AreEqual(36, al.Length);

            aslink = al.ASExternalLinks[0];
            ClassicAssert.AreEqual(aslink.EBit, 1);
            ClassicAssert.AreEqual(100, aslink.Metric);
            ClassicAssert.AreEqual(0, aslink.ExternalRouteTag);
            ClassicAssert.AreEqual(0, aslink.TypeOfService);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("0.0.0.0"), aslink.ForwardingAddress);
        }

        [Test]
        public void TestLSAPacket()
        {
            ClassicAssert.IsNotNull(_lsaPacket);
            ClassicAssert.AreEqual(OspfVersion.OspfV2, _lsaPacket.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateAcknowledgment, _lsaPacket.Type);
            ClassicAssert.AreEqual(0xe95e, _lsaPacket.Checksum);
            ClassicAssert.AreEqual(284, _lsaPacket.PacketLength);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("0.0.0.1"), _lsaPacket.AreaId);
            var ack = (OspfV2LinkStateAcknowledgmentPacket) _lsaPacket;
            ClassicAssert.AreEqual(13, ack.Acknowledgments.Count);

            var l = ack.Acknowledgments[0];
            ClassicAssert.AreEqual(l.Age, 2);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.Router);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x3a9c);
            ClassicAssert.AreEqual(l.Length, 48);

            l = ack.Acknowledgments[1];
            ClassicAssert.AreEqual(l.Age, 3);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("80.212.16.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x2a49);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[2];
            ClassicAssert.AreEqual(l.Age, 3);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("148.121.171.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x34a5);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[3];
            ClassicAssert.AreEqual(l.Age, 3);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.130.120.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0xd319);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[4];
            ClassicAssert.AreEqual(l.Age, 3);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.0.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x3708);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[5];
            ClassicAssert.AreEqual(l.Age, 3);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.1.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x2c12);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[6];
            ClassicAssert.AreEqual(l.Age, 3);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.172.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x3341);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[7];
            ClassicAssert.AreEqual(l.Age, 1);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("148.121.171.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x2eaa);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[8];
            ClassicAssert.AreEqual(l.Age, 1);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.130.120.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0xcd1e);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[9];
            ClassicAssert.AreEqual(l.Age, 1);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.0.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x310d);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[10];
            ClassicAssert.AreEqual(l.Age, 1);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.1.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x2617);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[11];
            ClassicAssert.AreEqual(l.Age, 1);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("192.168.172.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x2d46);
            ClassicAssert.AreEqual(l.Length, 36);

            l = ack.Acknowledgments[12];
            ClassicAssert.AreEqual(l.Age, 1);
            ClassicAssert.AreEqual(l.Options, 2);
            ClassicAssert.AreEqual(l.Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(l.Id, System.Net.IPAddress.Parse("80.212.16.0"));
            ClassicAssert.AreEqual(l.AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(l.SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(l.Checksum, 0x244e);
            ClassicAssert.AreEqual(l.Length, 36);
        }

        [Test]
        public void TestLSAPacketConstruction()
        {
            //test ctor 1
            var p = new OspfV2LinkStateAcknowledgmentPacket();

            ClassicAssert.AreEqual(OspfPacketType.LinkStateAcknowledgment, p.Type);

            //test re-creation
            var bytes = p.Bytes;
            var p2 = new OspfV2DatabaseDescriptorPacket(new ByteArraySegment(bytes));

            ClassicAssert.AreEqual(OspfPacketType.LinkStateAcknowledgment, p2.Type);

            //test ctor 2
            var lsas = new List<LinkStateAdvertisement>();
            var l = new LinkStateAdvertisement
            {
                AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.3"),
                Id = System.Net.IPAddress.Parse("192.168.170.3"),
                Age = 1,
                SequenceNumber = 0x80000001,
                Type = LinkStateAdvertisementType.Router,
                Options = 0x02
            };

            lsas.Add(l);

            l = new LinkStateAdvertisement
            {
                AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.2"),
                Id = System.Net.IPAddress.Parse("80.212.16.0"),
                Age = 2,
                SequenceNumber = 0x80000001,
                Type = LinkStateAdvertisementType.ASExternal,
                Options = 0x02
            };

            lsas.Add(l);

            l = new LinkStateAdvertisement
            {
                AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.2"),
                Id = System.Net.IPAddress.Parse("148.121.171.0"),
                Age = 2,
                SequenceNumber = 0x80000001,
                Type = LinkStateAdvertisementType.ASExternal,
                Options = 0x02
            };

            lsas.Add(l);

            var p3 = new OspfV2LinkStateAcknowledgmentPacket(lsas);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateAcknowledgment, p3.Type);

            ClassicAssert.AreEqual(3, p3.Acknowledgments.Count);

            ClassicAssert.AreEqual(p3.Acknowledgments[0].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(p3.Acknowledgments[0].Id, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(p3.Acknowledgments[0].Age, 1);
            ClassicAssert.AreEqual(p3.Acknowledgments[0].SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(p3.Acknowledgments[0].Type, LinkStateAdvertisementType.Router);
            ClassicAssert.AreEqual(p3.Acknowledgments[0].Options, 0x02);

            ClassicAssert.AreEqual(p3.Acknowledgments[1].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(p3.Acknowledgments[1].Id, System.Net.IPAddress.Parse("80.212.16.0"));
            ClassicAssert.AreEqual(p3.Acknowledgments[1].Age, 2);
            ClassicAssert.AreEqual(p3.Acknowledgments[1].SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(p3.Acknowledgments[1].Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(p3.Acknowledgments[1].Options, 0x02);

            ClassicAssert.AreEqual(p3.Acknowledgments[2].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
            ClassicAssert.AreEqual(p3.Acknowledgments[2].Id, System.Net.IPAddress.Parse("148.121.171.0"));
            ClassicAssert.AreEqual(p3.Acknowledgments[2].Age, 2);
            ClassicAssert.AreEqual(p3.Acknowledgments[2].SequenceNumber, 0x80000001);
            ClassicAssert.AreEqual(p3.Acknowledgments[2].Type, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(p3.Acknowledgments[2].Options, 0x02);
        }

        [Test]
        public void TestLSRConstruction()
        {
            //test ctor 1
            var p = new OspfV2LinkStateRequestPacket { RouterId = System.Net.IPAddress.Parse("192.168.255.255"), AreaId = System.Net.IPAddress.Parse("192.168.255.252") };

            ClassicAssert.AreEqual(OspfVersion.OspfV2, p.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateRequest, p.Type);

            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.255"), p.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.252"), p.AreaId);

            //test re-creation
            var bytes = p.Bytes;
            var lp = new OspfV2LinkStateRequestPacket(new ByteArraySegment(bytes));

            ClassicAssert.AreEqual(OspfVersion.OspfV2, lp.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateRequest, lp.Type);

            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.255"), lp.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.252"), lp.AreaId);

            //test ctor 2
            var lsrs = new List<LinkStateRequest>();

            var r = new LinkStateRequest
            {
                LSType = LinkStateAdvertisementType.Router, LinkStateID = System.Net.IPAddress.Parse("192.168.170.3"), AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.3")
            };

            lsrs.Add(r);

            r = new LinkStateRequest
            {
                LSType = LinkStateAdvertisementType.ASExternal, LinkStateID = System.Net.IPAddress.Parse("80.212.16.0"), AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.2")
            };

            lsrs.Add(r);

            r = new LinkStateRequest
            {
                LSType = LinkStateAdvertisementType.Network, LinkStateID = System.Net.IPAddress.Parse("148.121.171.0"), AdvertisingRouter = System.Net.IPAddress.Parse("192.168.170.2")
            };

            lsrs.Add(r);

            var lp2 = new OspfV2LinkStateRequestPacket(lsrs) { RouterId = System.Net.IPAddress.Parse("10.0.1.255"), AreaId = System.Net.IPAddress.Parse("10.0.2.252") };

            ClassicAssert.AreEqual(OspfVersion.OspfV2, lp2.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateRequest, lp2.Type);

            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.1.255"), lp2.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.2.252"), lp2.AreaId);

            ClassicAssert.AreEqual(lp2.Requests[0].LSType, LinkStateAdvertisementType.Router);
            ClassicAssert.AreEqual(lp2.Requests[0].LinkStateID, System.Net.IPAddress.Parse("192.168.170.3"));
            ClassicAssert.AreEqual(lp2.Requests[0].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.3"));

            ClassicAssert.AreEqual(lp2.Requests[1].LSType, LinkStateAdvertisementType.ASExternal);
            ClassicAssert.AreEqual(lp2.Requests[1].LinkStateID, System.Net.IPAddress.Parse("80.212.16.0"));
            ClassicAssert.AreEqual(lp2.Requests[1].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));

            ClassicAssert.AreEqual(lp2.Requests[2].LSType, LinkStateAdvertisementType.Network);
            ClassicAssert.AreEqual(lp2.Requests[2].LinkStateID, System.Net.IPAddress.Parse("148.121.171.0"));
            ClassicAssert.AreEqual(lp2.Requests[2].AdvertisingRouter, System.Net.IPAddress.Parse("192.168.170.2"));
        }

        [Test]
        public void TestLSRPacket()
        {
            OspfV2LinkStateRequestPacket lp;
            ClassicAssert.IsNotNull(_lsrPacket);
            ClassicAssert.AreEqual(OspfVersion.OspfV2, _lsrPacket.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateRequest, _lsrPacket.Type);
            ClassicAssert.AreEqual(0x7595, _lsrPacket.Checksum);
            ClassicAssert.AreEqual(_lsrPacket is OspfV2LinkStateRequestPacket, true);
            lp = (OspfV2LinkStateRequestPacket) _lsrPacket;
            ClassicAssert.AreEqual(7, lp.Requests.Count);
        }

        [Test]
        public void TestLsuConstruction()
        {
            //test ctor 1
            var p = new OspfV2LinkStateUpdatePacket { RouterId = System.Net.IPAddress.Parse("192.168.255.255"), AreaId = System.Net.IPAddress.Parse("192.168.255.252") };

            ClassicAssert.AreEqual(OspfVersion.OspfV2, p.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateUpdate, p.Type);

            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.255"), p.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.252"), p.AreaId);

            ClassicAssert.AreEqual(0, p.LsaNumber);

            //test re-creation
            var bytes = p.Bytes;
            var lp = new OspfV2LinkStateUpdatePacket(new ByteArraySegment(bytes));

            ClassicAssert.AreEqual(OspfVersion.OspfV2, lp.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateUpdate, lp.Type);

            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.255"), lp.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.252"), lp.AreaId);
            ClassicAssert.AreEqual(0, p.LsaNumber);

            //ctor 2
            //add routerer LSA
            var rlist = new List<RouterLink>();

            var rlink = new RouterLink
            {
                Type = 3,
                LinkId = System.Net.IPAddress.Parse("192.168.20.0"),
                LinkData = System.Net.IPAddress.Parse("255.255.255.0"),
                TosNumber = 0,
                Metric = 10
            };

            rlist.Add(rlink);

            rlink = new RouterLink
            {
                Type = 2,
                LinkId = System.Net.IPAddress.Parse("10.0.20.2"),
                LinkData = System.Net.IPAddress.Parse("10.0.20.2"),
                TosNumber = 0,
                Metric = 10
            };

            rlist.Add(rlink);
            var rl = new RouterLinksAdvertisement(rlist)
            {
                Age = 446,
                Options = 0x22,
                Id = System.Net.IPAddress.Parse("5.5.5.5"),
                AdvertisingRouter = System.Net.IPAddress.Parse("5.5.5.5"),
                SequenceNumber = 0x80000004,
                Checksum = 0x7caa,
                VBit = 0,
                EBit = 0,
                BBit = 0
            };

            //add network lsa
            var rtrs = new List<System.Net.IPAddress> { System.Net.IPAddress.Parse("5.5.5.5"), System.Net.IPAddress.Parse("4.4.4.4") };

            var nl = new NetworkLinksAdvertisement(rtrs)
            {
                Age = 446,
                Options = 0x22,
                Type = LinkStateAdvertisementType.Network,
                Id = System.Net.IPAddress.Parse("10.0.20.2"),
                AdvertisingRouter = System.Net.IPAddress.Parse("5.5.5.5"),
                SequenceNumber = 0x80000001,
                Checksum = 0xf6ed,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.252")
            };

            //add summary lsa
            var tms = new List<TypeOfServiceMetric>();

            var tm = new TypeOfServiceMetric { TypeOfService = 1, Metric = 11 };
            tms.Add(tm);

            tm = new TypeOfServiceMetric { TypeOfService = 2, Metric = 22 };
            tms.Add(tm);

            var sl = new SummaryLinkAdvertisement(tms)
            {
                Age = 11,
                Options = 0x22,
                Type = LinkStateAdvertisementType.Summary,
                Id = System.Net.IPAddress.Parse("192.168.10.0"),
                AdvertisingRouter = System.Net.IPAddress.Parse("4.4.4.4"),
                SequenceNumber = 0x80000001,
                Checksum = 0x1e7d,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.0"),
                Metric = 30
            };

            //add AS External LSA
            var aslink = new ASExternalLink
            {
                EBit = 1,
                Metric = 100,
                ExternalRouteTag = 0,
                TypeOfService = 0,
                ForwardingAddress = System.Net.IPAddress.Parse("0.0.0.0")
            };

            var links = new List<ASExternalLink> { aslink };

            var al = new ASExternalLinkAdvertisement(links)
            {
                Age = 197,
                Options = 0x20,
                Type = LinkStateAdvertisementType.ASExternal,
                Id = System.Net.IPAddress.Parse("172.16.2.0"),
                AdvertisingRouter = System.Net.IPAddress.Parse("2.2.2.2"),
                SequenceNumber = 0x80000001,
                Checksum = 0x3356,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.0")
            };

            //put them all together in a list
            var lsas = new List<LinkStateAdvertisement> { rl, nl, sl, al };

            p = new OspfV2LinkStateUpdatePacket(lsas) { RouterId = System.Net.IPAddress.Parse("192.168.255.255"), AreaId = System.Net.IPAddress.Parse("192.168.255.252") };

            ClassicAssert.AreEqual(OspfVersion.OspfV2, p.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateUpdate, p.Type);

            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.255"), p.RouterId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.255.252"), p.AreaId);

            ClassicAssert.AreEqual(4, p.LsaNumber);

            //test router lsa
            rl = (RouterLinksAdvertisement) p.Updates[0];
            ClassicAssert.AreEqual(446, rl.Age);
            ClassicAssert.AreEqual(0x22, rl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Router, rl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), rl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), rl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000004, rl.SequenceNumber);
            ClassicAssert.AreEqual(0x7caa, rl.Checksum);
            ClassicAssert.AreEqual(48, rl.Length);
            ClassicAssert.AreEqual(0, rl.VBit);
            ClassicAssert.AreEqual(0, rl.EBit);
            ClassicAssert.AreEqual(0, rl.BBit);
            ClassicAssert.AreEqual(2, rl.RouterLinks.Count);

            rlink = rl.RouterLinks[0];
            ClassicAssert.AreEqual(3, rlink.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.20.0"), rlink.LinkId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), rlink.LinkData);
            ClassicAssert.AreEqual(0, rlink.TosNumber);
            ClassicAssert.AreEqual(10, rlink.Metric);

            rlink = rl.RouterLinks[1];
            ClassicAssert.AreEqual(2, rlink.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), rlink.LinkId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), rlink.LinkData);
            ClassicAssert.AreEqual(0, rlink.TosNumber);
            ClassicAssert.AreEqual(10, rlink.Metric);

            //test network lsa
            nl = (NetworkLinksAdvertisement) p.Updates[1];
            ClassicAssert.AreEqual(446, nl.Age);
            ClassicAssert.AreEqual(0x22, nl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Network, nl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), nl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), nl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, nl.SequenceNumber);
            ClassicAssert.AreEqual(0xf6ed, nl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.252"), nl.NetworkMask);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), nl.AttachedRouters[0]);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), nl.AttachedRouters[1]);
            ClassicAssert.AreEqual(32, nl.Length);

            //test summary lsa
            sl = (SummaryLinkAdvertisement) p.Updates[2];
            ClassicAssert.AreEqual(11, sl.Age);
            ClassicAssert.AreEqual(0x22, sl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Summary, sl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.10.0"), sl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), sl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, sl.SequenceNumber);
            ClassicAssert.AreEqual(0x1e7d, sl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), sl.NetworkMask);
            ClassicAssert.AreEqual(30, sl.Metric);
            ClassicAssert.AreEqual(36, sl.Length);
            ClassicAssert.AreEqual(2, sl.TosMetrics.Count);

            ClassicAssert.AreEqual(1, sl.TosMetrics[0].TypeOfService);
            ClassicAssert.AreEqual(11, sl.TosMetrics[0].Metric);
            ClassicAssert.AreEqual(2, sl.TosMetrics[1].TypeOfService);
            ClassicAssert.AreEqual(22, sl.TosMetrics[1].Metric);

            //test AS-External-LSA
            al = (ASExternalLinkAdvertisement) p.Updates[3];
            ClassicAssert.AreEqual(197, al.Age);
            ClassicAssert.AreEqual(0x20, al.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.ASExternal, al.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("172.16.2.0"), al.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("2.2.2.2"), al.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, al.SequenceNumber);
            ClassicAssert.AreEqual(0x3356, al.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), al.NetworkMask);
            ClassicAssert.AreEqual(1, al.ASExternalLinks.Count);
            ClassicAssert.AreEqual(36, al.Length);

            aslink = al.ASExternalLinks[0];
            ClassicAssert.AreEqual(1, aslink.EBit);
            ClassicAssert.AreEqual(100, aslink.Metric);
            ClassicAssert.AreEqual(0, aslink.ExternalRouteTag);
            ClassicAssert.AreEqual(0, aslink.TypeOfService);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("0.0.0.0"), aslink.ForwardingAddress);
        }

        [Test]
        public void TestLsuPacket()
        {
            OspfV2LinkStateUpdatePacket lp;
            ClassicAssert.IsNotNull(_lsuPacket);
            ClassicAssert.AreEqual(OspfVersion.OspfV2, _lsuPacket.Version);
            ClassicAssert.AreEqual(OspfPacketType.LinkStateUpdate, _lsuPacket.Type);
            ClassicAssert.AreEqual(0x961f, _lsuPacket.Checksum);
            ClassicAssert.AreEqual(_lsuPacket is OspfV2LinkStateUpdatePacket, true);
            lp = (OspfV2LinkStateUpdatePacket) _lsuPacket;
            ClassicAssert.AreEqual(1, lp.LsaNumber);
            var l = lp.Updates;
            ClassicAssert.AreEqual(1, l.Count);
            ClassicAssert.AreEqual(typeof(RouterLinksAdvertisement), l[0].GetType());
            Console.WriteLine(l[0]);
        }

        [Test]
        public void TestNetworkLSACreation()
        {
            //ctor 1
            var nl = new NetworkLinksAdvertisement
            {
                Age = 333,
                Options = 0x20,
                Type = LinkStateAdvertisementType.Network,
                Id = System.Net.IPAddress.Parse("1.1.1.1"),
                AdvertisingRouter = System.Net.IPAddress.Parse("2.2.2.2"),
                SequenceNumber = 0x8000000F,
                Checksum = 0xdede,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.252")
            };

            ClassicAssert.AreEqual(333, nl.Age);
            ClassicAssert.AreEqual(0x20, nl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Network, nl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("1.1.1.1"), nl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("2.2.2.2"), nl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x8000000F, nl.SequenceNumber);
            ClassicAssert.AreEqual(0xdede, nl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.252"), nl.NetworkMask);
            ClassicAssert.AreEqual(24, nl.Length);

            //ctor 2
            var rtrs = new List<System.Net.IPAddress> { System.Net.IPAddress.Parse("5.5.5.5"), System.Net.IPAddress.Parse("4.4.4.4") };

            nl = new NetworkLinksAdvertisement(rtrs)
            {
                Age = 446,
                Options = 0x22,
                Type = LinkStateAdvertisementType.Network,
                Id = System.Net.IPAddress.Parse("10.0.20.2"),
                AdvertisingRouter = System.Net.IPAddress.Parse("5.5.5.5"),
                SequenceNumber = 0x80000001,
                Checksum = 0xf6ed,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.252")
            };

            ClassicAssert.AreEqual(446, nl.Age);
            ClassicAssert.AreEqual(0x22, nl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Network, nl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), nl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), nl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, nl.SequenceNumber);
            ClassicAssert.AreEqual(0xf6ed, nl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.252"), nl.NetworkMask);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), nl.AttachedRouters[0]);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), nl.AttachedRouters[1]);
            ClassicAssert.AreEqual(32, nl.Length);
        }

        [Test]
        public void TestOspfv2Auth()
        {
            PacketCapture c;
            GetPacketStatus status;
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "ospfv2_md5.pcap");
            var testSubjects = new OspfV2HelloPacket[4];
            var i = 0;

            dev.Open();
            while ((status = dev.GetNextPacket(out c)) == GetPacketStatus.PacketRead && i < 4)
            {
                var raw = c.GetPacket();
                testSubjects[i] = Packet.ParsePacket(raw.GetLinkLayers(), raw.Data).Extract<OspfV2HelloPacket>();
                i++;
            }

            dev.Close();

            ClassicAssert.AreEqual(testSubjects[0].Authentication, 0x000000103c7ec4f7);
            ClassicAssert.AreEqual(testSubjects[1].Authentication, 0x000000103c7ec4fc);
            ClassicAssert.AreEqual(testSubjects[2].Authentication, 0x000000103c7ec501);
            ClassicAssert.AreEqual(testSubjects[3].Authentication, 0x000000103c7ec505);
            ClassicAssert.AreEqual(0, testSubjects[0].NeighborIds.Count);
            ClassicAssert.AreEqual(0, testSubjects[1].NeighborIds.Count);
            ClassicAssert.AreEqual(1, testSubjects[2].NeighborIds.Count);
            ClassicAssert.AreEqual(1, testSubjects[3].NeighborIds.Count);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.0.2"), testSubjects[2].NeighborIds[0]);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.0.1"), testSubjects[3].NeighborIds[0]);
        }

        [Test]
        public void TestRouterLSAConstruction()
        {
            //ctor 1
            var rl = new RouterLinksAdvertisement
            {
                Age = 333,
                Options = 0x20,
                Id = System.Net.IPAddress.Parse("1.1.1.1"),
                AdvertisingRouter = System.Net.IPAddress.Parse("2.2.2.2"),
                SequenceNumber = 0x80000001,
                Checksum = 0xaaaa,
                VBit = 1,
                EBit = 0,
                BBit = 1
            };

            ClassicAssert.AreEqual(333, rl.Age);
            ClassicAssert.AreEqual(0x20, rl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Router, rl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("1.1.1.1"), rl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("2.2.2.2"), rl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, rl.SequenceNumber);
            ClassicAssert.AreEqual(0xaaaa, rl.Checksum);
            ClassicAssert.AreEqual(24, rl.Length);
            ClassicAssert.AreEqual(1, rl.VBit);
            ClassicAssert.AreEqual(0, rl.EBit);
            ClassicAssert.AreEqual(1, rl.BBit);
            ClassicAssert.AreEqual(0, rl.RouterLinks.Count);

            var rlist = new List<RouterLink>();

            var rlink = new RouterLink
            {
                Type = 3,
                LinkId = System.Net.IPAddress.Parse("192.168.20.0"),
                LinkData = System.Net.IPAddress.Parse("255.255.255.0"),
                TosNumber = 0,
                Metric = 10
            };

            rlist.Add(rlink);

            rlink = new RouterLink
            {
                Type = 2,
                LinkId = System.Net.IPAddress.Parse("10.0.20.2"),
                LinkData = System.Net.IPAddress.Parse("10.0.20.2"),
                TosNumber = 0,
                Metric = 10
            };

            rlist.Add(rlink);

            //ctor 2
            rl = new RouterLinksAdvertisement(rlist)
            {
                Age = 446,
                Options = 0x22,
                Id = System.Net.IPAddress.Parse("5.5.5.5"),
                AdvertisingRouter = System.Net.IPAddress.Parse("5.5.5.5"),
                SequenceNumber = 0x80000004,
                Checksum = 0x7caa,
                VBit = 0,
                EBit = 0,
                BBit = 0
            };

            ClassicAssert.AreEqual(446, rl.Age);
            ClassicAssert.AreEqual(0x22, rl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Router, rl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), rl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("5.5.5.5"), rl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000004, rl.SequenceNumber);
            ClassicAssert.AreEqual(0x7caa, rl.Checksum);
            ClassicAssert.AreEqual(48, rl.Length);
            ClassicAssert.AreEqual(0, rl.VBit);
            ClassicAssert.AreEqual(0, rl.EBit);
            ClassicAssert.AreEqual(0, rl.BBit);
            ClassicAssert.AreEqual(2, rl.RouterLinks.Count);

            rlink = rl.RouterLinks[0];
            ClassicAssert.AreEqual(3, rlink.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.20.0"), rlink.LinkId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), rlink.LinkData);
            ClassicAssert.AreEqual(0, rlink.TosNumber);
            ClassicAssert.AreEqual(10, rlink.Metric);

            rlink = rl.RouterLinks[1];
            ClassicAssert.AreEqual(2, rlink.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), rlink.LinkId);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("10.0.20.2"), rlink.LinkData);
            ClassicAssert.AreEqual(0, rlink.TosNumber);
            ClassicAssert.AreEqual(10, rlink.Metric);

            //re-creation
        }

        [Test]
        public void TestSummaryLSAConstruction()
        {
            //ctor 1
            var sl = new SummaryLinkAdvertisement
            {
                Age = 22,
                Options = 0x20,
                Type = LinkStateAdvertisementType.Summary,
                Id = System.Net.IPAddress.Parse("1.1.1.1"),
                AdvertisingRouter = System.Net.IPAddress.Parse("4.4.4.4"),
                SequenceNumber = 0x8000000F,
                Checksum = 0xdddd,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.0"),
                Metric = 10
            };

            ClassicAssert.AreEqual(22, sl.Age);
            ClassicAssert.AreEqual(0x20, sl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Summary, sl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("1.1.1.1"), sl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), sl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x8000000F, sl.SequenceNumber);
            ClassicAssert.AreEqual(0xdddd, sl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), sl.NetworkMask);
            ClassicAssert.AreEqual(10, sl.Metric);
            ClassicAssert.AreEqual(28, sl.Length);
            ClassicAssert.AreEqual(0, sl.TosMetrics.Count);

            var tms = new List<TypeOfServiceMetric>();

            var tm = new TypeOfServiceMetric { TypeOfService = 1, Metric = 11 };
            tms.Add(tm);

            tm = new TypeOfServiceMetric { TypeOfService = 2, Metric = 22 };
            tms.Add(tm);

            //ctor 2
            sl = new SummaryLinkAdvertisement(tms)
            {
                Age = 11,
                Options = 0x22,
                Type = LinkStateAdvertisementType.Summary,
                Id = System.Net.IPAddress.Parse("192.168.10.0"),
                AdvertisingRouter = System.Net.IPAddress.Parse("4.4.4.4"),
                SequenceNumber = 0x80000001,
                Checksum = 0x1e7d,
                NetworkMask = System.Net.IPAddress.Parse("255.255.255.0"),
                Metric = 30
            };

            ClassicAssert.AreEqual(11, sl.Age);
            ClassicAssert.AreEqual(0x22, sl.Options);
            ClassicAssert.AreEqual(LinkStateAdvertisementType.Summary, sl.Type);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("192.168.10.0"), sl.Id);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("4.4.4.4"), sl.AdvertisingRouter);
            ClassicAssert.AreEqual(0x80000001, sl.SequenceNumber);
            ClassicAssert.AreEqual(0x1e7d, sl.Checksum);
            ClassicAssert.AreEqual(System.Net.IPAddress.Parse("255.255.255.0"), sl.NetworkMask);
            ClassicAssert.AreEqual(30, sl.Metric);
            ClassicAssert.AreEqual(36, sl.Length);
            ClassicAssert.AreEqual(2, sl.TosMetrics.Count);

            ClassicAssert.AreEqual(1, sl.TosMetrics[0].TypeOfService);
            ClassicAssert.AreEqual(11, sl.TosMetrics[0].Metric);
            ClassicAssert.AreEqual(2, sl.TosMetrics[1].TypeOfService);
            ClassicAssert.AreEqual(22, sl.TosMetrics[1].Metric);
        }
    }