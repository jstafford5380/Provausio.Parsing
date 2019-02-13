using System;
using System.Diagnostics.CodeAnalysis;

namespace Provausio.Parsing.Csv.Csv
{
    [ExcludeFromCodeCoverage]
    public class ParseError
    {
        /// <summary>
        /// The approximate line number that was read which caused the error
        /// </summary>
        public long LineNumber { get; set; }

        /// <summary>
        /// Message of the error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The stack trace, if any.
        /// </summary>
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return $"Line {LineNumber}: {Message}";
        }
    }
}
