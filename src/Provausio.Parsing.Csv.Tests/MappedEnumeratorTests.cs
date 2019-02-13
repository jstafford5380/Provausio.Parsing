using System.Collections.Generic;
using Provausio.Parsing.Csv.Csv;
using Xunit;

namespace Provausio.Parsing.Csv.Tests
{
    public class MappedEnumeratorTests
    {
        private readonly string[] _testValues;
        private readonly Dictionary<int, int> _testMap;

        public MappedEnumeratorTests()
        {
            _testMap = new Dictionary<int, int> {{2, 0}, {1, 1}, {0, 2}};
            _testValues = new[] {"foo", "bar", "baz"};
        }

        [Fact]
        public void MoveNext_FirstIsLast()
        {
            // arrange
            var enumerator = new MappedEnumerator(_testMap, _testValues);

            // act
            enumerator.MoveNext();

            // assert
            Assert.Equal("baz", enumerator.Current);
        }

        [Fact]
        public void MoveNext_AllMapCorrectly()
        {
            // arrange
            var enumerator = new MappedEnumerator(_testMap, _testValues);

            // act & assert
            enumerator.MoveNext();
            Assert.Equal("baz", enumerator.Current);

            enumerator.MoveNext();
            Assert.Equal("bar", enumerator.Current);

            enumerator.MoveNext();
            Assert.Equal("foo", enumerator.Current);
        }

        [Fact]
        public void Reset_CurrentIsNull()
        {
            // arrange
            var enumerator = new MappedEnumerator(_testMap, _testValues);
            enumerator.MoveNext();
            enumerator.MoveNext();

            // act
            enumerator.Reset();

            // assert
            Assert.Null(enumerator.Current);
        }
    }
}