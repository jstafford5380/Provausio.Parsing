using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Provausio.Parsing.Csv.Mapping.Mutators
{
    public interface IMutatorFactory
    {
        /// <summary>
        /// Gets the mutator function for the specified 'type' label.
        /// </summary>
        /// <param name="label">The type label.</param>
        /// <param name="mutatorInfo">The mutator information.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Could not find an initializer for '{label}</exception>
        Func<string, string> GetMutator(string label, JObject mutatorInfo);

        /// <summary>
        /// Registers the specified mutator with a type label.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label">The label.</param>
        void Register<T>(string label)
            where T : IMutator;
    }

    /// <summary>
    /// Mutator registry.
    /// </summary>
    public class MutatorFactory : IMutatorFactory
    {
        private readonly Dictionary<string, Action<JObject>> _flyweightInitializer 
            = new Dictionary<string, Action<JObject>>();
        
        private readonly Dictionary<string, Func<string, string>> _mutators 
            = new Dictionary<string, Func<string, string>>();

        /// <summary>
        /// Initializes the <see cref="MutatorFactory"/> class.
        /// </summary>
        public MutatorFactory()
        {
            Register<DateTimeMutator>("date");
            Register<ValueMutator>("enum");
            Register<DelimiterMutator>("delimited");
        }

        /// <summary>
        /// Gets the mutator function for the specified 'type' label.
        /// </summary>
        /// <param name="label">The type label.</param>
        /// <param name="mutatorInfo">The mutator information.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Could not find an initializer for '{label}</exception>
        public Func<string, string> GetMutator(string label, JObject mutatorInfo)
        {
            // flyweight
            if (_mutators.TryGetValue(label, out var mutator))
                return mutator;

            if(!_flyweightInitializer.ContainsKey(label))
                throw new ArgumentOutOfRangeException($"Could not find an initializer for '{label}' mutator. Did you forget to register it?");
            
            _flyweightInitializer[label](mutatorInfo);
            return _mutators[label];
        }

        /// <summary>
        /// Registers the specified mutator with a type label.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label">The label.</param>
        public void Register<T>(string label)
            where T : IMutator
        {
            _flyweightInitializer.Add(
                label, 
                j => _mutators.Add(label, j.ToObject<T>().Mutate));
        }
    }
}