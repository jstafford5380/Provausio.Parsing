using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Provausio.Parsing.Csv.Csv;
using Provausio.Parsing.Csv.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Provausio.Parsing.Csv.Tests.Mapping
{
    public class CsvMapperTests
    {
        private readonly MemoryStream _outStream;

        public CsvMapperTests()
        {
            _outStream = new MemoryStream();
        }

        [Fact]
        public void Load_NullConfig_Throws()
        {
            // arrange
            var parser = new CsvParser("Foo,Bar,Baz");
            var mapper = new CsvMapper(parser, _outStream);

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => mapper.Load((IDictionary<string, IncomingFormat>) null));
        }

        [Fact]
        public async Task Remap_Writes_Stream()
        {
            // arrange
            var config = GetTestConfig();
            var parser = GetTestParser().UseHeaders();
            var mapper = new CsvMapper(parser, _outStream);
            mapper.Load(config);

            // act
            await mapper.RemapAsync(',');

            // assert
            Assert.NotEqual(0, _outStream.Length);
        }

        [Fact]
        public async Task Remap_Writes_TransformedFields()
        {
            // arrange
            var config = GetTestConfig();
            var parser = GetTestParser().UseHeaders();
            var mapper = new CsvMapper(parser, _outStream);
            mapper.Load(config);

            // act
            await mapper.RemapAsync(',');

            // assert
            _outStream.Seek(0, SeekOrigin.Begin);
            var asText = Encoding.UTF8.GetString(_outStream.ToArray());
            var firstLine = asText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[0];
            var fields = firstLine.Split(',');
            Assert.Equal("foo-1", fields[0]);
            Assert.Equal("bar-1", fields[1]);
            Assert.Equal("baz-1", fields[2]);
        }

        [Fact]
        public async Task Remap_IgnoresUnmapped()
        {
            // arrange
            var config = GetTestConfig();
            var parser = GetTestParser().UseHeaders();
            var mapper = new CsvMapper(parser, _outStream);
            mapper.Load(config);
            var outStream = new MemoryStream();

            // act
            await mapper.RemapAsync(',');
            outStream.Seek(0, SeekOrigin.Begin);
            var asText = Encoding.UTF8.GetString(outStream.ToArray());

            // assert
            Assert.DoesNotContain("bat", asText);
        }

        [Fact]
        public async Task Remap_MutatorWasRun()
        {
            // arrange
            var config = GetTestConfig();
            var parser = GetTestParser().UseHeaders();
            var mapper = new CsvMapper(parser, _outStream);
            mapper.Load(config);

            // act
            await mapper.RemapAsync(',');


            // assert
            _outStream.Seek(0, SeekOrigin.Begin);
            var asText = Encoding.UTF8.GetString(_outStream.ToArray());
            var lines = asText.Split(Environment.NewLine);
            var firstRecord = lines[1];
            var fields = firstRecord.Split(',');

            Assert.Equal("unknown", fields[0]);
        }

        private static IDictionary<string, IncomingFormat> GetTestConfig()
        {
            var mutatorInfo = "{ \"New\": \"new\", \"Used\": \"used\", \"default\": \"unknown\" }";
            var valueJObj = JsonConvert.DeserializeObject<JObject>(mutatorInfo);
            return new Dictionary<string, IncomingFormat>
            {
                {
                    "Foo",
                    new IncomingFormat {Destination = "foo-1", ExpectNull = false, IsMapped = true, Type = "enum", MutatorInfo = new Dictionary<string, JObject>
                    {
                        {"enum", valueJObj}
                    }}
                },
                {
                    "Bar",
                    new IncomingFormat {Destination = "bar-1", ExpectNull = false, IsMapped = true, Type = "string"}
                },
                {
                    "Baz",
                    new IncomingFormat {Destination = "baz-1", ExpectNull = false, IsMapped = true, Type = "string"}
                },
                {
                    "Bat",
                    new IncomingFormat {IsMapped = false}
                }
            };
        }

        private static CsvParser GetTestParser(string delimiter = ",")
        {
            var headers = new List<string>{"Foo", "Bar", "Baz", "Bat"};
            var line1 = new List<string>{"1a", "2a", "3a", "4a"};
            var line2 = new List<string>{"2b", "2b", "2b", "4b"};
            
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine(string.Join(delimiter, headers));
            writer.WriteLine(string.Join(delimiter, line1));
            writer.WriteLine(string.Join(delimiter, line2));
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            return new CsvParser(stream).UseDelimiters(delimiter);
        }
    }
}
