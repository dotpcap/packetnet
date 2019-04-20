using System;
using NUnit.Framework;
using PacketDotNet.Ieee80211;
using System.IO;

namespace Test.PacketType.Ieee80211
{
    [TestFixture]
    public class RadioTapFieldsTest
    {
        [Test]
        public void Test_ChannelRadioTapField ()
        {
            ChannelRadioTapField field = new ChannelRadioTapField(2142, RadioTapChannelFlags.Channel2Ghz | RadioTapChannelFlags.Ofdm);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            ChannelRadioTapField recreatedField = new ChannelRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.FrequencyMHz, recreatedField.FrequencyMHz);
            Assert.AreEqual(field.Channel, recreatedField.Channel);
            Assert.AreEqual(field.Flags, recreatedField.Flags);
        }
    
        [Test]
        public void Test_FhssRadioTapField()
        {
            FhssRadioTapField field = new FhssRadioTapField(5, 6);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            FhssRadioTapField recreatedField = new FhssRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.ChannelHoppingSet, recreatedField.ChannelHoppingSet);
            Assert.AreEqual(field.Pattern, recreatedField.Pattern);
        }
        
        [Test]
        public void Test_FlagsRadioTapField()
        {
            FlagsRadioTapField field = new FlagsRadioTapField(RadioTapFlags.ShortPreamble | RadioTapFlags.WepEncrypted);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            FlagsRadioTapField recreatedField = new FlagsRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.Flags, recreatedField.Flags);
        }
        
        [Test]
        public void Test_RateRadioTapField ()
        {
            RateRadioTapField field = new RateRadioTapField(2);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            RateRadioTapField recreatedField = new RateRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.RateMbps, recreatedField.RateMbps);
        }
        
        [Test]
        public void Test_DbAntennaSignalRadioTapField ()
        {
            DbAntennaSignalRadioTapField field = new DbAntennaSignalRadioTapField(0x12);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            DbAntennaSignalRadioTapField recreatedField = new DbAntennaSignalRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.SignalStrengthdB, recreatedField.SignalStrengthdB);
        }
        
        [Test]
        public void Test_DbAntennaNoiseRadioTapField ()
        {
            DbAntennaNoiseRadioTapField field = new DbAntennaNoiseRadioTapField(0xAB);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            DbAntennaNoiseRadioTapField recreatedField = new DbAntennaNoiseRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.AntennaNoisedB, recreatedField.AntennaNoisedB);
        }
        
        [Test]
        public void Test_AntennaRadioTapField()
        {
            AntennaRadioTapField field = new AntennaRadioTapField(0xAB);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            AntennaRadioTapField recreatedField = new AntennaRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.Antenna, recreatedField.Antenna);
        }
        
        [Test]
        public void Test_DbmAntennaSignalRadioTapField ()
        {
            DbmAntennaSignalRadioTapField field = new DbmAntennaSignalRadioTapField(-128);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            DbmAntennaSignalRadioTapField recreatedField = new DbmAntennaSignalRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.AntennaSignalDbm, recreatedField.AntennaSignalDbm);
        }
        
        [Test]
        public void Test_DbmAntennaNoiseRadioTapField()
        {
            DbmAntennaNoiseRadioTapField field = new DbmAntennaNoiseRadioTapField(127);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            DbmAntennaNoiseRadioTapField recreatedField = new DbmAntennaNoiseRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.AntennaNoisedBm, recreatedField.AntennaNoisedBm);
        }
        
        [Test]
        public void Test_LockQualityRadioTapField()
        {
            LockQualityRadioTapField field = new LockQualityRadioTapField(0x1234);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            LockQualityRadioTapField recreatedField = new LockQualityRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.SignalQuality, recreatedField.SignalQuality);
        }
        
        [Test]
        public void Test_TsftRadioTapField()
        {
            TsftRadioTapField field = new TsftRadioTapField(0x12345678);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            TsftRadioTapField recreatedField = new TsftRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.TimestampUsec, recreatedField.TimestampUsec);
        }
        
        [Test]
        public void Test_RxFlagsRadioTapField ()
        {
            RxFlagsRadioTapField field = new RxFlagsRadioTapField(true);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            RxFlagsRadioTapField recreatedField = new RxFlagsRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.PlcpCrcCheckFailed, recreatedField.PlcpCrcCheckFailed);
        }
        
        [Test]
        public void Test_TxAttenuationRadioTapField ()
        {
            TxAttenuationRadioTapField field = new TxAttenuationRadioTapField(-4321);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            TxAttenuationRadioTapField recreatedField = new TxAttenuationRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.TxPower, recreatedField.TxPower);
        }
        
        [Test]
        public void Test_DbTxAttenuationRadioTapField()
        {
            DbTxAttenuationRadioTapField field = new DbTxAttenuationRadioTapField(-1234);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            DbTxAttenuationRadioTapField recreatedField = new DbTxAttenuationRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.TxPowerdB, recreatedField.TxPowerdB);
        }
        
        [Test]
        public void Test_DbmTxPowerRadioTapField()
        {
            DbmTxPowerRadioTapField field = new DbmTxPowerRadioTapField(100);
            
            var bytes = new byte[field.Length];
            field.CopyTo(bytes, 0);
            
            DbmTxPowerRadioTapField recreatedField = new DbmTxPowerRadioTapField(new BinaryReader (new MemoryStream(bytes)));
            
            Assert.AreEqual(field.TxPowerdBm, recreatedField.TxPowerdBm);
        }
    }
    
}

