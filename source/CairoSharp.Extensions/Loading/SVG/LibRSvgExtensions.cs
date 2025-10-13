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
        /// <param name="viewPort">
        /// viewport size at which the whole SVG would be fitted
        /// </param>
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
            ArgumentNullException.ThrowIfNull(fileName);

            GError* error      = null;
            GFile* file        = null;
            RsvgHandle* handle = null;

            try
            {
                file   = g_file_new_for_path(fileName);
                handle = rsvg_handle_new_from_gfile_sync(file, flags, cancellable: null, &error);

                RenderCore(handle, cr, viewPort, dpi, error);
            }
            finally
            {
                if (handle is not null)
                {
                    g_object_unref(handle);
                }

                if (file is not null)
                {
                    g_object_unref(file);
                }
            }
        }

        /// <summary>
        /// Loads the SVG data and renders it in cairo.
        /// </summary>
        /// <param name="svgData">SVG data</param>
        /// <param name="viewPort">
        /// viewport size at which the whole SVG would be fitted
        /// </param>
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
            if (svgData.IsEmpty)
            {
                throw new ArgumentException("no data given", nameof(svgData));
            }

            GError* error             = null;
            GInputStream* inputStream = null;
            RsvgHandle* handle        = null;

            try
            {
                fixed (byte* ptr = svgData)
                {
                    // According https://gnome.pages.gitlab.gnome.org/librsvg/Rsvg-2.0/ctor.Handle.new_from_data.html
                    // rsvg_handle_new_from_data shouldn't be used, as no flags, etc. can be set.
                    inputStream = g_memory_input_stream_new_from_data(ptr, (nint)svgData.Length, &DummyDestroyNotify);
                    handle      = rsvg_handle_new_from_stream_sync(inputStream, null, flags, cancellable: null, &error);

                    RenderCore(handle, cr, viewPort, dpi, error);
                }
            }
            finally
            {
                if (handle is not null)
                {
                    g_object_unref(handle);
                }

                if (inputStream is not null)
                {
                    g_object_unref(inputStream);
                }
            }

            // Let the GC do it's thing.
            static void DummyDestroyNotify(void* _) { }
        }
    }

    private static void RenderCore(RsvgHandle* handle, CairoContext cr, RsvgRectangle viewPort, double dpi, GError* error)
    {
        if (handle is null)
        {
            throw new LibRsvgException(error);
        }

        rsvg_handle_set_dpi(handle, dpi);

        if (!rsvg_handle_render_document(handle, cr.NativeContext, &viewPort, &error))
        {
            throw new LibRsvgException(error);
        }
    }
}
