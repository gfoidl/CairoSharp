// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Reflection;

namespace CreateColorMaps;

internal abstract class ColorMapGenerator(string name, string? comment = null)
{
    protected readonly string _name    = name;
    private readonly string?  _comment = comment;

    protected abstract string Type { get; }
    //-------------------------------------------------------------------------
    public string Name => _name;
    //-------------------------------------------------------------------------
    public string Run()
    {
        List<(double red, double green, double blue)> colors = [.. this.GetColors()];

        string outputFileName = $"../../../../source/CairoSharp.Extensions/Colors/ColorMaps/{this.Type}/{_name}ColorMap.cs";
        Directory.CreateDirectory(Path.GetDirectoryName(outputFileName)!);

        using StreamWriter sw = File.CreateText(outputFileName);
        this.WriteHeader(sw, colors.Count);

        WriteData(sw, colors);
        this.WriteFooter(sw);

        return outputFileName;
    }
    //-------------------------------------------------------------------------
    protected abstract IEnumerable<(double Red, double Green, double Blue)> GetColors();
    //-------------------------------------------------------------------------
    private void WriteHeader(StreamWriter sw, int entries)
    {
        string summary = _comment ?? $"Color map {_name}";

        sw.WriteLine($$"""
            // (c) gfoidl, all rights reserved

            /* Created by tool CreateColorMaps at {{DateTimeOffset.UtcNow:O}} */

            using System.CodeDom.Compiler;

            namespace Cairo.Extensions.Colors.ColorMaps.{{this.Type}};

            /// <summary>
            /// {{summary}}
            /// </summary>
            [GeneratedCode("CreateKnownColors", "{{GetToolVersion()}}")]
            public sealed class {{_name}}ColorMap : ColorMap
            {
                protected override int Entries               => {{entries}};
                protected override double ScaleFactor        => {{entries - 1}};    // Entries - 1 (perf optimization)
                protected override ReadOnlySpan<double> Data =>
                [
            """);

        static string GetToolVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? version  = assembly.GetName().Version;

            Debug.Assert(version is not null);
            return version.ToString();
        }
    }
    //-------------------------------------------------------------------------
    private static void WriteData(StreamWriter sw, List<(double red, double green, double blue)> colors)
    {
        const string Spaces8 = "        ";

        for (int i = 0; i < colors.Count; ++i)
        {
            (double red, double green, double blue) = colors[i];

            Debug.Assert(0 <= red   && red   <= 1);
            Debug.Assert(0 <= green && green <= 1);
            Debug.Assert(0 <= blue  && blue  <= 1);

            sw.Write($"{Spaces8}{red}, {green}, {blue}");

            if (i < colors.Count - 1)
            {
                sw.WriteLine(',');
            }
            else
            {
                sw.WriteLine();
            }
        }
    }
    //-------------------------------------------------------------------------
    private void WriteFooter(StreamWriter sw)
    {
        if (_comment is null)
        {
            sw.WriteLine("""
                    ];
                }
                """);
        }
        else
        {
            sw.WriteLine($$"""
                    ];

                    public override string Description => "{{_comment}}";
                }
                """);
        }
    }
}
