using System;
using System.IO;
using System.Text;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// PPI process info field.
    /// </summary>
    public class PpiProcessInfo : PpiFields
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType => PpiFieldType.PpiProcessInfo;

        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override Int32 Length
        {
            get
            {
                var processLength = String.IsNullOrEmpty(ProcessPath) ? 0 : Encoding.UTF8.GetByteCount(ProcessPath);
                var userLength = String.IsNullOrEmpty(UserName) ? 0 : Encoding.UTF8.GetByteCount(UserName);
                var groupLength = String.IsNullOrEmpty(GroupName) ? 0 : Encoding.UTF8.GetByteCount(GroupName);
                return 19 + processLength + userLength + groupLength;
            }
        }

        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        /// <value>
        /// The process identifier.
        /// </value>
        public UInt32 ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the thread identifier.
        /// </summary>
        /// <value>
        /// The thread identifier.
        /// </value>
        public UInt32 ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the process path.
        /// </summary>
        /// <value>
        /// The process path.
        /// </value>
        public String ProcessPath { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public UInt32 UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>
        /// The user name.
        /// </value>
        public String UserName { get; set; }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public UInt32 GroupId { get; set; }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        /// <value>
        /// The group name.
        /// </value>
        public String GroupName { get; set; }

        /// <summary>
        /// Gets the field bytes. This doesn't include the PPI field header.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public override Byte[] Bytes
        {
            get
            {
                var ms = new MemoryStream();
                var writer = new BinaryWriter(ms);

                writer.Write(ProcessId);
                writer.Write(ThreadId);

                var pathBytes = Encoding.UTF8.GetBytes(ProcessPath ?? String.Empty);
                writer.Write((Byte) pathBytes.Length);
                writer.Write(pathBytes);

                writer.Write(UserId);

                var userBytes = Encoding.UTF8.GetBytes(UserName ?? String.Empty);
                writer.Write((Byte) userBytes.Length);
                writer.Write(userBytes);

                writer.Write(GroupId);

                var groupBytes = Encoding.UTF8.GetBytes(GroupName ?? String.Empty);
                writer.Write((Byte) groupBytes.Length);
                writer.Write(groupBytes);

                return ms.ToArray();
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiProcessInfo" /> class from the
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public PpiProcessInfo(BinaryReader br)
        {
            ProcessId = br.ReadUInt32();
            ThreadId = br.ReadUInt32();

            var pathLength = br.ReadByte();
            ProcessPath = Encoding.UTF8.GetString(br.ReadBytes(pathLength));

            UserId = br.ReadUInt32();

            var userLength = br.ReadByte();
            UserName = Encoding.UTF8.GetString(br.ReadBytes(userLength));

            GroupId = br.ReadUInt32();

            var groupLength = br.ReadByte();
            GroupName = Encoding.UTF8.GetString(br.ReadBytes(groupLength));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PpiProcessInfo" /> class.
        /// </summary>
        public PpiProcessInfo()
        { }

        #endregion Constructors
    }
}