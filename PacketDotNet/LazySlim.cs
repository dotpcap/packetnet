/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace PacketDotNet
{
    public class LazySlim<T> where T : class
    {
        private readonly Func<T> _valueFactory;
        private T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazySlim{T}" /> struct.
        /// </summary>
        /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        public LazySlim(Func<T> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        /// <summary>
        /// Gets the lazily initialized value of the current instance.
        /// </summary>
        public T Value
        {
            get
            {
                if (IsValueCreated)
                    return _value;


                return CreateValue();
            }
        }

        /// <summary>
        /// Gets a value indicating whether a value has been created for this instance;
        /// </summary>
        public bool IsValueCreated { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="T"/> using the value factory if it's not null, or using the <see cref="Activator"/> otherwise.
        /// </summary>
        /// <returns><see cref="T"/>.</returns>
        private T CreateValue()
        {
            if (_valueFactory != null)
            { 
                _value = _valueFactory?.Invoke();
                IsValueCreated = true;
            }
            else
            {
                _value = Activator.CreateInstance<T>();
                IsValueCreated = true;
            }

            return _value;
        }
    }
}