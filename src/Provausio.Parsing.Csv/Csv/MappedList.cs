using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Provausio.Parsing.Csv.Csv
{
    public class MappedList : IEnumerable<string>
    {
        private readonly IDictionary<int, int> _map;
        private readonly MappedEnumerator _enumerator;
        private string[] _values;

        public string this[int index] => _values[_map[index]];

        public int Count => _values.Length;

        public MappedList(IDictionary<int, int> map)
        {
            _map = map;
            _enumerator = new MappedEnumerator(_map);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _enumerator;
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SetValues(string[] values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            _enumerator.SetInternalList(values);
        }

        public bool Contains(string item)
        {
            return _values.Contains(item);
        }
    }
}