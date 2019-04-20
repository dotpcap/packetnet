using System;

namespace Test
{
    public class ByteSetupMethods
    {
        public static void Setup(out byte[] bytes, out int testRuns, out int startIndex,
                            out int expectedValue)
        {
            testRuns = 40000000; // needs to be enough runs, taking at least seconds, to get
                                 // an accurate result

            // presume that bytes contains a network ordered 32bit value
            bytes = new byte[5];
            bytes[0] = 0xFF;
            bytes[1] = 0x0A;
            bytes[2] = 0x0B;
            bytes[3] = 0x0C;
            bytes[4] = 0x0D;
            startIndex = 1; // we start at offset 1

            expectedValue = 0x0A0B0C0D;
        }
    }
}
