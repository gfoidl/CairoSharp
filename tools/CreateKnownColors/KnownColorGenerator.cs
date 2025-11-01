// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace CreateKnownColors;

internal static class KnownColorGenerator
{
    public static void Run()
    {
        XElement xml          = XElement.Load("data/System.Drawing.Primitives.xml");
        using StreamWriter sw = File.CreateText("../../../../source/CairoSharp.Extensions/Colors/KnownColors.cs");

        WriteHeader(sw);

        foreach ((Color color, string? comment) in GetColors(xml).OrderBy(c => c.Color.Name))
        {
            WriteColor(sw, color, comment);
        }

        WriteFooter(sw);
    }
    //-------------------------------------------------------------------------
    private static IEnumerable<(Color Color, string? Comment)> GetColors(XElement xml)
    {
        foreach (var item in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public))
        {
            if (item.PropertyType != typeof(Color)) continue;

            Color color     = (Color)item.GetValue(null)!;
            string? comment = GetComment(xml, color);

            yield return (color, comment);
        }
    }
    //-------------------------------------------------------------------------
    private static string? GetComment(XElement xml, Color color)
    {
        XElement? member = xml
            .Descendants("member")
            .FirstOrDefault(x => x.Attribute("name")!.Value == $"P:System.Drawing.Color.{color.Name}");

        if (member == null)
        {
            return null;
        }

        string summary = member.Element("summary")!.Value;
        int index      = summary.IndexOf("ARGB");

        if (index < 0)
        {
            return color.Name;
        }

        string interestingPart = summary[index..];

        return $"{color.Name} -- {interestingPart}";
    }
    //-------------------------------------------------------------------------
    private static void WriteHeader(StreamWriter sw)
    {
        sw.WriteLine($$"""
            // (c) gfoidl, all rights reserved

            /* Created by tool CreateKnownColors at {{DateTimeOffset.UtcNow:O}} */

            using System.CodeDom.Compiler;

            namespace Cairo.Extensions.Colors;

            /// <summary>
            /// Predefined colors.
            /// </summary>
            [GeneratedCode("CreateKnownColors", "{{GetToolVersion()}}")]
            public static class KnownColors
            {
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
    private static void WriteColor(StreamWriter sw, Color color, string? comment = null)
    {
        double r = color.R / 255d;
        double g = color.G / 255d;
        double b = color.B / 255d;
        double a = color.A / 255d;

        string rr = r.ToString(CultureInfo.InvariantCulture);
        string gg = g.ToString(CultureInfo.InvariantCulture);
        string bb = b.ToString(CultureInfo.InvariantCulture);
        string aa = a.ToString(CultureInfo.InvariantCulture);

        sw.WriteLine($"""
                /// <summary>
                /// {comment ?? color.Name}
                /// </summary>
                public static readonly Color {color.Name} = new({rr}, {gg}, {bb}, {aa});
            """);
    }
    //-------------------------------------------------------------------------
    private static void WriteFooter(StreamWriter sw)
    {
        sw.WriteLine("}");
    }
}
