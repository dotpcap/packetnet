/*
This file is part of PacketDotNet.

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.
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