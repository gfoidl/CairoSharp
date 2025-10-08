// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.TagsAndLinks;

public static class CairoTagConstants
{
    /// <summary>
    /// Create a destination for a hyperlink. Destination tag attributes are detailed at
    /// <a href="https://www.cairographics.org/manual/cairo-Tags-and-Links.html#dest">Destinations</a>.
    /// </summary>
    /// <remarks>
    /// Since 1.16
    /// </remarks>
    public const string CairoTagDest = "cairo.dest";

    /// <summary>
    /// Create hyperlink. Link tag attributes are detailed at
    /// <a href="https://www.cairographics.org/manual/cairo-Tags-and-Links.html#link">Links</a>.
    /// </summary>
    /// <remarks>
    /// Since 1.16
    /// </remarks>
    public const string CairoTagLink = "Link";

    /// <summary>
    /// Create a content tag.
    /// </summary>
    /// <remarks>
    /// Since 1.18
    /// </remarks>
    public const string CairoTagContent = "cairo.content";

    /// <summary>
    /// Create a content reference tag.
    /// </summary>
    /// <remarks>
    /// Since 1.18
    /// </remarks>
    public const string CairoTagContentRef = "cairo.content_ref";
}
