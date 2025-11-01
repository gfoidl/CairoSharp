// (c) gfoidl, all rights reserved

using System.Diagnostics;

namespace CreateColorMaps;

internal sealed class OptimizedCsvColorMapGenerator(string inputFile, string name, string? comment = null)
    : ColorMapGenerator(name, comment)
{
    private readonly string _inputFile = inputFile;

    protected override string Type => "Optimized";
    //-------------------------------------------------------------------------
    protected override IEnumerable<(double Red, double Green, double Blue)> GetColors()
    {
        using StreamReader sr = File.OpenText(_inputFile);

        // header line
        sr.ReadLine();

        while (!sr.EndOfStream)
        {
            string line   = sr.ReadLine()!;
            string[] cols = line.Split(',');

            Debug.Assert(cols.Length == 4);

            double r = double.Parse(cols[1]);
            double g = double.Parse(cols[2]);
            double b = double.Parse(cols[3]);

            Debug.Assert(r is >= 0 and <= 1);
            Debug.Assert(g is >= 0 and <= 1);
            Debug.Assert(b is >= 0 and <= 1);

            yield return (r, g, b);
        }
    }
}
