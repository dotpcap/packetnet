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
                    return GetBitFieldValue(0);
                }

                set
                {
                    SetBitFieldValue(0, value);
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
                    return GetBitFieldValue(1);
                }

                set
                {
                    SetBitFieldValue(1, value);
                }
            }

            public bool CfPollable
            {
                get
                {
                    return GetBitFieldValue(2);
                }

                set
                {
                    SetBitFieldValue(2, value);
                }
            }


            public bool CfPollRequest
            {
                get
                {
                    return GetBitFieldValue(3);
                }

                set
                {
                    SetBitFieldValue(3, value);
                }
            }

            public bool Privacy
            {
                get
                {
                    return GetBitFieldValue(4);
                }

                set
                {
                    SetBitFieldValue(4, value);
                }
            }

            public bool ShortPreamble
            {
                get
                {
                    return GetBitFieldValue(5);
                }

                set
                {
                    SetBitFieldValue(5, value);
                }
            }

            public bool Pbcc
            {
                get
                {
                    return GetBitFieldValue(6);
                }

                set
                {
                    SetBitFieldValue(6, value);
                }
            }

            public bool ChannelAgility
            {
                get
                {
                    return GetBitFieldValue(7);
                }

                set
                {
                    SetBitFieldValue(7, value);
                }
            }

            public bool ShortTimeSlot
            {
                get
                {
                    return GetBitFieldValue(10);
                }

                set
                {
                    SetBitFieldValue(10, value);
                }
            }

            public bool DssOfdm
            {
                get
                {
                    return GetBitFieldValue(13);
                }

                set
                {
                    SetBitFieldValue(13, value);
                }
            }

            /// <summary>
            /// Returns true if the bit is set false if not.
            /// </summary>
            /// <param name="index">0 indexed position of the bit</param>
            private bool GetBitFieldValue(ushort index)
            {
                return (((Field >> index) & 0x1) == 1) ? true : false;
            }

            private void SetBitFieldValue(ushort index, bool value)
            {
                if (value)
                {
                    Field |= unchecked((UInt16)(1 << index));
                }
                else
                {
                    Field &= unchecked((UInt16)~(1 << index));
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
