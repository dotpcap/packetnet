using System;
using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet.Utils;

namespace PacketDotNet.LLDP
{
    /// <summary>
    /// A Chassis ID TLV
    /// </summary>
    public class ChassisID : TLV
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        /// <summary>
        /// Length of the sub type field in bytes
        /// </summary>
        private const int SubTypeLength = 1;

        #region Constructors

        /// <summary>
        /// Creates a Chassis ID TLV by parsing a byte[]
        /// </summary>
        /// <param name="bytes">
        /// </param>
        /// <param name="offset">
        /// The Chassis ID TLV's offset from the
        /// origin of the LLDP
        /// </param>
        public ChassisID(byte[] bytes, int offset) :
            base(bytes, offset)
        {
            log.Debug("");
        }

        /// <summary>
        /// Creates a Chassis ID TLV and sets it value
        /// </summary>
        /// <param name="subType">
        /// The ChassisID subtype
        /// </param>
        /// <param name="subTypeValue">
        /// The subtype's value
        /// </param>
        public ChassisID(ChassisSubTypes subType, object subTypeValue)
        {
            log.DebugFormat("subType {0}", subType);

            EmptyTLVDataInit();

            Type = TLVTypes.ChassisID;

            SubType = subType;

            // method will resize the tlv
            SubTypeValue = subTypeValue;
        }

        /// <summary>
        /// Create a ChassisID given a mac address
        /// </summary>
        /// <param name="MACAddress">
        /// A <see cref="PhysicalAddress"/>
        /// </param>
        public ChassisID(PhysicalAddress MACAddress)
        {
            log.DebugFormat("MACAddress {0}", MACAddress.ToString());

            EmptyTLVDataInit();

            Type = TLVTypes.ChassisID;
            SubType = ChassisSubTypes.MACAddress;

            SubTypeValue = MACAddress;
        }

        /// <summary>
        /// Create a ChassisID given an interface name
        /// http://tools.ietf.org/search/rfc2863 page 38
        /// </summary>
        /// <param name="InterfaceName">
        /// A <see cref="System.String"/>
        /// </param>
        public ChassisID(string InterfaceName)
        {
            log.DebugFormat("InterfaceName {0}", InterfaceName);

            EmptyTLVDataInit();

            Type = TLVTypes.ChassisID;
            SubType = ChassisSubTypes.InterfaceName;

            SetSubTypeValue(InterfaceName);
        }

        #endregion

        #region Properties

        /// <value>
        /// The type of the TLV subtype
        /// </value>
        public ChassisSubTypes SubType
        {
            get
            {
                return (ChassisSubTypes)tlvData.Bytes[ValueOffset];
            }

            set
            {
                // set the subtype
                tlvData.Bytes[ValueOffset] = (byte)value;
            }
        }

        /// <value>
        /// The TLV subtype value
        /// </value>
        public object SubTypeValue
        {
            get { return GetSubTypeValue(); }
            set { SetSubTypeValue(value); }
        }

