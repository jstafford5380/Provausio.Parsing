namespace Provausio.Parsing.Csv.Mapping.Mutators
{
    /// <summary>
    /// Used for changing delimiters in a quoted field, for example.
    /// </summary>
    /// <seealso>
    ///     <cref>Provausio.Parsing.Csv.Mapping.Mutators.IMutator</cref>
    /// </seealso>
    public class DelimiterMutator : IMutator
    {
        /// <summary>
        /// Gets or sets the source delimiter.
        /// </summary>
        /// <value>
        /// The source delimiter.
        /// </value>
        public char SourceDelimiter { get; set; }

        /// <summary>
        /// Gets or sets the destination delimiter.
        /// </summary>
        /// <value>
        /// The destination delimiter.
        /// </value>
        public char DestinationDelimiter { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Mutates the specified source value.
        /// </summary>
        /// <param name="sourceValue">The source value.</param>
        /// <returns></returns>
        public string Mutate(string sourceValue)
        {
            return sourceValue.Replace(SourceDelimiter, DestinationDelimiter);
        }
    }
}