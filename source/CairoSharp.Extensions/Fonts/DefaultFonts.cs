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
    /// Inconsolata
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Inconsolata.
    /// </returns>
    /// <remarks>
    /// Inconsolata is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpace
    {
        get
        {
            if (field is null)
            {
                using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.Inconsolata-Regular.ttf")!;
                field                   = FreeTypeFont.LoadFromStream(fontStream);
            }

            return field;
        }
    }

    /// <summary>
    /// Inconsolata (bold)
    /// </summary>
    /// <returns>
    /// A <see cref="FreeTypeFont"/> created with Inconsolata.
    /// </returns>
    /// <remarks>
    /// Inconsolata is licensed under
    /// <a href="https://openfontlicense.org/open-font-license-official-text/">SIL Open Font License</a>.
    /// </remarks>
    public static FontFace MonoSpaceBold
    {
        get
        {
            if (field is null)
            {
                using Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fonts.Inconsolata-Bold.ttf")!;
                field                   = FreeTypeFont.LoadFromStream(fontStream);
            }

            return field;
        }
    }
}
