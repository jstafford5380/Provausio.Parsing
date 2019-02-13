using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Provausio.Parsing.Csv.Csv;
using Xunit;

namespace Provausio.Parsing.Csv.Tests.Csv
{
    public class Usage
    {
        private readonly string _testData;

        public Usage()
        {
            _testData = "field1,field2,field3\r\nfoo,bar,baz\r\nfoo1,bar1,baz1";
        }

        [Fact]
        public void Test()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(_testData));
            var parser = new CsvParser(stream)
                .UseHeaders()
                .UseQuotedFields()
                .UseDelimiters(",");

            while (parser.Read())
            {
                Console.Out.Write(parser.ToJson());
            }
        }

        [Fact]
        public void TestMapper()
        {
            var map = new Dictionary<int, int> {{2, 0}, {1, 1}, {0, 2}};

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(_testData));
            var parser = new CsvParser(stream).UseHeaders();
            var mapper = new ArrayMapper(parser, map);

            while (parser.Read())
            {
                var y = parser.CurrentRecord;
                var x = mapper.CurrentRecord;
            }
        }
    }
}
