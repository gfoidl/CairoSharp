// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces;

public static class MimeTypes
{
    /// <summary>
    /// Group 3 or Group 4 CCITT facsimile encoding (International Telecommunication Union, Recommendations T.4 and T.6.)
    /// </summary>
    public const string CcittFax = "image/g3fax";

    /// <summary>
    /// Decode parameters for Group 3 or Group 4 CCITT facsimile encoding. See CCITT Fax Images.
    /// </summary>
    public const string CcittFaxParams = "application/x-cairo.ccitt.params";

    /// <summary>
    /// Encapsulated PostScript file. Encapsulated PostScript File Format Specification
    /// </summary>
    public const string Eps = "application/postscript";

    /// <summary>
    /// Embedding parameters Encapsulated PostScript data. See Embedding EPS files.
    /// </summary>
    public const string EpsParams = "application/x-cairo.eps.params";

    /// <summary>
    /// Joint Bi-level Image Experts Group image coding standard (ISO/IEC 11544).
    /// </summary>
    public const string Jbig2 = "application/x-cairo.jbig2";

    /// <summary>
    /// Joint Bi-level Image Experts Group image coding standard (ISO/IEC 11544) global segment.
    /// </summary>
    public const string Jbig2Global = "application/x-cairo.jbig2-global";

    /// <summary>
    /// An unique identifier shared by a JBIG2 global segment and all JBIG2 images that depend on the global segment.
    /// </summary>
    public const string Jbig2GlobalId = "application/x-cairo.jbig2-global-id";

    /// <summary>
    /// The Joint Photographic Experts Group (JPEG) 2000 image coding standard (ISO/IEC 15444-1).
    /// </summary>
    public const string Jp2 = "image/jp2";

    /// <summary>
    /// The Joint Photographic Experts Group (JPEG) image coding standard (ISO/IEC 10918-1).
    /// </summary>
    public const string Jpeg = "image/jpeg";

    /// <summary>
    /// The Portable Network Graphics image file format (ISO/IEC 15948).
    /// </summary>
    public const string Png = "image/png";

    /// <summary>
    /// URI for an image file (unofficial MIME type).
    /// </summary>
    public const string Uri = "text/x-uri";

    /// <summary>
    /// Unique identifier for a surface (cairo specific MIME type). All surfaces with the
    /// same unique identifier will only be embedded once.
    /// </summary>
    public const string UniqueId = "application/x-cairo.uuid";
}
