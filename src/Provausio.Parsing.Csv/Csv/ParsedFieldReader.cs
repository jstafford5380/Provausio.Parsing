using System;
using System.Collections.Generic;

namespace Provausio.Parsing.Csv.Csv
{
    public abstract class ParsedFieldReader : IDisposable
    {
        private readonly List<ParseError> _errors = new List<ParseError>();
        private bool _isDisposed;

        /// <summary>
        /// Last read line number
        /// </summary>
        public long LineNumber { get; protected set; }

        /// <summary>
        /// Specifies whether or  not the reader should throw when it encounters an error. If set to false, it will continue, but collect a list of errors in the 'ParseErrors' property
        /// </summary>
        public bool BreakOnError { get; set; }

        /// <summary>
        /// A collection of errors that were encountered during the read
        /// </summary>
        public IEnumerable<ParseError> ParseErrors => _errors;

        /// <summary>
        /// Reads the next line.
        /// </summary>
        /// <returns></returns>
        public abstract bool Read(bool throwOnError = false);

        protected void AddError(ParseError error)
        {
            _errors.Add(error);
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }

        public void Dispose()
        { 
            Dispose(true);            
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed) return;
            _isDisposed = true;
        }
    }
}
