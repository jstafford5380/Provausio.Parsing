using System.Collections.Generic;

namespace Provausio.Parsing.Csv.Mapping.Mutators
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <seealso cref="T:System.Collections.Generic.Dictionary`2" />
    /// <seealso cref="T:Provausio.Parsing.Csv.Mapping.Mutators.IMutator" />
    public class ValueMutator : Dictionary<string, string>, IMutator
    {
        /// <summary>
        /// Mutates the specified source value.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <returns></returns>
        public string Mutate(string sourceValue)
        {
            if (!this.ContainsKey(sourceValue)) sourceValue = "default";
            return this[sourceValue];
        }
    }
}