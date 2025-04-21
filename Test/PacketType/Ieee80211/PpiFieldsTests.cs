using System.IO;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211;

    [TestFixture]
    public class PpiFieldsTests
    {
        [Test]
        public void Test_Ppi802_3_Construction()
        {
            var field = new Ppi8023(Ppi8023.StandardFlags.FcsPresent,
                                    Ppi8023.ErrorFlags.InvalidFcs | Ppi8023.ErrorFlags.SymbolError);

            var recreatedField = new Ppi8023(new BinaryReader(new MemoryStream(field.Bytes)));

            ClassicAssert.AreEqual(Ppi8023.StandardFlags.FcsPresent, recreatedField.Flags);
            ClassicAssert.AreEqual(Ppi8023.ErrorFlags.InvalidFcs | Ppi8023.ErrorFlags.SymbolError, recreatedField.Errors);
        }

        [Test]
        public void Test_PpiAggregation_Construction()
        {
            var field = new PpiAggregation(22);
            ClassicAssert.AreEqual(22, field.InterfaceId);

            var recreatedField = new PpiAggregation(new BinaryReader(new MemoryStream(field.Bytes)));

            ClassicAssert.AreEqual(22, recreatedField.InterfaceId);
        }

        [Test]
        public void Test_PpiCaptureInfo_Construction()
        {
            var field = new PpiCaptureInfo();

            ClassicAssert.AreEqual(0, field.Bytes.Length);
        }

        [Test]
        public void Test_PpiCommon_Construction()
        {
            var field = new PpiCommon
            {
                TSFTimer = 0x1234567812345678,
                Flags = PpiCommon.CommonFlags.FcsIncludedInFrame | PpiCommon.CommonFlags.TimerSynchFunctionInUse,
                Rate = 2,
                ChannelFrequency = 2142,
                ChannelFlags = RadioTapChannelFlags.Channel2Ghz | RadioTapChannelFlags.Passive,
                FhssHopset = 0xAB,
                FhssPattern = 0xCD,
                AntennaSignalPower = -50,
                AntennaSignalNoise = 25
            };

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiCommon(new BinaryReader(ms));

            ClassicAssert.AreEqual(0x1234567812345678, recreatedField.TSFTimer);
            ClassicAssert.AreEqual(PpiCommon.CommonFlags.FcsIncludedInFrame | PpiCommon.CommonFlags.TimerSynchFunctionInUse, recreatedField.Flags);
            ClassicAssert.AreEqual(2, recreatedField.Rate);
            ClassicAssert.AreEqual(2142, recreatedField.ChannelFrequency);
            ClassicAssert.AreEqual(RadioTapChannelFlags.Channel2Ghz | RadioTapChannelFlags.Passive, recreatedField.ChannelFlags);
            ClassicAssert.AreEqual(0xAB, recreatedField.FhssHopset);
            ClassicAssert.AreEqual(0xCD, recreatedField.FhssPattern);
            ClassicAssert.AreEqual(-50, recreatedField.AntennaSignalPower);
            ClassicAssert.AreEqual(25, recreatedField.AntennaSignalNoise);
        }

        [Test]
        public void Test_PpiMacExtensions_Construction()
        {
            var field = new PpiMacExtensions { Flags = PpiMacExtensionFlags.DuplicateRx | PpiMacExtensionFlags.HtIndicator, AMpduId = 0x12345678, DelimiterCount = 0xA };

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiMacExtensions(new BinaryReader(ms));

            ClassicAssert.AreEqual(PpiMacExtensionFlags.DuplicateRx | PpiMacExtensionFlags.HtIndicator, recreatedField.Flags);
            ClassicAssert.AreEqual(0x12345678, field.AMpduId);
            ClassicAssert.AreEqual(0xA, field.DelimiterCount);
        }

        [Test]
        public void Test_PpiMacPhy_Construction()
        {
            var field = new PpiMacPhy
            {
                AMpduId = 0x12345678,
                DelimiterCount = 0xAB,
                ModulationCodingScheme = 0x1,
                SpatialStreamCount = 0x2,
                RssiCombined = 0x3,
                RssiAntenna0Control = 0x4,
                RssiAntenna1Control = 0x5,
                RssiAntenna2Control = 0x6,
                RssiAntenna3Control = 0x7,
                RssiAntenna0Ext = 0x8,
                RssiAntenna1Ext = 0x9,
                RssiAntenna2Ext = 0xA,
                RssiAntenna3Ext = 0xB,
                ExtensionChannelFrequency = 2142,
                ExtensionChannelFlags = RadioTapChannelFlags.Channel5Ghz | RadioTapChannelFlags.Passive,
                DbmAntenna0SignalPower = 0xC,
                DbmAntenna0SignalNoise = 0xD,
                DbmAntenna1SignalPower = 0xE,
                DbmAntenna1SignalNoise = 0xF,
                DbmAntenna2SignalPower = 0x1,
                DbmAntenna2SignalNoise = 0x2,
                DbmAntenna3SignalPower = 0x3,
                DbmAntenna3SignalNoise = 0x4,
                ErrorVectorMagnitude0 = 0xAAAAAAAA,
                ErrorVectorMagnitude1 = 0xBBBBBBBB,
                ErrorVectorMagnitude2 = 0xCCCCCCCC,
                ErrorVectorMagnitude3 = 0xDDDDDDDD
            };

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiMacPhy(new BinaryReader(ms));

            ClassicAssert.AreEqual(0x12345678, recreatedField.AMpduId);
            ClassicAssert.AreEqual(0xAB, recreatedField.DelimiterCount);
            ClassicAssert.AreEqual(0x1, recreatedField.ModulationCodingScheme);
            ClassicAssert.AreEqual(0x2, recreatedField.SpatialStreamCount);
            ClassicAssert.AreEqual(0x3, recreatedField.RssiCombined);
            ClassicAssert.AreEqual(0x4, recreatedField.RssiAntenna0Control);
            ClassicAssert.AreEqual(0x5, recreatedField.RssiAntenna1Control);
            ClassicAssert.AreEqual(0x6, recreatedField.RssiAntenna2Control);
            ClassicAssert.AreEqual(0x7, recreatedField.RssiAntenna3Control);
            ClassicAssert.AreEqual(0x8, recreatedField.RssiAntenna0Ext);
            ClassicAssert.AreEqual(0x9, recreatedField.RssiAntenna1Ext);
            ClassicAssert.AreEqual(0xA, recreatedField.RssiAntenna2Ext);
            ClassicAssert.AreEqual(0xB, recreatedField.RssiAntenna3Ext);
            ClassicAssert.AreEqual(2142, recreatedField.ExtensionChannelFrequency);
            ClassicAssert.AreEqual(RadioTapChannelFlags.Channel5Ghz | RadioTapChannelFlags.Passive, recreatedField.ExtensionChannelFlags);
            ClassicAssert.AreEqual(0xC, recreatedField.DbmAntenna0SignalPower);
            ClassicAssert.AreEqual(0xD, recreatedField.DbmAntenna0SignalNoise);
            ClassicAssert.AreEqual(0xE, recreatedField.DbmAntenna1SignalPower);
            ClassicAssert.AreEqual(0xF, recreatedField.DbmAntenna1SignalNoise);
            ClassicAssert.AreEqual(0x1, recreatedField.DbmAntenna2SignalPower);
            ClassicAssert.AreEqual(0x2, recreatedField.DbmAntenna2SignalNoise);
            ClassicAssert.AreEqual(0x3, recreatedField.DbmAntenna3SignalPower);
            ClassicAssert.AreEqual(0x4, recreatedField.DbmAntenna3SignalNoise);
            ClassicAssert.AreEqual(0xAAAAAAAA, recreatedField.ErrorVectorMagnitude0);
            ClassicAssert.AreEqual(0xBBBBBBBB, recreatedField.ErrorVectorMagnitude1);
            ClassicAssert.AreEqual(0xCCCCCCCC, recreatedField.ErrorVectorMagnitude2);
            ClassicAssert.AreEqual(0xDDDDDDDD, recreatedField.ErrorVectorMagnitude3);
        }

        [Test]
        public void Test_PpiProcessInfo_Construction()
        {
            var field = new PpiProcessInfo
            {
                ProcessId = 0x11223344,
                ThreadId = 0x55667788,
                ProcessPath = "UnitTestProcess",
                UserId = 0x99887766,
                UserName = "Hester the tester",
                GroupId = 0x22446688,
                GroupName = "ProcessInfoTestGroup"
            };

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiProcessInfo(new BinaryReader(ms));

            ClassicAssert.AreEqual(0x11223344, recreatedField.ProcessId);
            ClassicAssert.AreEqual(0x55667788, recreatedField.ThreadId);
            ClassicAssert.AreEqual("UnitTestProcess", recreatedField.ProcessPath);
            ClassicAssert.AreEqual(0x99887766, recreatedField.UserId);
            ClassicAssert.AreEqual("Hester the tester", recreatedField.UserName);
            ClassicAssert.AreEqual(0x22446688, recreatedField.GroupId);
            ClassicAssert.AreEqual("ProcessInfoTestGroup", recreatedField.GroupName);
        }

        [Test]
        public void Test_PpiSpectrum_Construction()
        {
            var field = new PpiSpectrum
            {
                StartingFrequency = 0x12345678,
                Resolution = 0x24683579,
                AmplitudeOffset = 0x98765432,
                AmplitudeResolution = 0x11223344,
                MaximumRssi = 0xAABB,
                SamplesData = new byte[] { 0xCC, 0xDD, 0xEE, 0xFF }
            };

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiSpectrum(new BinaryReader(ms));

            ClassicAssert.AreEqual(0x12345678, recreatedField.StartingFrequency);
            ClassicAssert.AreEqual(0x24683579, recreatedField.Resolution);
            ClassicAssert.AreEqual(0x98765432, recreatedField.AmplitudeOffset);
            ClassicAssert.AreEqual(0x11223344, recreatedField.AmplitudeResolution);
            ClassicAssert.AreEqual(0xAABB, recreatedField.MaximumRssi);
            ClassicAssert.AreEqual(new byte[] { 0xCC, 0xDD, 0xEE, 0xFF }, recreatedField.SamplesData);
        }

        [Test]
        public void Test_PpiUnknown_Construction()
        {
            var field = new PpiUnknown(0xAA, new byte[] { 0x1, 0x2, 0x3, 0x4 });

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiUnknown(0xAA, new BinaryReader(ms), 4);

            ClassicAssert.AreEqual(0xAA, (int) recreatedField.FieldType);
            ClassicAssert.AreEqual(new byte[] { 0x1, 0x2, 0x3, 0x4 }, recreatedField.Bytes);
        }
    }