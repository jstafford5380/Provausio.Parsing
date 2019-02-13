using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provausio.Parsing.Csv.Csv;
using Provausio.Parsing.Csv.Mapping.Mutators;
using Newtonsoft.Json;

namespace Provausio.Parsing.Csv.Mapping
{
    public class CsvMapper : IDisposable
    {
        private readonly CsvParser _parser;
        private readonly Stream _outStream;
        private IDictionary<string, IncomingFormat> _configuration;
        private readonly IMutatorFactory _mutatorFactory;
        private bool _isDisposed;

        public CsvMapper(CsvParser parser, Stream outStream, IMutatorFactory mutatorFactory = null)
        {
            _parser = parser;
            _outStream = outStream;
            _mutatorFactory = mutatorFactory ?? new MutatorFactory();
        }

        [ExcludeFromCodeCoverage]
        public void Load(string path)
        {
            // TODO: Validate: Scan config file to make sure there are mutators for all mutatorInfos?

            var json = File.ReadAllText(path);
            _configuration = JsonConvert.DeserializeObject<Dictionary<string, IncomingFormat>>(json);
        }

        public void Load(IDictionary<string, IncomingFormat> configuration)
        {
            if(configuration == null || !configuration.Any()) throw new ArgumentNullException(nameof(configuration), "Configuration was null or empty.");
            _configuration = configuration;
        } 

        public async Task RemapAsync(char outputDelimiter, int lineBufferSize = 4000)
        {
            if(_isDisposed)
                throw new ObjectDisposedException(nameof(CsvMapper));

            var currentLine = new List<string>();
            var wroteHeader = false;

            var captureIndexes = new List<int>();
            var lineBuffer = new LineBuffer(_outStream, lineBufferSize);

            while (_parser.Read()) // each record
            {
                if (!wroteHeader) 
                {
                    WriteHeaders(_parser, outputDelimiter, lineBuffer, captureIndexes);
                    wroteHeader = true;
                }
                
                currentLine.Clear();
                for (var i = 0; i < _parser.CurrentRecord.Length; i++) // process each field
                {
                    // skip if we don't have any info for the field
                    if (!_configuration.TryGetValue(_parser.Headers[i], out var format) || 
                        !format.IsMapped ||
                        !captureIndexes.Contains(i)) continue;

                    currentLine.Add($"{(_parser.UsingQuotedFields ? $"\"{Mutate(_parser.CurrentRecord[i], format)}\"" : Mutate(_parser.CurrentRecord[i], format))}");
                }
                
                await lineBuffer.AddAsync(string.Join(outputDelimiter, currentLine));
            }

            await lineBuffer.FlushAsync();
        }

        private void WriteHeaders(CsvParser parser, char outputDelimiter, LineBuffer buffer, ICollection<int> captureIndexes)
        {
            var outHeaders = new List<string>();
            for (var i = 0; i < parser.Headers.Length; i++)
            {
                // only get the headers for which we have mappings
                if (!_configuration.TryGetValue(parser.Headers[i], out var format) || !format.IsMapped) continue;
                outHeaders.Add(format.Destination);
                captureIndexes.Add(i);
            }

            buffer.AddAsync(string.Join(outputDelimiter, outHeaders)).Wait();
        }

        private string Mutate(string value, IncomingFormat format)
        {
            // TODO: simplify this and just use any registered mutator instead of trying to match on type (decouple them)

            if (format.MutatorInfo == null || !format.MutatorInfo.TryGetValue(format.Type, out var mutatorSettings))
                return value;

            var mutateFunc = _mutatorFactory.GetMutator(format.Type, mutatorSettings);
            return mutateFunc == null ? value : mutateFunc(value);
        }

        private class LineBuffer
        {
            private readonly Stream _outStream;
            private readonly int _size;
            private int _lineCount;
            private readonly StringBuilder _buffer = new StringBuilder();

            public LineBuffer(Stream outStream, int size)
            {
                _outStream = outStream;
                _size = size;
            }

            public Task AddAsync(string line)
            {
                _buffer.AppendLine(line);
                _lineCount++;
                return Update();
            }

            public async Task FlushAsync()
            {
                var bytes = Encoding.UTF8.GetBytes(_buffer.ToString());
                await _outStream.WriteAsync(bytes);

                _buffer.Clear();
                _lineCount = 0;
            }

            private Task Update()
            {
                return _lineCount < _size 
                    ? Task.CompletedTask 
                    : FlushAsync();
            }
        }

        public void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;

            _parser?.Dispose();
            _outStream?.Dispose();
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}