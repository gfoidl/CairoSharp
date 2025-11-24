// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading.SVG;

/// <summary>
/// Configuration flags for an RsvgHandle. Note that not all of RsvgHandle‘s constructors let you
/// specify flags.
/// </summary>
public enum RsvgHandleFlags
{
    /// <summary>
    /// No flags are set
    /// </summary>
    None = 0,

    /// <summary>
    /// Disable safety limits in the XML parser. Libxml2 has
    /// <a href="https://gitlab.gnome.org/GNOME/libxml2/blob/master/include/libxml/parserInternals.h">several limits</a>
    /// designed to keep malicious XML content from consuming too much memory while parsing. For security
    /// reasons, this should only be used for trusted input!
    /// </summary>
    Unlimited = 1,

    /// <summary>
    /// Use this if the <see cref="Surfaces.Surface"/> to which you are rendering is a PDF, PostScript, SVG,
    /// or Win32 Printing surface. This will make librsvg and cairo use the original, compressed data for
    /// images in the final output, instead of passing uncompressed images. For example, this will make
    /// the a resulting PDF file smaller and faster. Please see <see cref="Surfaces.Surface.SetMimeData(string, ReadOnlySpan{byte})"/>
    /// for details.
    /// </summary>
    KeepImageData = 2
}
