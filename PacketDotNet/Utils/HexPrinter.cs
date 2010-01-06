using System;
using System.Text;

namespace PacketDotNet.Utils
{
    /// <summary>
    /// Helper class that prints out an array of hex values
    /// </summary>
    public class HexPrinter
    {
        public static string GetString(byte[] Byte,
                                       int Offset,
                                       int Length)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = Offset; i < Offset + Length; i++)
            {
                sb.AppendFormat("[{0:x4}]", Byte[i]);
            }

            return sb.ToString();
        }
    }
}
