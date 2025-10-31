// (c) gfoidl, all rights reserved

using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

XElement xml          = XElement.Load("System.Drawing.Primitives.xml");
using StreamWriter sw = File.CreateText("KnownColors.cs");

WriteHeader(sw);

foreach ((Color color, string? comment) in GetColors(xml).OrderBy(c => c.Color.Name))
{
    WriteColor(sw, color, comment);
}

WriteFooter(sw);

Console.WriteLine("bye.");
//-----------------------------------------------------------------------------
static IEnumerable<(Color Color, string? Comment)> GetColors(XElement xml)
{
    foreach (var item in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public))
    {
        if (item.PropertyType != typeof(Color)) continue;

        Color color     = (Color)item.GetValue(null)!;
        string? comment = GetComment(xml, color);

        yield return (color, comment);
    }
}
//-----------------------------------------------------------------------------
static string? GetComment(XElement xml, Color color)
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
//-----------------------------------------------------------------------------
static void WriteHeader(StreamWriter sw)
{
    sw.WriteLine("using Cairo;");
    sw.WriteLine();
    sw.WriteLine("/* Created with CreateKnownColors. */");
    sw.WriteLine();
    sw.WriteLine("namespace Cairo.Extensions.Colors;");
    sw.WriteLine();
    sw.WriteLine("/// <summary>");
    sw.WriteLine("/// Predefined colors.");
    sw.WriteLine("/// </summary>");
    sw.WriteLine("public static class KnownColors");
    sw.WriteLine("{");
}
//-----------------------------------------------------------------------------
static void WriteColor(StreamWriter sw, Color color, string? comment = null)
{
    double r = color.R / 255d;
    double g = color.G / 255d;
    double b = color.B / 255d;
    double a = color.A / 255d;

    string rr = r.ToString(CultureInfo.InvariantCulture);
    string gg = g.ToString(CultureInfo.InvariantCulture);
    string bb = b.ToString(CultureInfo.InvariantCulture);
    string aa = a.ToString(CultureInfo.InvariantCulture);

    sw.WriteLine("    /// <summary>");
    sw.WriteLine("    /// {0}", comment ?? color.Name);
    sw.WriteLine("    /// </summary>");
    sw.WriteLine("    public static readonly Color {0} = new Color({1}, {2}, {3}, {4});", color.Name, rr, gg, bb, aa);
    sw.WriteLine();
}
//-----------------------------------------------------------------------------
static void WriteFooter(StreamWriter sw)
{
    sw.WriteLine("}");
}
