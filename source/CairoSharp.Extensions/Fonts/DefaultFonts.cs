// (c) gfoidl, all rights reserved

using System.IO;
using System.Reflection;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;

namespace Cairo.Extensions.Fonts;

/// <summary>
/// Provides default <see cref="FontFace"/>s.
/// </summary>
public static class DefaultFonts
{
    /// <summary>
    /// Helvetica
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerif => field ??= new ToyFontFace("Helvetica");

    /// <summary>
    /// Helvetica (bold)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerifBold => field ??= new ToyFontFace("Helvetica", weight: Drawing.Text.FontWeight.Bold);

    /// <summary>
    /// Helvetica (italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerifItalic => field ??= new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic);

    /// <summary>
    /// Helvetica (bold italic)
    /// </summary>
    /// <returns>
    /// A <see cref="ToyFontFace"/> created for Helvetica.
    /// </returns>
    public static FontFace SansSerifBoldItalic => field ??= new ToyFontFace("Helvetica", slant: Drawing.Text.FontSlant.Italic, weight: Drawing.Text.FontWeight.Bold);

    /// <summary>
    /// Source Code Pro
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpace
    {
        get
        {
            if (field is null)
            {
                using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-Regular.ttf")!;
                field                   = FreeTypeFont.LoadFromStream(fontStream);
            }

            return field;
        }
    }

    /// <summary>
    /// Source Code Pro (bold)
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpaceBold
    {
        get
        {
            if (field is null)
            {
                using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-Bold.ttf")!;
                field                   = FreeTypeFont.LoadFromStream(fontStream);
            }

            return field;
        }
    }

    /// <summary>
    /// Source Code Pro (italic)
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpaceItalic
    {
        get
        {
            if (field is null)
            {
                using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-Italic.ttf")!;
                field = FreeTypeFont.LoadFromStream(fontStream);
            }

            return field;
        }
    }

    /// <summary>
    /// Source Code Pro (bold italic)
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Source Code Pro.
    /// </returns>
    /// <remarks>
    /// Source Code Pro is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpaceBoldItalic
    {
        get
        {
            if (field is null)
            {
                using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.SourceCodePro-BoldItalic.ttf")!;
                field = FreeTypeFont.LoadFromStream(fontStream);
            }

            return field;
        }
    }
}
