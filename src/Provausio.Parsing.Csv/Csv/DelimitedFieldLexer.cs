using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Provausio.Parsing.Csv.Csv
{
    /// <summary>
    /// Default implementation of field lexer reads stream and returns field values.
    /// </summary>
    internal class DelimitedFieldLexer : IFieldLexer
    {
        private const int NoData = -1;
        private const int Delimiter = -2;
        private const int Initialize = -3;
        private const int EndOfLine = 0x0A;
        private const int WindowsEndOfLine = 0x0D;
        private const int Quote = 0x22;
        private const LexingStates BreakCondition = LexingStates.NoData | LexingStates.EndOfLine | LexingStates.StartingNextField;

        private readonly StringBuilder _buffer;
        private readonly TextReader _reader;

        private int[] _delimiters;
        private LexingStates _state;

        public bool HasMoreFields => _state != LexingStates.EndOfLine && _reader.Peek() != NoData;

        public DelimitedFieldLexer(TextReader reader)
        {
            _reader = reader;
            _buffer = new StringBuilder();
            _delimiters = new[] {(int)','};
        }

        public void SetDelimiters(params char[] delimiters)
        {
            if(delimiters == null || !delimiters.Any())
                throw new ArgumentNullException(nameof(delimiters), "Cannot set empty delimiter set.");

            _delimiters = delimiters
                .Select(delimiter => (int)delimiter)
                .ToArray();
        }

        public string GetNextField()
        {
            UpdateState(Initialize);

            while ((_state & BreakCondition) != _state)
            {
                var currentValue = _reader.Read();
                UpdateState(currentValue);

                switch (_state)
                {
                    case LexingStates.WindowsEndOfLine:
                        // just skip because the next character is probably going to be \n
                        continue;
                    case LexingStates.ReadingField:
                        _buffer.Append((char)currentValue);
                        break;
                    case LexingStates.StartingNextField:
                    case LexingStates.EndOfLine:
                    case LexingStates.NoData:
                        break;
                    case LexingStates.ReadingQuoted:
                        currentValue = _reader.Read();
                        while (currentValue != Quote)
                        {
                            _buffer.Append((char)currentValue);
                            currentValue = _reader.Read();
                        }
                        _state = LexingStates.ReadingField;
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown state '{_state}'");
                }
            }

            var field = _buffer.ToString();
            _buffer.Clear();

            return field;
        }

        private void UpdateState(int value)
        {
            // This switch is marginally faster than a dictionary and significantly faster than Hashtable

            value = _delimiters.Contains(value) ? Delimiter : value;

            switch (value)
            {
                case NoData:
                    _state = LexingStates.NoData;
                    break;
                case Delimiter:
                    _state = LexingStates.StartingNextField;
                    break;
                case EndOfLine:
                    _state = LexingStates.EndOfLine;
                    break;
                case WindowsEndOfLine:
                    _state = LexingStates.WindowsEndOfLine;
                    break;
                case Quote:
                    _state = LexingStates.ReadingQuoted;
                    break;
                default:
                    _state = LexingStates.ReadingField;
                    break;
            }
        }

        [Flags]
        private enum LexingStates
        {
            ReadingField = 1,
            ReadingQuoted = 1 << 1,
            StartingNextField = 1 << 2,
            WindowsEndOfLine = 1 << 3,
            EndOfLine = 1 << 4,
            NoData = 1 << 5
        }
    }
}