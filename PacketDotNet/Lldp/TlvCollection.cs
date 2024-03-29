﻿/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/
/*
 *  Copyright 2010 Evan Plaice <evanplaice@gmail.com>
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System.Collections.ObjectModel;

#if DEBUG
using log4net;
using System.Reflection;
#endif

namespace PacketDotNet.Lldp;

    /// <summary>
    /// Custom collection for TLV types
    /// Special behavior includes:
    /// - Preventing an EndOfLldpdu TLV from being added out of place
    /// - Checking and throwing exceptions if one-per-LLDP packet TLVs are added multiple times
    /// </summary>
    public class TlvCollection : Collection<Tlv>
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
        /// - Prevent duplicate end TLVs from being added
        /// - Ensure that an end TLV is present
        /// - Replace any automatically added end TLVs with the user provided tlv
        /// </summary>
        /// <param name="index">
        /// A <see cref="int" />
        /// </param>
        /// <param name="item">
        /// A <see cref="Tlv" />
        /// </param>
        protected override void InsertItem(int index, Tlv item)
        {
            Log.DebugFormat("index {0}, Tlv.GetType {1}, Tlv.Type {2}",
                            index,
                            item.GetType(),
                            item.Type);

            // if this is the first item and it isn't an End TLV we should add the end tlv
            if (Count == 0 && item.Type != TlvType.EndOfLldpu)
            {
                Log.Debug("Inserting EndOfLldpdu");
                base.InsertItem(0, new EndOfLldpduTlv());
            }
            else if (Count != 0)
            {
                // if the user is adding their own End TLV we should replace ours
                // with theirs
                if (item.Type == TlvType.EndOfLldpu)
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
            // if we have items insert the item before the last item as the last item is a EndOfLldpdu
            var insertPosition = Count == 0 ? 0 : Count - 1;

            Log.DebugFormat("Inserting item at position {0}", insertPosition);

            base.InsertItem(insertPosition, item);
        }
    }