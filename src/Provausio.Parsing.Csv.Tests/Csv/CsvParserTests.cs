using System;
using System.IO;
using System.Linq;
using Provausio.Parsing.Csv.Csv;
using Moq;
using Xunit;

namespace Provausio.Parsing.Csv.Tests.Csv
{
    public class CsvParserTests
    {
        private readonly string _testCsv;

        public CsvParserTests()
        {
            _testCsv = "FirstName,LastName\r\nFoo,Bar";
        }

        [Fact]
        public void Ctor_NullStream_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => new CsvParser((Stream) null));
        }

        [Fact]
        public void Ctor_NullString_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => new CsvParser((string) null));
        }

        [Fact]
        public void Read_NoHeaders_AssignNumbersAsHeaders()
        {
            // arrange
            var reader = new CsvParser(_testCsv);

            // act
            while (reader.Read()) { }

            // assert
            Assert.Equal("0", reader.Headers[0]);
            Assert.Equal("1", reader.Headers[1]);
        }

        [Fact]
        public void Read_ThrowOnError_Throws()
        {
            // arrange
            var parserMock = new Mock<ITextFieldParser>();
            parserMock.Setup(m => m.ReadFields()).Throws<ArgumentNullException>();
            var reader = new CsvParser(_testCsv, parserMock.Object);

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                while (reader.Read(true)) { }
            });
        }

        [Fact]
        public void Read_NoThrowOnError_CapturesError()
        {
            // arrange
            var parserMock = new Mock<ITextFieldParser>();
            parserMock.Setup(m => m.ReadFields()).Throws<ArgumentNullException>();
            var reader = new CsvParser(_testCsv, parserMock.Object);

            // act
            while (reader.Read()) {}

            // assert
            Assert.Single(reader.ParseErrors);
            Assert.IsType<ArgumentNullException>(reader.ParseErrors.First().Exception);
        }

        [Fact]
        public void Read_UnevenData_Throws()
        {
            // arrange
            const string csv = "Prop1,Prop2\r\nFoo";
            var reader = new CsvParser(csv);

            // act

            // assert
            Assert.Throws<FormatException>(() =>
            {
                while (reader.Read(true)) { }
            });
        }

        [Fact]
        public void Read_CustomDelimiters_Parses()
        {
            // arrange
            var csv = "Prop1|Prop2\r\nFoo|Bar";
            var reader = new CsvParser(csv);
            reader.UseDelimiters("|");

            // act
            while (reader.Read()) { }

            // assert
            Assert.Equal("Foo", reader.CurrentRecord[0]);
        }

        [Fact]
        public void Read_OnEachLine_ExecutesAction()
        {
            // arrange
            var csv = "Prop1|Prop2\r\nFoo|Bar";
            var changed = false;
            var reader = new CsvParser(csv);
            reader.UseDelimiters("|");
            reader.OnEachLine(tuple => changed = true);

            // act
            while(reader.Read()) { }

            // assert
            Assert.True(changed);
        }
    }
}
