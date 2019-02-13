namespace Provausio.Parsing.Csv.Mapping.Mutators
{
    public interface IMutator
    {
        /// <summary>
        /// Mutates the specified source value.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <returns></returns>
        string Mutate(string sourceValue);
    }
}