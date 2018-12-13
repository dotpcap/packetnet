using System;
using System.Globalization;

namespace PacketDotNet.MiscUtil.Conversion
{
    /// <summary>
    /// Used for manipulating sequences of decimal digits.
    /// </summary>
    public class ArbitraryDecimal
    {
        /// <summary>
        /// How many digits are *after* the decimal point
        /// </summary>
        private Int32 _decimalPoint;

        /// <summary>Digits in the decimal expansion, one byte per digit</summary>
        private Byte[] _digits;

        /// <summary>
        /// Constructs an arbitrary decimal expansion from the given long.
        /// The long must not be negative.
        /// </summary>
        internal ArbitraryDecimal(Int64 x)
        {
            var tmp = x.ToString(CultureInfo.InvariantCulture);
            _digits = new Byte[tmp.Length];
            for (var i = 0; i < tmp.Length; i++)
                _digits[i] = (Byte) (tmp[i] - '0');
            Normalize();
        }

        /// <summary>
        /// Multiplies the current expansion by the given amount, which should
        /// only be 2 or 5.
        /// </summary>
        internal void MultiplyBy(Int32 amount)
        {
            var result = new Byte[_digits.Length + 1];
            for (var i = _digits.Length - 1; i >= 0; i--)
            {
                var resultDigit = _digits[i] * amount + result[i + 1];
                result[i] = (Byte) (resultDigit / 10);
                result[i + 1] = (Byte) (resultDigit % 10);
            }

            if (result[0] != 0)
            {
                _digits = result;
            }
            else
            {
                Array.Copy(result, 1, _digits, 0, _digits.Length);
            }

            Normalize();
        }

        /// <summary>
        /// Shifts the decimal point; a negative value makes
        /// the decimal expansion bigger (as fewer digits come after the
        /// decimal place) and a positive value makes the decimal
        /// expansion smaller.
        /// </summary>
        internal void Shift(Int32 amount)
        {
            _decimalPoint += amount;
        }

        /// <summary>
        /// Removes leading/trailing zeroes from the expansion.
        /// </summary>
        private void Normalize()
        {
            Int32 first;
            for (first = 0; first < _digits.Length; first++)
                if (_digits[first] != 0)
                    break;


            Int32 last;
            for (last = _digits.Length - 1; last >= 0; last--)
                if (_digits[last] != 0)
                    break;


            if (first == 0 && last == _digits.Length - 1)
                return;


            var tmp = new Byte[last - first + 1];
            for (var i = 0; i < tmp.Length; i++)
                tmp[i] = _digits[i + first];

            _decimalPoint -= _digits.Length - (last + 1);
            _digits = tmp;
        }

        /// <summary>
        /// Converts the value to a proper decimal string representation.
        /// </summary>
        public override String ToString()
        {
            var digitString = new Char[_digits.Length];
            for (var i = 0; i < _digits.Length; i++)
                digitString[i] = (Char) (_digits[i] + '0');

            // Simplest case - nothing after the decimal point,
            // and last real digit is non-zero, eg value=35
            if (_decimalPoint == 0)
            {
                return new String(digitString);
            }

            // Fairly simple case - nothing after the decimal
            // point, but some 0s to add, eg value=350
            if (_decimalPoint < 0)
            {
                return new String(digitString) +
                       new String('0', -_decimalPoint);
            }

            // Nothing before the decimal point, eg 0.035
            if (_decimalPoint >= digitString.Length)
            {
                return "0." +
                       new String('0', _decimalPoint - digitString.Length) +
                       new String(digitString);
            }

            // Most complicated case - part of the string comes
            // before the decimal point, part comes after it,
            // eg 3.5
            return new String(digitString,
                              0,
                              digitString.Length - _decimalPoint) +
                   "." +
                   new String(digitString,
                              digitString.Length - _decimalPoint,
                              _decimalPoint);
        }
    }
}