// (c) gfoidl, all rights reserved

using System.Diagnostics;

namespace CreateColorMaps;

internal sealed class DefaultCsvColorMapGenerator(string inputFile, string name, string? comment = null)
    : ColorMapGenerator(name, comment)
{
    private readonly string _inputFile = inputFile;

    protected override string Type => "Default";
    //-------------------------------------------------------------------------
    protected override IEnumerable<(double Red, double Green, double Blue)> GetColors()
    {
        using StreamReader sr = File.OpenText(_inputFile);

        while (!sr.EndOfStream)
        {
            string line   = sr.ReadLine()!;
            string[] cols = line.Split(',');

            Debug.Assert(cols.Length == 3);

            double r = double.Parse(cols[0]);
            double g = double.Parse(cols[1]);
            double b = double.Parse(cols[2]);

            Debug.Assert(r is >= 0 and <= 1);
            Debug.Assert(g is >= 0 and <= 1);
            Debug.Assert(b is >= 0 and <= 1);

            yield return (r, g, b);
        }
    }
}
