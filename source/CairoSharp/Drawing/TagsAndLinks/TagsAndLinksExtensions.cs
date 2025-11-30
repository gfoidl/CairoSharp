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
        /// Marks the beginning of the <paramref name="tagName"/> structure. Call <see cref="TagEnd(CairoContext, string)"/>
        /// with the same<paramref name="tagName"/> to mark the end of the structure.
        /// <para>
        /// For common tag names in PDF see <see cref="Drawing.TagsAndLinks.CairoTagConstants"/>.
        /// </para>
        /// </summary>
        /// <param name="tagName">tag name</param>
        /// <param name="attributes">tag attributes</param>
        /// <returns>
        /// A <see cref="TagScope"/>, which when <see cref="TagScope.Dispose"/>d calls <see cref="TagEnd(CairoContext, string)"/>.
        /// <para>
        /// So instead of writing code like
        /// <code>
        /// cr.TagBegin("foo");
        /// // ...
        /// cr.TagEnd("foo");
        /// </code>
        /// one can write
        /// <code>
        /// using (cr.TagBegin("foo"))
        /// {
        ///     // ...
        /// }
        /// </code>
        /// </para>
        /// </returns>
        /// <remarks>
        /// See <a href="https://www.cairographics.org/manual/cairo-Tags-and-Links.html">cairo docs</a>
        /// for further information and examples. More examples are in the
        /// <a href="https://gitlab.com/saiwp/cairo/-/blob/master/test/pdf-structure.c?ref_type=heads#L43">cairo C tests</a>.
        /// <para>
        /// Invalid nesting of tags will cause cr to shutdown and throw a <see cref="CairoException"/> with
        /// <see cref="CairoException.Status"/> of <see cref="Status.TagError"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="CairoException">invalid nesting of tags</exception>
        public TagScope TagBegin(string tagName, string attributes)
        {
            ArgumentNullException.ThrowIfNull(tagName);
            ArgumentNullException.ThrowIfNull(attributes);

            cr.CheckDisposed();

            cairo_tag_begin(cr.Handle, tagName, attributes);

            return new TagScope(cr, tagName);
        }

        /// <summary>
        /// Marks the end of the <paramref name="tagName"/> structure.
        /// </summary>
        /// <param name="tagName">tag name</param>
        /// <remarks>
        /// Invalid nesting of tags will cause cr to shutdown and throw a <see cref="CairoException"/> with
        /// <see cref="CairoException.Status"/> of <see cref="Status.TagError"/>.
        /// <para>
        /// See <see cref="TagBegin(CairoContext, string, string)"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="CairoException">invalid nesting of tags</exception>
        public void TagEnd(string tagName)
        {
            ArgumentNullException.ThrowIfNull(tagName);
            cr.CheckDisposed();

            cairo_tag_end(cr.Handle, tagName);

            cr.Status.ThrowIfStatus(Status.TagError);
        }
    }
}

/// <summary>
/// A helper type for <see cref="TagsAndLinksExtensions.TagEnd(CairoContext, string)"/> / <see cref="TagsAndLinksExtensions.TagEnd(CairoContext, string)"/>.
/// </summary>
public ref struct TagScope
{
    private CairoContext?   _context;
    private readonly string _tagName;

    internal TagScope(CairoContext context, string tagName) => (_context, _tagName) = (context, tagName);

    /// <summary>
    /// Calls <see cref="TagsAndLinksExtensions.TagEnd(CairoContext, string)"/>
    /// </summary>
    public void Dispose()
    {
        _context?.TagEnd(_tagName);
        _context = null;
    }
}
