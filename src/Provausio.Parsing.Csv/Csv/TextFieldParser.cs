using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Provausio.Parsing.Csv.Csv
{
    internal class TextFieldParser : ITextFieldParser
    {
        private readonly StreamReader _reader;
        private readonly IFieldLexer _lexer;

        [ExcludeFromCodeCoverage]
        public Stream BaseStream => _reader.BaseStream;

        public bool EndOfData => _reader.EndOfStream;

        public bool HasQuotedFields { get; set; }

        public long LineNumber { get; set; }

        public TextFieldParser(StreamReader reader)
            : this(reader, new DelimitedFieldLexer(reader))
        {
        }

        public TextFieldParser(StreamReader reader, IFieldLexer lexer)
        {
            _reader = reader;
            _lexer = lexer;
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the delimiters
        /// </summary>
        /// <param name="delimiters"></param>
        public void SetDelimiters(params string[] delimiters)
        {
            if (delimiters.Any(character => character.Length > 1))
                throw new ArgumentException("Delimiters can only be 1 character long.");

            _lexer.SetDelimiters(delimiters.Select(delimiter => delimiter[0]).ToArray());
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the fields from the next line in the stream
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> ReadFields()
        {
            if (EndOfData)
                throw new InvalidOperationException("Reached end of stream.");

            while (_lexer.HasMoreFields || !EndOfData)
            {
                var field = _lexer.GetNextField();
                LineNumber++;
                yield return field;

                if (!_lexer.HasMoreFields) // this is to make up for the awkward loop condition
                    break;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _reader.Dispose();
        }
    }
}
