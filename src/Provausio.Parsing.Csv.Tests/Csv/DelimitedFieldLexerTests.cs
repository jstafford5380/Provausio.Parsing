using System;
using System.IO;
using System.Text;
using System.Threading;
using Provausio.Parsing.Csv.Csv;
using Xunit;

namespace Provausio.Parsing.Csv.Tests.Csv
{
    public class DelimitedFieldLexerTests
    {
        [Fact]
        public void SetDelimiters_NullDelimiterSet_Throws()
        {
            // arrange
            var reader = new StreamReader(new MemoryStream());
            var lexer = new DelimitedFieldLexer(reader);

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => lexer.SetDelimiters());
        }

        [Fact]
        public void GetNextField_QuotedFields_ReadsAllFieldsCorrectly()
        {
            // arrange
            var csv = "Foo,\"bar, baz\",bat";
            var reader = new StringReader(csv);
            var lexer = new DelimitedFieldLexer(reader);

            // act
            var field1 = lexer.GetNextField();
            var field2 = lexer.GetNextField();
            var field3 = lexer.GetNextField();

            // assert
            Assert.Equal("Foo", field1);
            Assert.Equal("bar, baz", field2);
            Assert.Equal("bat", field3);
        }
    }
}
