using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Meziantou.DataGenerator.Utilities
{
    public class ConcatList<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _first;
        private readonly IReadOnlyList<T> _second;

        public ConcatList(IReadOnlyList<T> first, IReadOnlyList<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            _first = first;
            _second = second;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _first.Concat(_second).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection. 
        /// </returns>
        public int Count
        {
            get
            {
                return _first.Count + _second.Count;
            }
        }

        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <returns>
        /// The element at the specified index in the read-only list.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get. </param>
        public T this[int index]
        {
            get
            {
                if (index < _first.Count)
                {
                    return _first[index];
                }

                return _second[index - _first.Count];
            }
        }
    }
}
