// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces.PDF;

/// <summary>
/// <see cref="PdfMetadata"/> is used by the cairo_pdf_surface_set_metadata() function
/// specify the metadata to set.
/// </summary>
public enum PdfMetadata
{
    /// <summary>
    /// The document title (Since 1.16)
    /// </summary>
    Title,

    /// <summary>
    /// The document author (Since 1.16)
    /// </summary>
    Author,

    /// <summary>
    /// The document subject (Since 1.16)
    /// </summary>
    Subject,

    /// <summary>
    /// The document keywords (Since 1.16)
    /// </summary>
    Keywords,

    /// <summary>
    /// The document creator (Since 1.16)
    /// </summary>
    Creator,

    /// <summary>
    /// The document creation date (Since 1.16)
    /// </summary>
    CreationDate,

    /// <summary>
    /// The document modification date (Since 1.16)
    /// </summary>
    ModificationDate
}
