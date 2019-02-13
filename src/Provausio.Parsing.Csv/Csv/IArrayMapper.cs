using System.Collections.Generic;

namespace Provausio.Parsing.Csv.Csv
{
    public interface IArrayMapper
    {
        IEnumerable<string> CurrentRecord { get; }
    }
}