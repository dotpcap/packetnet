/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

using System.Collections.Generic;
using PacketDotNet.Ndp;
using PacketDotNet.Utils;

namespace PacketDotNet;

public abstract class NdpPacket : Packet
{
	/// <summary>
	/// Initializes a new instance of the <see cref="NdpPacket" /> class.
	/// </summary>
	/// <param name="header">The header.</param>
	protected NdpPacket(ByteArraySegment header)
	{
		Header = header;
	}

	/// <summary>
	/// Gets or sets the options.
	/// </summary>
	public abstract List<NdpOption> OptionsCollection { get; set; }

	/// <summary>
	/// Parses options, pointed to by optionBytes into an array of Options
	/// </summary>
	/// <param name="optionBytes">
	/// A <see cref="T:System.Byte[]" />
	/// </param>
	/// <returns>
	/// A <see cref="List{Option}" />
	/// </returns>
	protected static List<NdpOption> ParseOptions(ByteArraySegment optionBytes)
	{
		// Reset the OptionsCollection list to prepare to be re-populated with new data.
		var options = new List<NdpOption>();

		if (optionBytes.Length == 0)
			return options;

		var offset = optionBytes.Offset;

		// The options should be bound by their options offset + length.
		var maxOffset = optionBytes.Offset + optionBytes.Length;

		// Include a basic check against the available length of the options buffer for invalid TCP packet data in the data offset field.
		while ((offset < maxOffset) && (offset < optionBytes.Bytes.Length - NdpOption.LengthFieldOffset))
		{
			var type = (OptionTypes) optionBytes.Bytes[offset + NdpOption.TypeFieldOffset];
			var length = optionBytes.Bytes[offset + NdpOption.LengthFieldOffset];

			switch (type)
			{
				case OptionTypes.SourceLinkLayerAddress:
				case OptionTypes.TargetLinkLayerAddress:
					options.Add(new NdpLinkLayerAddressOption(optionBytes.Bytes, offset, length));
					break;
				case OptionTypes.PrefixInformation:
					options.Add(new NdpLinkPrefixInformationOption(optionBytes.Bytes, offset, length));
					break;
				case OptionTypes.RedirectedHeader:
					options.Add(new NdpRedirectedHeaderOption(optionBytes.Bytes, offset, length));
					break;
				case OptionTypes.Mtu:
					options.Add(new NdpMtuOption(optionBytes.Bytes, offset, length));
					break;
			}

			offset += length * 8;
		}

		return options;
	}
}