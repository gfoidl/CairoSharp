// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces.PDF;

/// <summary>
/// <see cref="PdfOutlineFlags"/> is used by the cairo_pdf_surface_add_outline() function
/// specify the attributes of an outline item. These flags may be bitwise-or'd to produce
/// any combination of flags.
/// </summary>
[Flags]
public enum PdfOutlineFlags
{
    /// <summary>
    /// The outline item defaults to open in the PDF viewer (Since 1.16)
    /// </summary>
    Open = 1,

    /// <summary>
    /// The outline item is displayed by the viewer in bold text (Since 1.16)
    /// </summary>
    Bold = 2,

    /// <summary>
    /// The outline item is displayed by the viewer in italic text (Since 1.16)
    /// </summary>
    Italic = 4
}
