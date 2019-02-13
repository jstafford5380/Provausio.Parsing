using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Provausio.Parsing.Csv.Csv
{
    public class CsvParser : ParsedFieldReader
    {
        private readonly ITextFieldParser _fieldParser;
        private Action<(string[], string[])> _onEachLine;

        public bool UsingHeaders { get; private set; }

        public bool UsingQuotedFields => _fieldParser.HasQuotedFields;

        public string[] Headers { get; private set; }

        public string[] CurrentRecord { get; private set; }


        public CsvParser(Stream stream, ITextFieldParser parser = null)
        {
            _fieldParser = parser ?? new TextFieldParser(new StreamReader(stream));
            _fieldParser.SetDelimiters(",");
        }

        public CsvParser(string data, ITextFieldParser parser = null) 
            : this(new MemoryStream(Encoding.UTF8.GetBytes(data)), parser)
        {
        }

        public CsvParser UseDelimiters(params string[] delimiters)
        {
            _fieldParser.SetDelimiters(delimiters);
            return this;
        }

        public CsvParser UseQuotedFields()
        {
            _fieldParser.HasQuotedFields = true;
            return this;
        }

        public CsvParser UseHeaders(string[] headers = null)
        {
            Headers = headers;
            UsingHeaders = true;
            return this;
        }

        public CsvParser OnEachLine(Action<(string[] Headers, string[] CurrentLine)> action)
        {
            _onEachLine = action;
            return this;
        }

        public override bool Read(bool throwOnError = false)
        {
            if (_fieldParser.EndOfData) return false;

            CurrentRecord = null;
            try
            {
                // capture headers then advance one line
                if (UsingHeaders && Headers == null)
                {
                    Headers = _fieldParser.ReadFields().ToArray();
                }
                
                CurrentRecord = _fieldParser.ReadFields().ToArray();
                
                if (Headers == null) // if we're still null then just assign numbers as headers
                    Headers = Enumerable.Range(0, CurrentRecord.Length).Select(i => i.ToString()).ToArray();
                
                AssertLengths();

                _onEachLine?.Invoke((Headers, CurrentRecord));

                return true;
            }
            catch (Exception e)
            {
                var error = new ParseError
                {
                    LineNumber = LineNumber,
                    Exception = e,
                    Message = e.Message
                };

                AddError(error);

                if (throwOnError) throw;
                return false;
            }
        }

        public string ToJson()
        {
            // zipper the two together
            var dict = new Dictionary<string, string>();
            for (var i = 0; i < Headers.Length; i++)
                dict.Add(Headers[i], CurrentRecord[i]);

            return JsonConvert.SerializeObject(dict);
        }

        private void AssertLengths()
        {
            if(Headers.Length != CurrentRecord.Length)
                throw new FormatException("The number of fields does not match the number of headers!");
        }

        [ExcludeFromCodeCoverage]
        protected override void Dispose(bool disposing)
        {            
            _fieldParser.Dispose();
            base.Dispose(disposing);
        }
    }
}
