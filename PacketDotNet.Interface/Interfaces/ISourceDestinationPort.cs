using System;

namespace PacketDotNet.Interfaces
{
    public interface ISourceDestinationPort
    {
        /// <summary> Fetch the port number on the source host.</summary>
        UInt16 SourcePort { get; set; }

        /// <summary> Fetch the port number on the target host.</summary>
        UInt16 DestinationPort { get; set; }
    }
}