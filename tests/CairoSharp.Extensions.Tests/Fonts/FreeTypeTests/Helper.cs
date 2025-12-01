// (c) gfoidl, all rights reserved

using Cairo.Fonts.FreeType;

namespace CairoSharp.Extensions.Tests.Fonts.FreeTypeTests;

internal static class Helper
{
    public static FreeTypeFont LoadFreeTypeFontFromFile(string fontName) => FreeTypeFont.LoadFromFile($"Fonts/fonts/{fontName}");
}
