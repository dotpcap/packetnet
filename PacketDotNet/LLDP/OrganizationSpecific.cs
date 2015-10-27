/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using PacketDotNet.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// An Organization Specific TLV
    ///
    /// [TLV Type Length : 2][Organizationally Unique Identifier OUI : 3]
    /// [Organizationally Defined Subtype : 1][Organizationally Defined Information String : 0 - 507]
    /// </summary>
    [Serializable]
    public class OrganizationSpecific : TLV
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169, 0649
        private static readonly ILogInactive log;
#pragma warning restore 0169, 0649
#endif

        private const int OUILength = 3;
        private const int OUISubTypeLength = 1;

        #region Constructors

        /// <summary>
        /// Creates an Organization Specific TLV
        /// </summary>
        /// <param name="bytes">
        /// The LLDP Data unit being modified
        /// </param>
        /// <param name="offset">
        /// The Organization Specific TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public OrganizationSpecific(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            log.Debug("");
        }

        /// <summary>
        /// Creates an Organization Specific TLV and sets it value
        /// </summary>
        /// <param name="oui">
        /// An Organizationally Unique Identifier
        /// </param>
        /// <param name="subType">
        /// An Organizationally Defined SubType
        /// </param>
        /// <param name="infoString">
        /// An Organizationally Defined Information String
        /// </param>
        public OrganizationSpecific(byte[] oui, int subType, byte[] infoString)
        {
            log.Debug("");

            var length = TLVTypeLength.TypeLengthLength + OUILength + OUISubTypeLength;
            var bytes = new byte[length];
            var offset = 0;
            tlvData = new ByteArraySegment(bytes, offset, length);

            Type = TLVTypes.OrganizationSpecific;

            OrganizationUniqueID = oui;
            OrganizationDefinedSubType = subType;
            OrganizationDefinedInfoString = infoString;
        }

        #endregion

        #region Properties

        /// <summary>
        /// An Organizationally Unique Identifier
        /// </summary>
        public byte[] OrganizationUniqueID
        {
            get
            {
                byte[] oui = new byte[OUILength];
                Array.Copy(tlvData.Bytes, ValueOffset,
                           oui, 0,
                           OUILength);
                return oui;
            }

            set
            {
                Array.Copy(value, 0,
                           tlvData.Bytes, ValueOffset, OUILength);
            }
        }

        /// <summary>
        /// An Organizationally Defined SubType
        /// </summary>
        public int OrganizationDefinedSubType
        {
            get
            {
                return tlvData.Bytes[ValueOffset + OUILength];
            }
            set
            {
                tlvData.Bytes[ValueOffset + OUILength] = (byte)value;
            }
        }

        /// <summary>
        /// An Organizationally Defined Information String
        /// </summary>
        public byte[] OrganizationDefinedInfoString
        {
            get
            {
                var length = Length - (OUILength + OUISubTypeLength);

                var bytes = new byte[length];
                Array.Copy(tlvData.Bytes, ValueOffset + OUILength + OUISubTypeLength,
                           bytes, 0,
                           length);

                return bytes;
            }

            set
            {
                var length = Length - (OUILength + OUISubTypeLength);

                // do we have the right sized tlv?
                if(value.Length != length)
                {
                    var headerLength = TLVTypeLength.TypeLengthLength + OUILength + OUISubTypeLength;

                    // resize the tlv
                    var newLength =  headerLength + value.Length;
                    var bytes = new byte[newLength];

                    // copy the header bytes over
                    Array.Copy(tlvData.Bytes, tlvData.Offset,
                               bytes, 0,
                               headerLength);

                    // assign a new ByteArrayAndOffset to tlvData
                    var offset = 0;
                    tlvData = new ByteArraySegment(bytes, offset, newLength);
                }

                // copy the byte array in
                Array.Copy(value, 0,
                           tlvData.Bytes, ValueOffset + OUILength + OUISubTypeLength,
                           value.Length);
            }
        }

        /// <summary>
        /// Convert this Organization Specific TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString()
        {
            return string.Format("[OrganizationSpecific: OrganizationUniqueID={0}, OrganizationDefinedSubType={1}, OrganizationDefinedInfoString={2}]",
                    OUIToString(OrganizationUniqueID),
                    OrganizationDefinedSubType,
                    OrganizationDefinedInfoString);
        }

        private string OUIToString(byte[] OUIBytes)
        {
            string hexString = BitConverter.ToString(OUIBytes);
            string ouiStringFromList = OUIDefinitions.getEntry(hexString).OuiString;

            return string.Format("{0} ({1})", ouiStringFromList, hexString); 
        }

        #endregion
    }

    static class OUIDefinitions
    {
        public const string OUI_ENCAP_ETHER =       "00-00-00";   /* encapsulated Ethernet */
        public const string OUI_XEROX =             "00-00-06";   /* Xerox */
        public const string OUI_CISCO =             "00-00-0C";   /* Cisco (future use) */
        public const string OUI_IANA =              "00-00-5E";   /* the IANA */
        public const string OUI_NORTEL =            "00-00-81";   /* Nortel SONMP */
        public const string OUI_CISCO_90 =          "00-00-F8";   /* Cisco (IOS 9.0 and above?) */
        public const string OUI_CISCO_2 =           "00-01-42";   /* Cisco */
        public const string OUI_CISCO_3 =           "00-01-43";   /* Cisco */
        public const string OUI_FORCE10 =           "00-01-E8";   /* Force10 */
        public const string OUI_ERICSSON =          "00-01-EC";   /* Ericsson Group */
        public const string OUI_CATENA =            "00-02-5A";   /* Catena Networks */
        public const string OUI_ATHEROS =           "00-03-7F";   /* Atheros Communications */
        public const string OUI_SONY_ERICSSON =     "00-0A-D9";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_ARUBA =             "00-0B-86";   /* Aruba Networks */
        public const string OUI_SONY_ERICSSON_2 =   "00-0E-07";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_PROFINET =          "00-0E-CF";   /* PROFIBUS Nutzerorganisation e.V. */
        public const string OUI_RSN =               "00-0F-AC";   /* Wi-Fi : RSN */
        public const string OUI_SONY_ERICSSON_3 =   "00-0F-DE";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_CIMETRICS =         "00-10-90";   /* Cimetrics, Inc. */
        public const string OUI_IEEE_802_3 =        "00-12-0F";   /* IEEE 802.3 */
        public const string OUI_MEDIA_ENDPOINT =    "00-12-BB";   /* Media (TIA TR-41 Committee) */
        public const string OUI_SONY_ERICSSON_4 =   "00-12-EE";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_ERICSSON_MOBILE =   "00-15-E0";   /* Ericsson Mobile Platforms */
        public const string OUI_SONY_ERICSSON_5 =   "00-16-20";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_SONY_ERICSSON_6 =   "00-16-B8";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_SONY_ERICSSON_7 =   "00-18-13";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_BLUETOOTH =         "00-19-58";   /* Bluetooth SIG */
        public const string OUI_SONY_ERICSSON_8 =   "00-19-63";   /* Sony Ericsson Mobile Communications AB */
        public const string OUI_DCBX =              "00-1B-21";   /* Data Center Bridging Capabilities Exchange Protocol */
        public const string OUI_CISCO_UBI =         "00-1B-67";   /* Cisco/Ubiquisys */
        public const string OUI_IEEE_802_1QBG =     "00-1B-3F";   /* IEEE 802.1 Qbg */
        public const string OUI_NINTENDO =          "00-1F-32";   /* Nintendo */
        public const string OUI_TURBOCELL =         "00-20-F6";   /* KarlNet, who brought you Turbocell */
        public const string OUI_CISCOWL =           "00-40-96";   /* Cisco Wireless (Aironet) */
        public const string OUI_MARVELL =           "00-50-43";   /* Marvell Semiconductor */
        public const string OUI_WPAWME =            "00-50-F2";   /* Wi-Fi : WPA / WME */
        public const string OUI_ERICSSON_2 =        "00-80-37";   /* Ericsson Group */
        public const string OUI_IEEE_802_1 =        "00-80-C2";   /* IEEE 802.1 Committee */
        public const string OUI_PRE11N =            "00-90-4C";   /* Wi-Fi : 802.11 Pre-N */
        public const string OUI_ATM_FORUM =         "00-A0-3E";   /* ATM Forum */
        public const string OUI_ZYXELCOM =          "00-A0-C5";   /* ZyxelCom */
        public const string OUI_EXTREME =           "00-E0-2B";   /* Extreme EDP/ESRP */
        public const string OUI_CABLE_BPDU =        "00-E0-2F";   /* DOCSIS spanning tree BPDU */
        public const string OUI_FOUNDRY =           "00-E0-52";   /* Foundry */
        public const string OUI_SIEMENS =           "08-00-06";   /* Siemens AG */
        public const string OUI_APPLE_ATALK =       "08-00-07";   /* Appletalk */
        public const string OUI_HP =                "08-00-09";   /* Hewlett-Packard */
        public const string OUI_HP_2 =              "00-80-5F";   /* Hewlett-Packard */
        public const string OUI_HYTEC_GER =         "30-B2-16";   /* Hytec Geraetebau GmbH */
        public const string OUI_WFA =               "50-6F-9A";   /* Wi-Fi Alliance */
        public const string OUI_3GPP2 =             "CF-00-02";   /* 3GPP2 */

        public struct OUIEntry
        {
            private readonly string hexcode;
            private readonly string ouiString;

            public OUIEntry(string hexcode, string ouiString)
            {
                this.hexcode = hexcode;
                this.ouiString = ouiString;
            }

            public string Hexcode { get { return hexcode; } }
            public string OuiString { get { return ouiString; } }
        }

        public static OUIEntry getEntry(string pattern)
        {
            OUIEntry defaultOUIEntry = new OUIEntry("", "UNKNOWN");
            OUIEntry retVal = OUIArray.Where(i => i.Hexcode == pattern).DefaultIfEmpty(defaultOUIEntry).FirstOrDefault();
            
            return retVal;
        }

        static readonly IList<OUIEntry> OUIArray = new ReadOnlyCollection<OUIEntry>
            (new[] {
                new OUIEntry(OUI_ENCAP_ETHER,       "encapsulated Ethernet"),
                new OUIEntry(OUI_XEROX,             "Xerox"),
                new OUIEntry(OUI_CISCO,             "Cisco"),
                new OUIEntry(OUI_IANA,              "IANA"),
                new OUIEntry(OUI_NORTEL,            "Nortel Discovery Protocol"),
                new OUIEntry(OUI_CISCO_90,          "Cisco IOS 9.0 Compatible"),
                new OUIEntry(OUI_CISCO_2,           "Cisco"),
                new OUIEntry(OUI_CISCO_3,           "Cisco"),
                new OUIEntry(OUI_FORCE10,           "Force10 Networks"),
                new OUIEntry(OUI_ERICSSON,          "Ericsson Group"),
                new OUIEntry(OUI_CATENA,            "Catena Networks"),
                new OUIEntry(OUI_ATHEROS,           "Atheros Communications"),
                new OUIEntry(OUI_SONY_ERICSSON,     "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_ARUBA,             "Aruba Networks"),
                new OUIEntry(OUI_SONY_ERICSSON_2,   "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_PROFINET,          "PROFIBUS Nutzerorganisation e.V."),
                new OUIEntry(OUI_RSN,               "Wi-Fi : RSN"),
                new OUIEntry(OUI_SONY_ERICSSON_3,   "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_CIMETRICS,         "Cimetrics, Inc."),
                new OUIEntry(OUI_IEEE_802_3,        "IEEE 802.3"),
                new OUIEntry(OUI_MEDIA_ENDPOINT,    "Media (TIA TR-41 Committee)"),
                new OUIEntry(OUI_SONY_ERICSSON_4,   "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_ERICSSON_MOBILE,   "Ericsson Mobile Platforms"),
                new OUIEntry(OUI_SONY_ERICSSON_5,   "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_SONY_ERICSSON_6,   "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_SONY_ERICSSON_7,   "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_BLUETOOTH,         "Bluetooth SIG"),
                new OUIEntry(OUI_SONY_ERICSSON_8,   "Sony Ericsson Mobile Communications AB"),
                new OUIEntry(OUI_DCBX,              "Data Center Bridging Capabilities Exchange Protocol"),
                new OUIEntry(OUI_CISCO_UBI,         "Cisco/Ubiquisys"),
                new OUIEntry(OUI_IEEE_802_1QBG,     "IEEE 802.1 Qbg"),
                new OUIEntry(OUI_NINTENDO,          "Nintendo"),
                new OUIEntry(OUI_TURBOCELL,         "KarlNet, who brought you Turbocell"),
                new OUIEntry(OUI_CISCOWL,           "Cisco Wireless (Aironet)"),
                new OUIEntry(OUI_MARVELL,           "Marvell Semiconductor"),
                new OUIEntry(OUI_WPAWME,            "Wi-Fi : WPA / WME"),
                new OUIEntry(OUI_ERICSSON_2,        "Ericsson Group"),
                new OUIEntry(OUI_IEEE_802_1,        "IEEE 802.1"),
                new OUIEntry(OUI_PRE11N,            "Wi-Fi : 802.11 Pre-N"),
                new OUIEntry(OUI_ATM_FORUM,         "ATM Forum"),
                new OUIEntry(OUI_ZYXELCOM,          "ZyxelCom"),
                new OUIEntry(OUI_EXTREME,           "Extreme EDP / ESRP"),
                new OUIEntry(OUI_CABLE_BPDU,        "DOCSIS spanning tree BPDU"),
                new OUIEntry(OUI_FOUNDRY,           "Foundry"),
                new OUIEntry(OUI_SIEMENS,           "Siemens AG"),
                new OUIEntry(OUI_APPLE_ATALK,       "Appletalk"),
                new OUIEntry(OUI_HP,                "Hewlett-Packard"),
                new OUIEntry(OUI_HP_2,              "Hewlett-Packard"),
                new OUIEntry(OUI_HYTEC_GER,         "Hytec Geraetebau GmbH"),
                new OUIEntry(OUI_WFA,               "Wi-Fi Alliance"),
                new OUIEntry(OUI_3GPP2,             "3GPP2")
                /* End of List */
            }
            );
    }
}
