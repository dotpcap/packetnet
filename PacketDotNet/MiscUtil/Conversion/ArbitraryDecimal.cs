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
        private int _decimalPoint;

        /// <summary>Digits in the decimal expansion, one byte per digit</summary>
        private byte[] _digits;

        /// <summary>
        /// Constructs an arbitrary decimal expansion from the given long.
        /// The long must not be negative.
        /// </summary>
        internal ArbitraryDecimal(long x)
        {
            var tmp = x.ToString(CultureInfo.InvariantCulture);
            _digits = new byte[tmp.Length];
            for (var i = 0; i < tmp.Length; i++)
                _digits[i] = (byte) (tmp[i] - '0');
            Normalize();
        }

        /// <summary>
        /// Multiplies the current expansion by the given amount, which should
        /// only be 2 or 5.
        /// </summary>
        internal void MultiplyBy(int amount)
        {
            var result = new byte[_digits.Length + 1];
            for (var i = _digits.Length - 1; i >= 0; i--)
            {
                var resultDigit = _digits[i] * amount + result[i + 1];
                result[i] = (byte) (resultDigit / 10);
                result[i + 1] = (byte) (resultDigit % 10);
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
        internal void Shift(int amount)
        {
            _decimalPoint += amount;
        }

        /// <summary>
        /// Removes leading/trailing zeroes from the expansion.
        /// </summary>
        private void Normalize()
        {
            int first;
            for (first = 0; first < _digits.Length; first++)
                if (_digits[first] != 0)
                    break;


            int last;
            for (last = _digits.Length - 1; last >= 0; last--)
                if (_digits[last] != 0)
                    break;


            if (first == 0 && last == _digits.Length - 1)
                return;


            var tmp = new byte[last - first + 1];
            for (var i = 0; i < tmp.Length; i++)
                tmp[i] = _digits[i + first];

            _decimalPoint -= _digits.Length - (last + 1);
            _digits = tmp;
        }

        /// <summary>
        /// Converts the value to a proper decimal string representation.
        /// </summary>
        public override string ToString()
        {
            var digitString = new char[_digits.Length];
            for (var i = 0; i < _digits.Length; i++)
                digitString[i] = (char) (_digits[i] + '0');

            // Simplest case - nothing after the decimal point,
            // and last real digit is non-zero, eg value=35
            if (_decimalPoint == 0)
            {
                return new string(digitString);
            }

            // Fairly simple case - nothing after the decimal
            // point, but some 0s to add, eg value=350
            if (_decimalPoint < 0)
            {
                return new string(digitString) +
                       new string('0', -_decimalPoint);
            }

            // Nothing before the decimal point, eg 0.035
            if (_decimalPoint >= digitString.Length)
            {
                return "0." +
                       new string('0', _decimalPoint - digitString.Length) +
                       new string(digitString);
            }

            // Most complicated case - part of the string comes
            // before the decimal point, part comes after it,
            // eg 3.5
            return new string(digitString,
                              0,
                              digitString.Length - _decimalPoint) +
                   "." +
                   new string(digitString,
                              digitString.Length - _decimalPoint,
                              _decimalPoint);
        }
    }
}