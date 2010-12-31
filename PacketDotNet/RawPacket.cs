using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet.Utils;

namespace PacketDotNet
{
    /// <summary>
    /// Raw packet as loaded from a pcap device or file
    /// </summary>
    public class RawPacket
    {
        /// <value>
        /// Link layer from which this packet was captured
        /// </value>
        public LinkLayers LinkLayerType
        {
            get;
            set;
        }

        /// <value>
        /// The unix timeval when the packet was created
        /// </value>
        public PosixTimeval Timeval
        {
            get;
            set;
        }

        /// <summary> Fetch data portion of the packet.</summary>
        public virtual byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LinkLayerType">
        /// A <see cref="LinkLayers"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="Data">
        /// A <see cref="System.Byte"/>
        /// </param>
        public RawPacket(LinkLayers LinkLayerType,
                         PosixTimeval Timeval,
                         byte[] Data)
        {
            this.LinkLayerType = LinkLayerType;
            this.Timeval = Timeval;
            this.Data = Data;
        }

        /// <value>
        /// Color used when generating the text description of a packet
        /// </value>
        public virtual System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Black;
            }
        }

        /// <summary>Output this packet as a readable string</summary>
        public override System.String ToString()
        {
            return ToString(StringOutputType.Normal);
        }

        /// <summary cref="Packet.ToString(StringOutputType)" />
        public string ToString(StringOutputType outputFormat)
        {
            var buffer = new StringBuilder();
            string color = "";
            string colorEscape = "";

            if(outputFormat == StringOutputType.Colored || outputFormat == StringOutputType.VerboseColored)
            {
                color = Color;
                colorEscape = AnsiEscapeSequences.Reset;
            }

            if(outputFormat == StringOutputType.Normal || outputFormat == StringOutputType.Colored)
            {
                // build the output string
                buffer.AppendFormat("[{0}RawPacket: LinkLayerType={2}, Timeval={3}]{1}",
                    color,
                    colorEscape,
                    LinkLayerType,
                    Timeval);
            }

            if(outputFormat == StringOutputType.Verbose || outputFormat == StringOutputType.VerboseColored)
            {
                // collect the properties and their value
                Dictionary<string,string> properties = new Dictionary<string,string>();
                properties.Add("link layer type", LinkLayerType.ToString() + " (0x" + LinkLayerType.ToString("x") + ")");
                properties.Add("timeval", Timeval.ToString());

                // calculate the padding needed to right-justify the property names
                int padLength = Utils.RandomUtils.LongestStringLength(new List<string>(properties.Keys));

                // build the output string
                buffer.AppendLine("Raw:  ******* Raw - \"Raw Packet\"");
                buffer.AppendLine("Raw:");
                foreach(var property in properties)
                {
                    buffer.AppendLine("Raw: " + property.Key.PadLeft(padLength) + " = " + property.Value);
                }
                buffer.AppendLine("Raw:");
            }

            return buffer.ToString();
        }
    }
}
