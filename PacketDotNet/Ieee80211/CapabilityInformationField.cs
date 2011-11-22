using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class CapabilityInformationField
        {

            /// <summary>
            /// Is set to 1 when the beacon frame is representing an ESS (as opposed to an IBSS)
            /// 
            /// This field and IsIbss should be mutually exclusive
            /// </summary>
            public bool IsEss
            {
                get
                {
                    return ((Field & 0x1) == 1) ? true : false;
                }
            }

            /// <summary>
            /// Is set to 1 when the beacon frame is representing an IBSS (as opposed to an ESS)
            /// 
            /// This field and IsEss should be mutually exclusive
            /// </summary>
            public bool IsIbss
            {
                get
                {
                    return (((Field >> 1) & 0x1) == 1) ? true : false;
                }
            }

            public bool CfPollable
            {
                get
                {
                    return (((Field >> 2) & 0x1) == 1) ? true : false;
                }
            }


            public bool CfPollRequest
            {
                get
                {
                    return (((Field >> 3) & 0x1) == 1) ? true : false;
                }
            }

            public bool Privacy
            {
                get
                {
                    return (((Field >> 4) & 0x1) == 1) ? true : false;
                }
            }

            public bool ShortPreamble
            {
                get
                {
                    return (((Field >> 5) & 0x1) == 1) ? true : false;
                }
            }

            public bool Pbcc
            {
                get
                {
                    return (((Field >> 6) & 0x1) == 1) ? true : false;
                }
            }

            public bool ChannelAgility
            {
                get
                {
                    return (((Field >> 7) & 0x1) == 1) ? true : false;
                }
            }

            public bool ShortTimeSlot
            {
                get
                {
                    return (((Field >> 10) & 0x1) == 1) ? true : false;
                }
            }

            public bool DssOfdm
            {
                get
                {
                    return (((Field >> 13) & 0x1) == 1) ? true : false;
                }
            }

            public UInt16 Field { get; set; }

            public CapabilityInformationField()
            {

            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="field">
            /// A <see cref="UInt16"/>
            /// </param>
            public CapabilityInformationField(UInt16 field)
            {
                this.Field = field;
            }
        } 
    }
}
