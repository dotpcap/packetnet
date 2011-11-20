using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet
{
    public class Ieee80211DurationField
    {
        /// <summary>
        /// This is the raw Duration field
        /// 
        /// </summary>
        public  UInt16 Field;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">
        /// A <see cref="UInt16"/>
        /// </param>
        public Ieee80211DurationField(UInt16 field)
        {
            this.Field = field;
        }
    }
}
