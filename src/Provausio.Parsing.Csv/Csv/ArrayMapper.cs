using System.Collections.Generic;

namespace Provausio.Parsing.Csv.Csv
{
    public class ArrayMapper : IArrayMapper
    {
        private readonly MappedList _mappedList;
        private readonly CsvParser _parser;

        public IEnumerable<string> CurrentRecord => GetCurrentRecord();

        public ArrayMapper(CsvParser parser, IDictionary<int, int> map)
        {
            _parser = parser;
            _mappedList = new MappedList(map);
        }

        private IEnumerable<string> GetCurrentRecord()
        {
            _mappedList.SetValues(_parser.CurrentRecord);
            return _mappedList;
        }
    }
}