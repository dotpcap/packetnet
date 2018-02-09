using System;
using System.IO;
using System.Text;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// PPI process info field.
    /// </summary>
    public class PpiProcessInfo : PpiField
    {
        #region Properties

        /// <summary>Type of the field</summary>
        public override PpiFieldType FieldType
        {
            get { return PpiFieldType.PpiProcessInfo;}
        }
            
        /// <summary>
        /// Gets the length of the field data.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public override int Length
        {
            get
            {
                var processLength = (String.IsNullOrEmpty(this.ProcessPath)) ? 0 : Encoding.UTF8.GetByteCount(this.ProcessPath);
                var userLength = (String.IsNullOrEmpty(this.UserName)) ? 0 : Encoding.UTF8.GetByteCount(this.UserName);
                var groupLength = (String.IsNullOrEmpty(this.GroupName)) ? 0 : Encoding.UTF8.GetByteCount(this.GroupName);
                return 19 + processLength + userLength + groupLength;
            }
        }
            
        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        /// <value>
        /// The process identifier.
        /// </value>
        public uint ProcessId { get; set; }
        /// <summary>
        /// Gets or sets the thread identifier.
        /// </summary>
        /// <value>
        /// The thread identifier.
        /// </value>
        public uint ThreadId { get; set; }
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
        public uint UserId { get; set; }
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
        public uint GroupId { get; set; }
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
        public override byte[] Bytes
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                    
                writer.Write(this.ProcessId);
                writer.Write(this.ThreadId);
                    
                var pathBytes = Encoding.UTF8.GetBytes(this.ProcessPath ?? String.Empty);
                writer.Write((byte)pathBytes.Length);
                writer.Write(pathBytes);
                    
                writer.Write(this.UserId);
                    
                var userBytes = Encoding.UTF8.GetBytes(this.UserName ?? String.Empty);
                writer.Write((byte)userBytes.Length);
                writer.Write(userBytes);
                    
                writer.Write(this.GroupId);
                    
                var groupBytes = Encoding.UTF8.GetBytes(this.GroupName ?? String.Empty);
                writer.Write((byte)groupBytes.Length);
                writer.Write(groupBytes);
                        
                return ms.ToArray();
            }
        }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.PpiProcessInfo"/> class from the 
        /// provided stream.
        /// </summary>
        /// <remarks>
        /// The position of the BinaryReader's underlying stream will be advanced to the end
        /// of the PPI field.
        /// </remarks>
        /// <param name='br'>
        /// The stream the field will be read from
        /// </param>
        public PpiProcessInfo (BinaryReader br)
        {
                
            this.ProcessId = br.ReadUInt32();
            this.ThreadId = br.ReadUInt32();
                
            var pathLength = br.ReadByte();
            this.ProcessPath = Encoding.UTF8.GetString(br.ReadBytes(pathLength));
                
            this.UserId = br.ReadUInt32();
                
            var userLength = br.ReadByte();
            this.UserName = Encoding.UTF8.GetString(br.ReadBytes(userLength));
                
            this.GroupId = br.ReadUInt32();
                
            var groupLength = br.ReadByte();
            this.GroupName = Encoding.UTF8.GetString(br.ReadBytes(groupLength));
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketDotNet.Ieee80211.PpiProcessInfo"/> class.
        /// </summary>
        public PpiProcessInfo ()
        {
                
        }

        #endregion Constructors
    }
}