#region Header

// Tamir Khason http://khason.net/
//
// Released under MS-PL : 6-Apr-09

#endregion Header

using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PacketDotNet.Utils
{
    /// <summary>Implements a 32-bits cyclic redundancy check (CRC) hash algorithm.</summary>
    /// <remarks>This class is not intended to be used for security purposes. For security applications use MD5, SHA1, SHA256, SHA384, 
    /// or SHA512 in the System.Security.Cryptography namespace.</remarks>
    public class Crc32 : HashAlgorithm
    {
        #region Fields

        /// <summary>Gets the default polynomial (used in WinZip, Ethernet, etc.)</summary>
        /// <remarks>The default polynomial is a bit-reflected version of the standard polynomial 0x04C11DB7 used by WinZip, Ethernet, etc.</remarks>
        public static readonly uint DefaultPolynomial = 0xEDB88320; // Bitwise reflection of 0x04C11DB7;
        private const uint _allOnes = 0xffffffff;
        private uint _crc;
        private readonly uint[] _crc32Table;
        private static readonly Hashtable _crc32TablesCache;
        private static readonly Crc32 _defaultCRC;

        #endregion Fields

        #region Constructors

        /// <summary>Creates a CRC32 object using the <see cref="DefaultPolynomial"/>.</summary>
        public Crc32()
            : this(DefaultPolynomial)
        {
        }

        /// <summary>Creates a CRC32 object using the specified polynomial.</summary>
        /// <remarks>The polynomial should be supplied in its bit-reflected form. <see cref="DefaultPolynomial"/>.</remarks>
        public Crc32(uint polynomial)
        {
            HashSizeValue = 32;
            _crc32Table = (uint[])_crc32TablesCache[polynomial];
            if (_crc32Table == null) {
                _crc32Table = BuildCrc32Table(polynomial);
                _crc32TablesCache.Add(polynomial, _crc32Table);
            }
            Initialize();
        }

        // static constructor
        static Crc32()
        {
            _crc32TablesCache = Hashtable.Synchronized(new Hashtable());
            _defaultCRC = new Crc32();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>Computes the CRC32 value for the given ASCII string using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(string asciiString)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(asciiString));
        }

        /// <summary>Computes the CRC32 value for the given input stream using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(Stream inputStream)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(inputStream));
        }

        /// <summary>Computes the CRC32 value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(byte[] buffer)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(buffer));
        }

        /// <summary>Computes the hash value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(byte[] buffer, int offset, int count)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(buffer, offset, count));
        }

        /// <summary>Computes the hash value for the given ASCII string.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public byte[] ComputeHash(string asciiString)
        {
            byte[] rawBytes = Encoding.ASCII.GetBytes(asciiString);
            return ComputeHash(rawBytes);
        }

        /// <summary>Computes the hash value for the given input stream.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public new byte[] ComputeHash(Stream inputStream)
        {
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0) {
                HashCore(buffer, 0, bytesRead);
            }
            return HashFinal();
        }

        /// <summary>Computes the hash value for the input data.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public new byte[] ComputeHash(byte[] buffer)
        {
            return ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>Computes the hash value for the input data.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public new byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            HashCore(buffer, offset, count);
            return HashFinal();
        }

        /// <summary>Initializes an implementation of HashAlgorithm.</summary>
        public override sealed void Initialize()
        {
            _crc = _allOnes;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>Routes data written to the object into the hash algorithm for computing the hash.</summary>
        protected override void HashCore(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++) {
                ulong ptr = (_crc & 0xFF) ^ buffer[i];
                _crc >>= 8;
                _crc ^= _crc32Table[ptr];
            }
        }

        /// <summary>Finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        protected override byte[] HashFinal()
        {
            var finalHash = new byte[4];
            ulong finalCrc = _crc ^ _allOnes;

            finalHash[3] = (byte)((finalCrc >> 0) & 0xFF);
            finalHash[2] = (byte)((finalCrc >> 8) & 0xFF);
            finalHash[1] = (byte)((finalCrc >> 16) & 0xFF);
            finalHash[0] = (byte)((finalCrc >> 24) & 0xFF);

            return finalHash;
        }

        #endregion Protected Methods

        #region Private Methods

        // Builds a crc32 table given a polynomial
        private static uint[] BuildCrc32Table(uint polynomial)
        {
            var table = new uint[256];

            // 256 values representing ASCII character codes.
            for (int i = 0; i < 256; i++) {
                var crc = (uint)i;
                for (int j = 8; j > 0; j--) {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                table[i] = crc;
            }

            return table;
        }

        private static int ToInt32(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }

        #endregion Private Methods
    }
}