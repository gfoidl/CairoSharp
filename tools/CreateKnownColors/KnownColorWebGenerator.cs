// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CreateKnownColors;

internal static partial class KnownColorWebGenerator
{
    private readonly record struct ColorEntry(string Name, byte Red, byte Green, byte Blue);
    //-------------------------------------------------------------------------
    public static void Run()
    {
        using StreamWriter sw = File.CreateText("../../../../source/CairoSharp.Extensions/Colors/KnownColorsWeb.cs");

        WriteHeader(sw);

        foreach(ColorEntry colorEntry in GetColors("data/web-colors.txt"))
        {
            WriteColor(sw, colorEntry);
        }

        WriteFooter(sw);
    }
    //-------------------------------------------------------------------------
    private static IEnumerable<ColorEntry> GetColors(string file)
    {
        foreach (string line in File.ReadLines(file))
        {
            Match match = ColorRegex().Match(line);

            if (!match.Success)
            {
                throw new Exception("Line does not match regex");
            }

            string name = match.Groups["name"].Value;

            byte red    = byte.Parse(match.Groups["red"]  .Value);
            byte green  = byte.Parse(match.Groups["green"].Value);
            byte blue   = byte.Parse(match.Groups["blue"] .Value);

            yield return new ColorEntry(name, red, green, blue);
        }
    }

    [GeneratedRegex(@"^(?<name>\w+)\s+rgb\((?<red>\s?\d+),\s+(?<green>\s?\d+),\s+(?<blue>\s?\d+)\)$")]
    private static partial Regex ColorRegex();
    //-------------------------------------------------------------------------
    private static void WriteHeader(StreamWriter sw)
    {
        sw.WriteLine($$"""
            // (c) gfoidl, all rights reserved

            /* Created by tool CreateKnownColors at {{DateTimeOffset.UtcNow:O}} */

            using System.CodeDom.Compiler;

            namespace Cairo.Extensions.Colors;

            /// <summary>
            /// Predefined web colors based on W3C.
            /// </summary>
            /// <remarks>
            /// Colors are based on the table in <a href="https://www.w3.org/TR/css-color-3/#svg-color">Extended color keywords</a>.
            /// </remarks>
            [GeneratedCode("CreateKnownColors", "{{GetToolVersion()}}")]
            public static class KnownColorsWeb
            {
            """);

        static string GetToolVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? version = assembly.GetName().Version;

            Debug.Assert(version is not null);
            return version.ToString();
        }
    }
    //-------------------------------------------------------------------------
    private static void WriteColor(StreamWriter sw, ColorEntry colorEntry)
    {
        double r = colorEntry.Red   / 255d;
        double g = colorEntry.Green / 255d;
        double b = colorEntry.Blue  / 255d;

        string rr = r.ToString(CultureInfo.InvariantCulture);
        string gg = g.ToString(CultureInfo.InvariantCulture);
        string bb = b.ToString(CultureInfo.InvariantCulture);

        sw.WriteLine($"""
                /// <summary>
                /// {colorEntry.Name}
                /// </summary>
                public static readonly Color {colorEntry.Name} = new({rr}, {gg}, {bb}, 1d);
            """);
    }
    //-------------------------------------------------------------------------
    private static void WriteFooter(StreamWriter sw)
    {
        sw.WriteLine("}");
    }
}
