using System;
using System.Collections.Generic;
using System.Text;
using Provausio.Parsing.Csv.Mapping.Mutators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Provausio.Parsing.Csv.Tests.Mapping.Mutators
{
    public class MutatorFactoryTests
    {
        [Fact]
        public void RegisterT_RegistersMutator()
        {
            // arrange
            var factory = new MutatorFactory();
            var mutatorInfo = new { Prop1 = "foo" };
            var asJson = JsonConvert.SerializeObject(mutatorInfo);
            var jObj = JsonConvert.DeserializeObject<JObject>(asJson);

            // act
            factory.Register<TestMutator>("test");

            // assert
            var mutator = factory.GetMutator("test", jObj);
            Assert.Equal("xx-foo", mutator("xx"));
        }

        [Fact]
        public void RegisterT_DeserializesJObject()
        {
            // arrange
            var factory = new MutatorFactory();
            factory.Register<TestMutator>("test");
            var mutatorInfo = new { Prop1 = "foo" };
            var asJson = JsonConvert.SerializeObject(mutatorInfo);
            var jObj = JsonConvert.DeserializeObject<JObject>(asJson);

            // act
            var mutator = factory.GetMutator("test", jObj);

            // assert

            Assert.Equal("xx-foo", mutator("xx"));
        }

        [Fact]
        public void GetMutator_Flyweight_ReusesMutator()
        {
            // arrange
            var factory = new MutatorFactory();
            factory.Register<TestMutator>("test");
            var mutatorInfo = new { Prop1 = "foo" };
            var asJson = JsonConvert.SerializeObject(mutatorInfo);
            var jObj = JsonConvert.DeserializeObject<JObject>(asJson);

            // act
            var mutator = factory.GetMutator("test", jObj);
            var mutator2 = factory.GetMutator("test", jObj);

            // assert   
            Assert.Equal(mutator2, mutator);
        }

        [Fact]
        public void GetMutator_NotRegistered_Throws()
        {
            // arrange
            var factory = new MutatorFactory();

            // act

            // assert
            Assert.Throws<ArgumentOutOfRangeException>(() => factory.GetMutator("foo", new JObject()));
        }
    }

    public class TestMutator : IMutator
    {
        public string Prop1 { get; set; }

        public string Mutate(string sourceValue)
        {
            return $"{sourceValue}-{Prop1}";
        }
    }
}
