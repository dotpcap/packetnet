using System.IO;
using NUnit.Framework;
using PacketDotNet.Ieee80211;

namespace Test.PacketType.Ieee80211;

    [TestFixture]
    public class RadioTapFieldsTest
    {
        [Test]
        public void Test_AntennaRadioTapField()
        {
            var field = new AntennaRadioTapField(0xAB);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new AntennaRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.Antenna, recreatedField.Antenna);
        }

        [Test]
        public void Test_ChannelRadioTapField()
        {
            var field = new ChannelRadioTapField(2142, RadioTapChannelFlags.Channel2Ghz | RadioTapChannelFlags.Ofdm);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new ChannelRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.FrequencyMHz, recreatedField.FrequencyMHz);
            Assert.AreEqual(field.Channel, recreatedField.Channel);
            Assert.AreEqual(field.Flags, recreatedField.Flags);
        }

        [Test]
        public void Test_DbAntennaNoiseRadioTapField()
        {
            var field = new DbAntennaNoiseRadioTapField(0xAB);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new DbAntennaNoiseRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.AntennaNoisedB, recreatedField.AntennaNoisedB);
        }

        [Test]
        public void Test_DbAntennaSignalRadioTapField()
        {
            var field = new DbAntennaSignalRadioTapField(0x12);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new DbAntennaSignalRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.SignalStrengthdB, recreatedField.SignalStrengthdB);
        }

        [Test]
        public void Test_DbmAntennaNoiseRadioTapField()
        {
            var field = new DbmAntennaNoiseRadioTapField(127);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new DbmAntennaNoiseRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.AntennaNoisedBm, recreatedField.AntennaNoisedBm);
        }

        [Test]
        public void Test_DbmAntennaSignalRadioTapField()
        {
            var field = new DbmAntennaSignalRadioTapField(-128);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new DbmAntennaSignalRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.AntennaSignalDbm, recreatedField.AntennaSignalDbm);
        }

        [Test]
        public void Test_DbmTxPowerRadioTapField()
        {
            var field = new DbmTxPowerRadioTapField(100);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new DbmTxPowerRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.TxPowerdBm, recreatedField.TxPowerdBm);
        }

        [Test]
        public void Test_DbTxAttenuationRadioTapField()
        {
            var field = new DbTxAttenuationRadioTapField(-1234);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new DbTxAttenuationRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.TxPowerdB, recreatedField.TxPowerdB);
        }

        [Test]
        public void Test_FhssRadioTapField()
        {
            var field = new FhssRadioTapField(5, 6);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new FhssRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.ChannelHoppingSet, recreatedField.ChannelHoppingSet);
            Assert.AreEqual(field.Pattern, recreatedField.Pattern);
        }

        [Test]
        public void Test_FlagsRadioTapField()
        {
            var field = new FlagsRadioTapField(RadioTapFlags.ShortPreamble | RadioTapFlags.WepEncrypted);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new FlagsRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.Flags, recreatedField.Flags);
        }

        [Test]
        public void Test_LockQualityRadioTapField()
        {
            var field = new LockQualityRadioTapField(0x1234);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new LockQualityRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.SignalQuality, recreatedField.SignalQuality);
        }

        [Test]
        public void Test_RateRadioTapField()
        {
            var field = new RateRadioTapField(2);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new RateRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.RateMbps, recreatedField.RateMbps);
        }

        [Test]
        public void Test_RxFlagsRadioTapField()
        {
            var field = new RxFlagsRadioTapField(true);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new RxFlagsRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.PlcpCrcCheckFailed, recreatedField.PlcpCrcCheckFailed);
        }

        [Test]
        public void Test_TsftRadioTapField()
        {
            var field = new TsftRadioTapField(0x12345678);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new TsftRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.TimestampUsec, recreatedField.TimestampUsec);
        }

        [Test]
        public void Test_TxAttenuationRadioTapField()
        {
            var field = new TxAttenuationRadioTapField(-4321);

            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);

            var recreatedField = new TxAttenuationRadioTapField(new BinaryReader(new MemoryStream(bytes)));

            Assert.AreEqual(field.TxPower, recreatedField.TxPower);
        }
    }