using Provausio.Parsing.Csv.Mapping.Mutators;
using Xunit;

namespace Provausio.Parsing.Csv.Tests.Mapping.Mutators
{
    public class DateTimeMutatorTests
    {
        [Theory]
        [InlineData("2018-11-30 12:21:22", "2018-11-30T12:21:22")]
        [InlineData("2018-09-30 13:21:22", "2018-09-30T13:21:22")]
        public void Mutate_InputDate_IsTransformed(string input, string output)
        {
            // arrange
            var mutator = new DateTimeMutator {SourceFormat = "yyyy-MM-dd HH:mm:ss", DestinationFormat = "yyyy-MM-ddTHH:mm:ss"};

            // act
            var result = mutator.Mutate(input);

            // assert
            Assert.Equal(output, result);
        }
    }

    public class DelimiterMutatorTests
    {
        [Theory]
        [InlineData(',', '|')]
        [InlineData('\t', '|')]
        [InlineData('|', '\t')]
        public void Mutate_Delimiters_Changed(char sourceDelimiter, char destinationDelimiter)
        {
            // arrange
            var input = $"foo{sourceDelimiter}bar{sourceDelimiter}baz";
            var expectedOutput = $"foo{destinationDelimiter}bar{destinationDelimiter}baz"; ;
            var mutator = new DelimiterMutator
            {
                SourceDelimiter = sourceDelimiter,
                DestinationDelimiter = destinationDelimiter
            };

            // act
            var result = mutator.Mutate(input);

            // assert
            Assert.Equal(expectedOutput, result);
        }
    }

    public class ValueMutatorTests
    {
        [Fact]
        public void Mutate_ValuesChanged()
        {
            // arrange
            var mutator = new ValueMutator {{"foo", "1"}, {"bar", "2"}};

            // act

            // assert
            Assert.Equal("1", mutator.Mutate("foo"));
            Assert.Equal("2", mutator.Mutate("bar"));
        }

        [Fact]
        public void Mutate_NoMap_ReturnsDefault()
        {
            // arrange
            var mutator = new ValueMutator { { "foo", "1" }, { "bar", "2" }, { "default", "xxx"} };

            // act
            var result = mutator.Mutate("yy");

            // assert
            Assert.Equal("xxx", result);
        }
    }
}
