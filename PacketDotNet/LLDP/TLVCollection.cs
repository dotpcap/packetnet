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
using System.Collections.ObjectModel;
using System.Reflection;
using log4net;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// Custom collection for TLV types
    /// Special behavior includes:
    /// - Preventing an EndOfLLDPDU tlv from being added out of place
    /// - Checking and throwing exceptions if one-per-LLDP packet TLVs are added multiple times
    /// </summary>
    [Serializable]
    public class TLVCollection : Collection<TLV>
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
        /// Override to:
        /// - Prevent duplicate end tlvs from being added
        /// - Ensure that an end tlv is present
        /// - Replace any automatically added end tlvs with the user provided tlv
        /// </summary>
        /// <param name="index">
        /// A <see cref="System.Int32" />
        /// </param>
        /// <param name="item">
        /// A <see cref="TLV" />
        /// </param>
        protected override void InsertItem(Int32 index, TLV item)
        {
            Log.DebugFormat("index {0}, TLV.GetType {1}, TLV.Type {2}",
                            index,
                            item.GetType(),
                            item.Type);

            // if this is the first item and it isn't an End TLV we should add the end tlv
            if (Count == 0 && item.Type != TLVTypes.EndOfLLDPU)
            {
                Log.Debug("Inserting EndOfLLDPDU");
                base.InsertItem(0, new EndOfLLDPDU());
            }
            else if (Count != 0)
            {
                // if the user is adding their own End tlv we should replace ours
                // with theirs
                if (item.Type == TLVTypes.EndOfLLDPU)
                {
                    Log.DebugFormat("Replacing {0} with user provided {1}, Type {2}",
                                    this[Count - 1].GetType(),
                                    item.GetType(),
                                    item.Type);
                    SetItem(Count - 1, item);
                    return;
                }
            }

            // if we have no items insert the first item wherever
            // if we have items insert the item befor the last item as the last item is a EndOfLLDPDU
            var insertPosition = Count == 0 ? 0 : Count - 1;

            Log.DebugFormat("Inserting item at position {0}", insertPosition);

            base.InsertItem(insertPosition, item);
        }
    }
}