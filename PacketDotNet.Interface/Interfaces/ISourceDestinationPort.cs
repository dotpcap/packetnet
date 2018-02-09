namespace PacketDotNet.Interface.Interfaces
{
    public interface ISourceDestinationPort
    {
        /// <summary> Fetch the port number on the source host.</summary>
        ushort SourcePort { get; set; }

        /// <summary> Fetch the port number on the target host.</summary>
        ushort DestinationPort { get; set; }
    }
}