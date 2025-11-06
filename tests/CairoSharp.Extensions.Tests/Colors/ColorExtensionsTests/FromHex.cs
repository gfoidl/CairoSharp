// (c) gfoidl, all rights reserved

using Cairo;
using Cairo.Extensions.Colors;

namespace CairoSharp.Extensions.Tests.Colors.ColorExtensionsTests;

[TestFixture]
public class FromHex
{
    [Test, TestCaseSource(nameof(Hex_parsed_correctly_TestCases))]
    public Color Hex_parsed_correctly(string hex)
    {
        return Color.FromHex(hex);
    }

    private static IEnumerable<TestCaseData> Hex_parsed_correctly_TestCases()
    {
        // Based on https://de.wikipedia.org/wiki/Hexadezimale_Farbdefinition#Beispiele_f%C3%BCr_Farbcodierungen

        yield return new TestCaseData("#000000").Returns(Color.FromRgbBytes(  0,   0,   0));
        yield return new TestCaseData("#888888").Returns(Color.FromRgbBytes(136, 136, 136));
        yield return new TestCaseData("#FFFFFF").Returns(Color.FromRgbBytes(255, 255, 255));
        yield return new TestCaseData("#FF0000").Returns(Color.FromRgbBytes(255,   0,   0));
        yield return new TestCaseData("#FF0000").Returns(Color.FromRgbBytes(255,   0,   0));
        yield return new TestCaseData("#778877").Returns(Color.FromRgbBytes(119, 136, 119));

        // Alpha
        yield return new TestCaseData("#778877FF").Returns(Color.FromRgbaBytes(119, 136, 119, 255));
        yield return new TestCaseData("#77887700").Returns(Color.FromRgbaBytes(119, 136, 119,   0));
        yield return new TestCaseData("#ff64d980").Returns(Color.FromRgbaBytes(255, 100, 217, 128));
    }
}
