using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    namespace Ieee80211
    {
        public class DurationField
        {
            /// <summary>
            /// This is the raw Duration field
            /// 
            /// </summary>
            public UInt16 Field { get; set; }

            public DurationField()
            {
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="field">
            /// A <see cref="UInt16"/>
            /// </param>
            public DurationField(UInt16 field)
            {
                this.Field = field;
            }
        } 
    }
}
