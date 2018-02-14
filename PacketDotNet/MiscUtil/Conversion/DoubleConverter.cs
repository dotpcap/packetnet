using System;
using System.Globalization;

namespace MiscUtil.Conversion
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
        public static String ToExactString (Double d)
        {
            if (Double.IsPositiveInfinity(d))
                return "+Infinity";
            if (Double.IsNegativeInfinity(d))
                return "-Infinity";
            if (Double.IsNaN(d))
                return "NaN";

            // Translate the double into sign, exponent and mantissa.
            Int64 bits = BitConverter.DoubleToInt64Bits(d);
            Boolean negative = (bits < 0);
            Int32 exponent = (Int32) ((bits >> 52) & 0x7ffL);
            Int64 mantissa = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher,
            // but there's no extra normalisation bit in the mantissa
            if (exponent==0)
            {
                exponent++;
            }
            // Normal numbers; leave exponent as it is but add extra
            // bit to the front of the mantissa
            else
            {
                mantissa = mantissa | (1L<<52);
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
            while((mantissa & 1) == 0)
            {    /*  i.e., Mantissa is even */
                mantissa >>= 1;
                exponent++;
            }

            // Construct a new decimal expansion with the mantissa
            ArbitraryDecimal ad = new ArbitraryDecimal (mantissa);

            // If the exponent is less than 0, we need to repeatedly
            // divide by 2 - which is the equivalent of multiplying
            // by 5 and dividing by 10.
            if (exponent < 0)
            {
                for (Int32 i=0; i < -exponent; i++)
                    ad.MultiplyBy(5);
                ad.Shift(-exponent);
            }
                // Otherwise, we need to repeatedly multiply by 2
            else
            {
                for (Int32 i=0; i < exponent; i++)
                    ad.MultiplyBy(2);
            }

            // Finally, return the string with an appropriate sign
            if (negative)
                return "-"+ad.ToString();
            else
                return ad.ToString();
        }

        /// <summary>
        /// Private class used for manipulating sequences of decimal digits.
        /// </summary>
        class ArbitraryDecimal
        {
            /// <summary>Digits in the decimal expansion, one byte per digit</summary>
            Byte[] digits;
            /// <summary>
            /// How many digits are *after* the decimal point
            /// </summary>
            Int32 decimalPoint=0;

            /// <summary>
            /// Constructs an arbitrary decimal expansion from the given long.
            /// The long must not be negative.
            /// </summary>
            internal ArbitraryDecimal (Int64 x)
            {
                String tmp = x.ToString(CultureInfo.InvariantCulture);
                this.digits = new Byte[tmp.Length];
                for (Int32 i=0; i < tmp.Length; i++) this.digits[i] = (Byte) (tmp[i]-'0');
                this.Normalize();
            }

            /// <summary>
            /// Multiplies the current expansion by the given amount, which should
            /// only be 2 or 5.
            /// </summary>
            internal void MultiplyBy(Int32 amount)
            {
                Byte[] result = new Byte[this.digits.Length+1];
                for (Int32 i= this.digits.Length-1; i >= 0; i--)
                {
                    Int32 resultDigit = this.digits[i]*amount+result[i+1];
                    result[i]=(Byte)(resultDigit/10);
                    result[i+1]=(Byte)(resultDigit%10);
                }
                if (result[0] != 0)
                {
                    this.digits=result;
                }
                else
                {
                    Array.Copy (result, 1, this.digits, 0, this.digits.Length);
                }

                this.Normalize();
            }

            /// <summary>
            /// Shifts the decimal point; a negative value makes
            /// the decimal expansion bigger (as fewer digits come after the
            /// decimal place) and a positive value makes the decimal
            /// expansion smaller.
            /// </summary>
            internal void Shift (Int32 amount)
            {
                this.decimalPoint += amount;
            }

            /// <summary>
            /// Removes leading/trailing zeroes from the expansion.
            /// </summary>
            internal void Normalize()
            {
                Int32 first;
                for (first=0; first < this.digits.Length; first++)
                    if (this.digits[first]!=0)
                        break;
                Int32 last;
                for (last= this.digits.Length-1; last >= 0; last--)
                    if (this.digits[last]!=0)
                        break;

                if (first==0 && last== this.digits.Length-1)
                    return;

                Byte[] tmp = new Byte[last-first+1];
                for (Int32 i=0; i < tmp.Length; i++)
                    tmp[i]= this.digits[i+first];

                this.decimalPoint -= this.digits.Length-(last+1);
                this.digits=tmp;
            }

            /// <summary>
            /// Converts the value to a proper decimal string representation.
            /// </summary>
            public override String ToString()
            {
                Char[] digitString = new Char[this.digits.Length];
                for (Int32 i=0; i < this.digits.Length; i++)
                    digitString[i] = (Char)(this.digits[i]+'0');

                // Simplest case - nothing after the decimal point,
                // and last real digit is non-zero, eg value=35
                if (this.decimalPoint==0)
                {
                    return new String (digitString);
                }

                // Fairly simple case - nothing after the decimal
                // point, but some 0s to add, eg value=350
                if (this.decimalPoint < 0)
                {
                    return new String (digitString)+
                        new String ('0', -this.decimalPoint);
                }

                // Nothing before the decimal point, eg 0.035
                if (this.decimalPoint >= digitString.Length)
                {
                    return "0."+
                        new String ('0',(this.decimalPoint-digitString.Length))+
                        new String (digitString);
                }

                // Most complicated case - part of the string comes
                // before the decimal point, part comes after it,
                // eg 3.5
                return new String (digitString, 0,
                    digitString.Length- this.decimalPoint)+
                    "."+
                    new String (digitString,
                    digitString.Length- this.decimalPoint, this.decimalPoint);
            }
        }
    }
}