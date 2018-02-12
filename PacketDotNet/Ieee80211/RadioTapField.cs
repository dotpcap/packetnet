using System;
using System.IO;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    ///     Abstract class for all radio tap fields
    /// </summary>
    public abstract class RadioTapField
    {
        /// <summary>Type of the field</summary>
        public abstract RadioTapType FieldType { get; }

        /// <summary>
        ///     Gets the length of the field data.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public abstract UInt16 Length { get; }

        /// <summary>
        ///     Copies the field data to the destination buffer at the specified offset.
        /// </summary>
        public abstract void CopyTo(Byte[] dest, Int32 offset);

        /// <summary>
        ///     Parse a radio tap field, indicated by bitIndex, from a given BinaryReader
        /// </summary>
        /// <param name="bitIndex">
        ///     A <see cref="System.Int32" />
        /// </param>
        /// <param name="br">
        ///     A <see cref="BinaryReader" />
        /// </param>
        /// <returns>
        ///     A <see cref="RadioTapField" />
        /// </returns>
        public static RadioTapField Parse(Int32 bitIndex, BinaryReader br)
        {
            var Type = (RadioTapType) bitIndex;
            switch (Type)
            {
                case RadioTapType.Flags:
                    return new FlagsRadioTapField(br);
                case RadioTapType.Rate:
                    return new RateRadioTapField(br);
                case RadioTapType.DbAntennaSignal:
                    return new DbAntennaSignalRadioTapField(br);
                case RadioTapType.DbAntennaNoise:
                    return new DbAntennaNoiseRadioTapField(br);
                case RadioTapType.Antenna:
                    return new AntennaRadioTapField(br);
                case RadioTapType.DbmAntennaSignal:
                    return new DbmAntennaSignalRadioTapField(br);
                case RadioTapType.DbmAntennaNoise:
                    return new DbmAntennaNoiseRadioTapField(br);
                case RadioTapType.Channel:
                    return new ChannelRadioTapField(br);
                case RadioTapType.Fhss:
                    return new FhssRadioTapField(br);
                case RadioTapType.LockQuality:
                    return new LockQualityRadioTapField(br);
                case RadioTapType.TxAttenuation:
                    return new TxAttenuationRadioTapField(br);
                case RadioTapType.DbTxAttenuation:
                    return new DbTxAttenuationRadioTapField(br);
                case RadioTapType.DbmTxPower:
                    return new DbmTxPowerRadioTapField(br);
                case RadioTapType.Tsft:
                    return new TsftRadioTapField(br);
                case RadioTapType.RxFlags:
                    return new RxFlagsRadioTapField(br);
                default:
                    //the RadioTap fields are extendable so there may be some we dont know about
                    return null;
            }
        }
    }
}