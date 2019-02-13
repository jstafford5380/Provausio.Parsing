using System.Collections.Generic;
using Provausio.Parsing.Csv.Csv;
using Xunit;

namespace Provausio.Parsing.Csv.Tests
{
    public class MappedListTests
    {
        private readonly string[] _testValues;
        private readonly Dictionary<int, int> _testMap;

        public MappedListTests()
        {
            _testMap = new Dictionary<int, int> { { 2, 0 }, { 1, 1 }, { 0, 2 } };
            _testValues = new[] { "foo", "bar", "baz" };
        }

        [Fact]
        public void Index_FirstIsLast()
        {
            // arrange
            var list = new MappedList(_testMap);
            list.SetValues(_testValues);

            // act
            var value = list[0];

            // assert
            Assert.Equal("baz", value);
        }

        [Fact]
        public void Foreach_ListIsBackwards()
        {
            // arrange
            var expected = new[] {"baz", "bar", "foo"};
            var list = new MappedList(_testMap);
            list.SetValues(_testValues);

            // act
            var output = new List<string>();
            foreach (var value in list)
            {
                output.Add(value);
            }

            // assert
            var asArray = output.ToArray();
            Assert.Equal(expected, asArray);
        }

        [Fact]
        public void Count_ReturnsCount()
        {
            // arrange
            var list = new MappedList(_testMap);
            list.SetValues(_testValues);

            // act
            var count = list.Count;

            // assert
            Assert.Equal(_testValues.Length, count);
        }

        [Fact]
        public void Contains_DoesContain_IsTrue()
        {
            // arrange
            var list = new MappedList(_testMap);
            list.SetValues(_testValues);

            // act
            var result = list.Contains("foo");

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_DoesNotContain_IsFalse()
        {
            // arrange
            var list = new MappedList(_testMap);
            list.SetValues(_testValues);

            // act
            var result = list.Contains("bat");

            // assert
            Assert.False(result);
        }
    }
}