        /// <summary>
        /// If SubType is ChassisComponent
        /// </summary>
        public byte[] ChassisComponent
        {
            get { return (byte[])GetSubTypeValue(); }
            set
            {
                SubType = ChassisSubTypes.ChassisComponent;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceName the interface name
        /// </summary>
        public string InterfaceName
        {
            get { return (string)GetSubTypeValue(); }
            set
            {
                SubType = ChassisSubTypes.InterfaceName;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is MACAddress the mac address
        /// </summary>
        public PhysicalAddress MACAddress
        {
            get { return (PhysicalAddress)GetSubTypeValue(); }
            set
            {
                SubType = ChassisSubTypes.MACAddress;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is NetworkAddress the network address
        /// </summary>
        public LLDP.NetworkAddress NetworkAddress
        {
            get { return (LLDP.NetworkAddress)GetSubTypeValue(); }
            set
            {
                SubType = ChassisSubTypes.NetworkAddress;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is PortComponent
        /// </summary>
        public byte[] PortComponent
        {
            get { return (byte[])GetSubTypeValue(); }
            set
            {
                SubType = ChassisSubTypes.PortComponent;
                SetSubTypeValue(value);
            }
        }

        /// <summary>
        /// If SubType is InterfaceAlias
        /// </summary>
        public byte[] InterfaceAlias
        {
            get { return (byte[])GetSubTypeValue(); }
            set
            {
                SubType = ChassisSubTypes.InterfaceAlias;
                SetSubTypeValue(value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Helper method to reduce duplication in type specific constructors
        /// </summary>
        private void EmptyTLVDataInit()
        {
            var length = TLVTypeLength.TypeLengthLength + SubTypeLength;
            var bytes = new byte[length];
            int offset = 0;
            tlvData = new ByteArraySegment(bytes, offset, length);
        }

        private object GetSubTypeValue()
        {
            byte[] val;
            int dataOffset = ValueOffset + SubTypeLength;
            int dataLength = Length - SubTypeLength;

            switch (SubType)
            {
                case ChassisSubTypes.ChassisComponent:
                case ChassisSubTypes.InterfaceAlias:
                case ChassisSubTypes.LocallyAssigned:
                case ChassisSubTypes.PortComponent:
                    val = new byte[dataLength];
                    Array.Copy(tlvData.Bytes, dataOffset,
                               val, 0,
                               dataLength);
                    return val;
                case ChassisSubTypes.NetworkAddress:
                    return new NetworkAddress(tlvData.Bytes,
                                              dataOffset,
                                              dataLength);
                case ChassisSubTypes.MACAddress:
                    val = new byte[dataLength];
                    Array.Copy(tlvData.Bytes, dataOffset,
                               val, 0,
                               dataLength);
                    return new PhysicalAddress(val);
                case ChassisSubTypes.InterfaceName:
                    return System.Text.ASCIIEncoding.ASCII.GetString(tlvData.Bytes, dataOffset, dataLength);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(object val)
        {
            byte[] valBytes;

            // make sure we have the correct type
            switch (SubType)
            {
                case ChassisSubTypes.ChassisComponent:
                case ChassisSubTypes.InterfaceAlias:
                case ChassisSubTypes.LocallyAssigned:
                case ChassisSubTypes.PortComponent:
                    if(!(val is byte[]))
                    {
                        throw new ArgumentOutOfRangeException("expected byte[] for type");
                    }

                    valBytes = (byte[])val;

                    SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.NetworkAddress:
                    if(!(val is NetworkAddress))
                    {
                        throw new ArgumentOutOfRangeException("expected NetworkAddress instance for NetworkAddress");
                    }

                    valBytes = ((NetworkAddress)val).Bytes;

                    SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.InterfaceName:
                    if(!(val is string))
                    {
                        throw new ArgumentOutOfRangeException("expected string for InterfaceName");
                    }

                    var interfaceName = (string)val;

                    valBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(interfaceName);

                    SetSubTypeValue(valBytes);
                    break;
                case ChassisSubTypes.MACAddress:
                    if(!(val is PhysicalAddress))
                    {
                        throw new ArgumentOutOfRangeException("expected PhysicalAddress for MACAddress");
                    }

                    var physicalAddress = (PhysicalAddress)val;

                    SetSubTypeValue(physicalAddress.GetAddressBytes());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetSubTypeValue(byte[] subTypeValue)
        {
            // is the length different than the current length?
            if(subTypeValue.Length != Length)
            {
                var headerLength = TLVTypeLength.TypeLengthLength + SubTypeLength;
                var newTlvMemory = new byte[headerLength + subTypeValue.Length];

                // copy the header data over
                Array.Copy(tlvData.Bytes, tlvData.Offset, newTlvMemory, 0, headerLength);

                // update the tlv memory pointer, offset and length
                tlvData = new ByteArraySegment(newTlvMemory, 0, newTlvMemory.Length);
            }

            Array.Copy(subTypeValue, 0, tlvData.Bytes, ValueOffset + SubTypeLength,
                       subTypeValue.Length);
        }

        /// <summary>
        /// Convert this Chassis ID TLV to a string.
        /// </summary>
        /// <returns>
        /// A human readable string
        /// </returns>
        public override string ToString ()
        {
            return string.Format("[ChassisID: SubType={0}, SubTypeValue={1}]", SubType, SubTypeValue);
        }
        
        #endregion
    }
}