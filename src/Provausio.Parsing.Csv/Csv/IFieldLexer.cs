namespace Provausio.Parsing.Csv.Csv
{
    public interface IFieldLexer
    {
        /// <summary>
        /// Get whether or not there are more fields on the current line.
        /// </summary>
        bool HasMoreFields { get; }

        /// <summary>
        /// Sets the characters that will act as markers between each field.
        /// </summary>
        /// <param name="delimiters"></param>
        void SetDelimiters(params char[] delimiters);

        /// <summary>
        /// Parses the next field. Returns true/false indicating whether or not there is more 
        /// </summary>
        /// <returns></returns>
        string GetNextField();
    }
}
