using System;
using System.Collections;
using System.Collections.Generic;

namespace Provausio.Parsing.Csv.Csv
{
    public sealed class MappedEnumerator : IEnumerator<string>
    {
        private readonly IDictionary<int, int> _fieldMapping;
        private string[] _fields;
        private int _currentIndex;

        public MappedEnumerator(IDictionary<int, int> fieldMapping)
        {
            _fieldMapping = fieldMapping;
        }

        public MappedEnumerator(IDictionary<int, int> fieldMapping, string[] fields)
        {
            _fieldMapping = fieldMapping;
            _fields = fields;
            _currentIndex = -1;
        }

        public void SetInternalList(string[] newValues)
        {
            _fields = newValues;
            Reset();
        }

        public bool MoveNext()
        {
            if (++_currentIndex >= _fields.Length)
                return false;

            Current = _fields[_fieldMapping[_currentIndex]];
            return true;
        }

        public void Reset()
        {
            _currentIndex = -1;
            Current = null;
        }

        public string Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() 
        { 
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // nothing to dispose
        }
    }
}