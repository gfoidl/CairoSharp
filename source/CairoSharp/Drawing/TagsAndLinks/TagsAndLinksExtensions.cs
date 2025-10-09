// (c) gfoidl, all rights reserved

using static Cairo.Drawing.TagsAndLinks.TagsAndLinksNative;

namespace Cairo;

/// <summary>
/// Tags and Links — Hyperlinks and document structure
/// </summary>
/// <remarks>
/// The tag functions provide the ability to specify hyperlinks and document logical
/// structure on supported backends. The following tags are supported:
/// <list type="bullet">
/// <item>Link - Create a hyperlink</item>
/// <item>Destinations - Create a hyperlink destination</item>
/// <item>Document Structure Tags - Create PDF Document Structure</item>
/// </list>
/// </remarks>
public static unsafe class TagsAndLinksExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Marks the beginning of the <paramref name="tagName"/> structure.
        /// Call <see cref="TagEnd(CairoContext, string)"/> with the same
        /// <paramref name="tagName"/> to mark the end of the structure.
        /// </summary>
        /// <param name="tagName">tag name</param>
        /// <param name="attributes">tag attributes</param>
        /// <remarks>
        /// See <a href="https://www.cairographics.org/manual/cairo-Tags-and-Links.html#cairo-tag-begin">cairo docs</a>
        /// for further information.
        /// </remarks>
        public void TagBegin(string tagName, string attributes)
        {
            ArgumentNullException.ThrowIfNull(tagName);
            ArgumentNullException.ThrowIfNull(attributes);

            cr.CheckDisposed();

            cairo_tag_begin(cr.Handle, tagName, attributes);
        }

        /// <summary>
        /// Marks the end of the <paramref name="tagName"/> structure.
        /// </summary>
        /// <param name="tagName">tag name</param>
        /// <remarks>
        /// Invalid nesting of tags will cause cr to shutdown and throw a <see cref="CairoException"/>.
        /// <para>
        /// See <see cref="TagBegin(CairoContext, string, string)"/>.
        /// </para>
        /// </remarks>
        public void TagEnd(string tagName)
        {
            ArgumentNullException.ThrowIfNull(tagName);
            cr.CheckDisposed();

            cairo_tag_end(cr.Handle, tagName);

            cr.Status.ThrowIfStatus(Status.TagError);
        }
    }
}
