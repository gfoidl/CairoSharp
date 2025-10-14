// (c) gfoidl, all rights reserved

using static Cairo.Extensions.Loading.LoadingNative;

namespace Cairo.Extensions.Loading.SVG;

/// <summary>
/// Extensions for librsvg and <see cref="CairoContext"/>.
/// </summary>
public static unsafe class LibRSvgExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Loads the SVG file and renders it in cairo.
        /// </summary>
        /// <param name="fileName">SVG file</param>
        /// <param name="viewPort">viewport size at which the whole SVG would be fitted</param>
        /// <param name="flags">flags from <see cref="RsvgHandleFlags"/></param>
        /// <param name="dpi">
        /// the DPI at which the SVG will be rendered in cairo. Common values are 75, 90 and 300 DPI. See
        /// <a href="https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/class.Handle.html#resolution-of-the-rendered-image-dots-per-inch-or-dpi">Resolution of the rendered image (dots per inch, or DPI)</a>
        /// for further information. Current CSS assumes a default DPI of 96 (the default used here).
        /// </param>
        /// <remarks>
        /// This function sets the “base file” of the handle to be <paramref name="fileName"/> itself, so
        /// SVG elements like <c>&lt;image&gt;</c> which reference external resources will be resolved
        /// relative to the location of <paramref name="fileName"/>. See
        /// <a href="https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/class.Handle.html#the-base-file-and-resolving-references-to-external-files">The "base file" and resolving references to external files</a>
        /// for further information.
        /// <para>
        /// The <paramref name="viewPort"/> gives the position and size at which the whole SVG document will
        /// be rendered. The document is scaled proportionally to fit into this viewport.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <c>null</c></exception>
        /// <exception cref="LibRsvgException">an error occured</exception>
        public void LoadSvg(string fileName, RsvgRectangle viewPort, RsvgHandleFlags flags = RsvgHandleFlags.None, double dpi = 96d)
        {
            using SvgDocument svg = new(fileName, flags, dpi);

            // We pass the handle down to RenderCore, so that the JIT sees that the svg doesn't
            // leave the scope, and may be potentially become stack allocated and not on the heap.
            // This could happen for .NET 10+
            RenderCore(svg.Handle, cr, viewPort);
        }

        /// <summary>
        /// Loads the SVG data and renders it in cairo.
        /// </summary>
        /// <param name="svgData">SVG data</param>
        /// <param name="viewPort">viewport size at which the whole SVG would be fitted</param>
        /// <param name="flags">flags from <see cref="RsvgHandleFlags"/></param>
        /// <param name="dpi">
        /// the DPI at which the SVG will be rendered in cairo. Common values are 75, 90 and 300 DPI. See
        /// <a href="https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/class.Handle.html#resolution-of-the-rendered-image-dots-per-inch-or-dpi">Resolution of the rendered image (dots per inch, or DPI)</a>
        /// for further information. Current CSS assumes a default DPI of 96 (the default used here).
        /// </param>
        /// <remarks>
        /// The <paramref name="viewPort"/> gives the position and size at which the whole SVG document will
        /// be rendered. The document is scaled proportionally to fit into this viewport.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="svgData"/> is empty</exception>
        /// <exception cref="LibRsvgException">an error occured</exception>
        public void LoadSvg(ReadOnlySpan<byte> svgData, RsvgRectangle viewPort, RsvgHandleFlags flags = RsvgHandleFlags.None, double dpi = 96d)
        {
            using SvgDocument svg = new(svgData, flags, dpi);
            RenderCore(svg.Handle, cr, viewPort);
        }

        /// <summary>
        /// Loads the <paramref name="svgDocument"/> and renders it in cairo.
        /// </summary>
        /// <param name="svgDocument">SVG document</param>
        /// <param name="viewPort">viewport size at which the whole SVG would be fitted</param>
        /// <param name="dpi">
        /// the DPI at which the SVG will be rendered in cairo. Common values are 75, 90 and 300 DPI. See
        /// <a href="https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/class.Handle.html#resolution-of-the-rendered-image-dots-per-inch-or-dpi">Resolution of the rendered image (dots per inch, or DPI)</a>
        /// for further information. Current CSS assumes a default DPI of 96 (the default used here).
        /// </param>
        /// <remarks>
        /// The <paramref name="viewPort"/> gives the position and size at which the whole SVG document will
        /// be rendered. The document is scaled proportionally to fit into this viewport.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="svgDocument"/> is <c>null</c></exception>
        /// <exception cref="LibRsvgException">an error occured</exception>
        public void LoadSvg(SvgDocument svgDocument, RsvgRectangle viewPort, double dpi = 96d)
        {
            ArgumentNullException.ThrowIfNull(svgDocument);

            RenderCore(svgDocument.Handle, cr, viewPort);
        }
    }

    private static void RenderCore(RsvgHandle* handle, CairoContext cr, RsvgRectangle viewPort)
    {
        GError* error = null;

        if (!rsvg_handle_render_document(handle, cr.NativeContext, &viewPort, &error))
        {
            throw new LibRsvgException(error);
        }
    }
}
