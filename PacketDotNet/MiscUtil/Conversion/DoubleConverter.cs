using System;

namespace PacketDotNet.MiscUtil.Conversion
{
    /// <summary>
    /// A class to allow the conversion of doubles to string representations of
    /// their exact decimal values. The implementation aims for readability over
    /// efficiency.
    /// </summary>
    public class DoubleConverter
    {
        /// <summary>
        /// Converts the given double to a string representation of its
        /// exact decimal value.
        /// </summary>
        /// <param name="d">The double to convert.</param>
        /// <returns>A string representation of the double's exact decimal value.</returns>
        public static String ToExactString(Double d)
        {
            if (Double.IsPositiveInfinity(d))
                return "+Infinity";
            if (Double.IsNegativeInfinity(d))
                return "-Infinity";
            if (Double.IsNaN(d))
                return "NaN";


            // Translate the double into sign, exponent and mantissa.
            var bits = BitConverter.DoubleToInt64Bits(d);
            var negative = bits < 0;
            var exponent = (Int32) ((bits >> 52) & 0x7ffL);
            var mantissa = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher,
            // but there's no extra normalisation bit in the mantissa
            if (exponent == 0)
            {
                exponent++;
            }
            // Normal numbers; leave exponent as it is but add extra
            // bit to the front of the mantissa
            else
            {
                mantissa = mantissa | (1L << 52);
            }

            // Bias the exponent. It's actually biased by 1023, but we're
            // treating the mantissa as m.0 rather than 0.m, so we need
            // to subtract another 52 from it.
            exponent -= 1075;

            if (mantissa == 0)
            {
                return "0";
            }

            /* Normalize */
            while ((mantissa & 1) == 0)
            {
                /*  i.e., Mantissa is even */
                mantissa >>= 1;
                exponent++;
            }

            // Construct a new decimal expansion with the mantissa
            var ad = new ArbitraryDecimal(mantissa);

            // If the exponent is less than 0, we need to repeatedly
            // divide by 2 - which is the equivalent of multiplying
            // by 5 and dividing by 10.
            if (exponent < 0)
            {
                for (var i = 0; i < -exponent; i++)
                    ad.MultiplyBy(5);
                ad.Shift(-exponent);
            }
            // Otherwise, we need to repeatedly multiply by 2
            else
            {
                for (var i = 0; i < exponent; i++)
                    ad.MultiplyBy(2);
            }

            // Finally, return the string with an appropriate sign
            if (negative)
                return "-" + ad;


            return ad.ToString();
        }
    }
}