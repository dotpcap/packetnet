using System.IO;
using NUnit.Framework;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class PpiFieldsTests
    {
        [Test]
        public void Test_Ppi802_3_Construction()
        {
            var field = new Ppi8023(Ppi8023.StandardFlags.FcsPresent,
                                    Ppi8023.ErrorFlags.InvalidFcs | Ppi8023.ErrorFlags.SymbolError);

            var recreatedField = new Ppi8023(new BinaryReader(new MemoryStream(field.Bytes)));

            Assert.AreEqual(Ppi8023.StandardFlags.FcsPresent, recreatedField.Flags);
            Assert.AreEqual(Ppi8023.ErrorFlags.InvalidFcs | Ppi8023.ErrorFlags.SymbolError, recreatedField.Errors);
        }

        [Test]
        public void Test_PpiAggregation_Construction()
        {
            var field = new PpiAggregation(22);
            Assert.AreEqual(22, field.InterfaceId);

            var recreatedField = new PpiAggregation(new BinaryReader(new MemoryStream(field.Bytes)));

            Assert.AreEqual(22, recreatedField.InterfaceId);
        }

        [Test]
        public void Test_PpiCaptureInfo_Construction()
        {
            var field = new PpiCaptureInfo();

            Assert.AreEqual(0, field.Bytes.Length);
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

            Assert.AreEqual(0x1234567812345678, recreatedField.TSFTimer);
            Assert.AreEqual(PpiCommon.CommonFlags.FcsIncludedInFrame | PpiCommon.CommonFlags.TimerSynchFunctionInUse, recreatedField.Flags);
            Assert.AreEqual(2, recreatedField.Rate);
            Assert.AreEqual(2142, recreatedField.ChannelFrequency);
            Assert.AreEqual(RadioTapChannelFlags.Channel2Ghz | RadioTapChannelFlags.Passive, recreatedField.ChannelFlags);
            Assert.AreEqual(0xAB, recreatedField.FhssHopset);
            Assert.AreEqual(0xCD, recreatedField.FhssPattern);
            Assert.AreEqual(-50, recreatedField.AntennaSignalPower);
            Assert.AreEqual(25, recreatedField.AntennaSignalNoise);
        }

        [Test]
        public void Test_PpiMacExtensions_Construction()
        {
            var field = new PpiMacExtensions { Flags = PpiMacExtensionFlags.DuplicateRx | PpiMacExtensionFlags.HtIndicator, AMpduId = 0x12345678, DelimiterCount = 0xA };

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiMacExtensions(new BinaryReader(ms));

            Assert.AreEqual(PpiMacExtensionFlags.DuplicateRx | PpiMacExtensionFlags.HtIndicator, recreatedField.Flags);
            Assert.AreEqual(0x12345678, field.AMpduId);
            Assert.AreEqual(0xA, field.DelimiterCount);
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

            Assert.AreEqual(0x12345678, recreatedField.AMpduId);
            Assert.AreEqual(0xAB, recreatedField.DelimiterCount);
            Assert.AreEqual(0x1, recreatedField.ModulationCodingScheme);
            Assert.AreEqual(0x2, recreatedField.SpatialStreamCount);
            Assert.AreEqual(0x3, recreatedField.RssiCombined);
            Assert.AreEqual(0x4, recreatedField.RssiAntenna0Control);
            Assert.AreEqual(0x5, recreatedField.RssiAntenna1Control);
            Assert.AreEqual(0x6, recreatedField.RssiAntenna2Control);
            Assert.AreEqual(0x7, recreatedField.RssiAntenna3Control);
            Assert.AreEqual(0x8, recreatedField.RssiAntenna0Ext);
            Assert.AreEqual(0x9, recreatedField.RssiAntenna1Ext);
            Assert.AreEqual(0xA, recreatedField.RssiAntenna2Ext);
            Assert.AreEqual(0xB, recreatedField.RssiAntenna3Ext);
            Assert.AreEqual(2142, recreatedField.ExtensionChannelFrequency);
            Assert.AreEqual(RadioTapChannelFlags.Channel5Ghz | RadioTapChannelFlags.Passive, recreatedField.ExtensionChannelFlags);
            Assert.AreEqual(0xC, recreatedField.DbmAntenna0SignalPower);
            Assert.AreEqual(0xD, recreatedField.DbmAntenna0SignalNoise);
            Assert.AreEqual(0xE, recreatedField.DbmAntenna1SignalPower);
            Assert.AreEqual(0xF, recreatedField.DbmAntenna1SignalNoise);
            Assert.AreEqual(0x1, recreatedField.DbmAntenna2SignalPower);
            Assert.AreEqual(0x2, recreatedField.DbmAntenna2SignalNoise);
            Assert.AreEqual(0x3, recreatedField.DbmAntenna3SignalPower);
            Assert.AreEqual(0x4, recreatedField.DbmAntenna3SignalNoise);
            Assert.AreEqual(0xAAAAAAAA, recreatedField.ErrorVectorMagnitude0);
            Assert.AreEqual(0xBBBBBBBB, recreatedField.ErrorVectorMagnitude1);
            Assert.AreEqual(0xCCCCCCCC, recreatedField.ErrorVectorMagnitude2);
            Assert.AreEqual(0xDDDDDDDD, recreatedField.ErrorVectorMagnitude3);
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

            Assert.AreEqual(0x11223344, recreatedField.ProcessId);
            Assert.AreEqual(0x55667788, recreatedField.ThreadId);
            Assert.AreEqual("UnitTestProcess", recreatedField.ProcessPath);
            Assert.AreEqual(0x99887766, recreatedField.UserId);
            Assert.AreEqual("Hester the tester", recreatedField.UserName);
            Assert.AreEqual(0x22446688, recreatedField.GroupId);
            Assert.AreEqual("ProcessInfoTestGroup", recreatedField.GroupName);
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

            Assert.AreEqual(0x12345678, recreatedField.StartingFrequency);
            Assert.AreEqual(0x24683579, recreatedField.Resolution);
            Assert.AreEqual(0x98765432, recreatedField.AmplitudeOffset);
            Assert.AreEqual(0x11223344, recreatedField.AmplitudeResolution);
            Assert.AreEqual(0xAABB, recreatedField.MaximumRssi);
            Assert.AreEqual(new byte[] { 0xCC, 0xDD, 0xEE, 0xFF }, recreatedField.SamplesData);
        }

        [Test]
        public void Test_PpiUnknown_Construction()
        {
            var field = new PpiUnknown(0xAA, new byte[] { 0x1, 0x2, 0x3, 0x4 });

            var ms = new MemoryStream(field.Bytes);
            var recreatedField = new PpiUnknown(0xAA, new BinaryReader(ms), 4);

            Assert.AreEqual(0xAA, (int) recreatedField.FieldType);
            Assert.AreEqual(new byte[] { 0x1, 0x2, 0x3, 0x4 }, recreatedField.Bytes);
        }
    }
}