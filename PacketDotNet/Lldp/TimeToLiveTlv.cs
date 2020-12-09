/*
This file is part of PacketDotNet

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using PacketDotNet.Utils;
using PacketDotNet.Utils.Converters;
#if DEBUG
using System.Reflection;
using log4net;

#endif

namespace PacketDotNet.Lldp
{
    /// <summary>
    /// A Time to Live Tlv
    /// </summary>
    public class TimeToLiveTlv : Tlv
    {
#if DEBUG
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive Log;
#pragma warning restore 0169, 0649
#endif

        /// <summary>
        /// Number of bytes in the value portion of this tlv
        /// </summary>
        private const int ValueLength = 2;

        /// <summary>
        /// Creates a TTL Tlv
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The TTL TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public TimeToLiveTlv(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            Log.Debug("");
        }

        /// <summary>
        /// Creates a TTL TLV and sets it value
        /// </summary>
        /// <param name="seconds">
        /// The length in seconds until the LLDP
        /// is refreshed
        /// </param>
        public TimeToLiveTlv(ushort seconds)
        {
            Log.Debug("");

            var bytes = new byte[TlvTypeLength.TypeLengthLength + ValueLength];
            var offset = 0;
            var length = bytes.Length;
            Data = new ByteArraySegment(bytes, offset, length);

            Type = TlvType.TimeToLive;
            Seconds = seconds;
        }

        /// <value>
        /// The number of seconds until the LLDP needs
        /// to be refreshed
        /// A value of 0 means that the LLDP source is
        /// closed and should no longer be refreshed
        /// </value>
        public ushort Seconds
        {
            get => EndianBitConverter.Big.ToUInt16(Data.Bytes,
                                                   Data.Offset + TlvTypeLength.TypeLengthLength);
            set => EndianBitConverter.Big.CopyBytes(value,
                                                    Data.Bytes,
                                                    Data.Offset + TlvTypeLength.TypeLengthLength);
        }

        /// <summary>
        /// Convert this TTL TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return $"[TimeToLive: Seconds={Seconds}]";
        }
    }
}