// (c) gfoidl, all rights reserved

using static Cairo.Extensions.Loading.LoadingNative;

namespace Cairo.Extensions.Loading.PDF;

/// <summary>
/// Extensions for poppler and <see cref="CairoContext"/>.
/// </summary>
public static unsafe class PopplerExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Loads a PDF file and renders it in cairo.
        /// </summary>
        /// <param name="fileName">PDF file</param>
        /// <param name="pageIndex">a page index</param>
        /// <param name="printing"><c>true</c> for printing mode, <c>false</c> otherwise</param>
        /// <param name="flags">flags which allow to select which annotations to render</param>
        /// <remarks>
        /// Some things get rendered differently between screens and printers:
        /// <list type="bullet">
        /// <item>
        /// PDF annotations get rendered according to their <paramref name="flags"/> value. For example,
        /// <see cref="PopplerAnnotFlag.Print"/> refers to whether an annotation is printed or not, whereas
        /// <see cref="PopplerAnnotFlag.NoView"/> refers to whether an annotation is invisible when
        /// displaying to the screen.
        /// </item>
        /// <item>
        /// PDF supports "hairlines" of width 0.0, which often get rendered as having a width of 1 device
        /// pixel. When displaying on a screen, cairo may render such lines wide so that they are hard to
        /// see, and Poppler makes use of PDF's Stroke Adjust graphics parameter to make the lines easier to
        /// see. However, when printing, Poppler is able to directly use a printer's pixel size instead.
        /// </item>
        /// <item>
        /// Some advanced features in PDF may require an image to be rasterized before sending off to a
        /// printer. This may produce raster images which exceed cairo's limits. The "printing" functions
        /// will detect this condition and try to down-scale the intermediate surfaces as appropriate.
        /// </item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <c>null</c></exception>
        /// <exception cref="PopplerException">an error occured</exception>
        public void LoadPdf(string fileName, int pageIndex, bool printing = false, PopplerAnnotFlag flags = PopplerAnnotFlag.Unknown)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            GError* error             = null;
            GFile* file               = null;
            PopplerDocument* document = null;
            PopplerPage* page         = null;

            try
            {
                file     = g_file_new_for_path(fileName);
                document = poppler_document_new_from_gfile(file, password: null, cancellable: null, &error);
                page     = RenderCore(document, pageIndex, cr, printing, flags);
            }
            finally
            {
                if (page is not null)
                {
                    g_object_unref(page);
                }

                if (document is not null)
                {
                    g_object_unref(document);
                }

                if (file is not null)
                {
                    g_object_unref(file);
                }
            }
        }

        /// <summary>
        /// Loads the PDF data and renders it in cairo.
        /// </summary>
        /// <param name="pdfData">PDF data</param>
        /// <param name="pageIndex">a page index</param>
        /// <param name="printing"><c>true</c> for printing mode, <c>false</c> otherwise</param>
        /// <param name="flags">flags which allow to select which annotations to render</param>
        /// <remarks>
        /// See <see cref="LoadPdf(CairoContext, string, int, bool, PopplerAnnotFlag)"/> for the meaning of the
        /// <paramref name="printing"/> argument.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="pdfData"/> is empty</exception>
        /// <exception cref="LibRsvgException">an error occured</exception>
        public void LoadPdf(ReadOnlySpan<byte> pdfData, int pageIndex, bool printing = false, PopplerAnnotFlag flags = PopplerAnnotFlag.Unknown)
        {
            if (pdfData.IsEmpty)
            {
                throw new ArgumentException("no data given", nameof(pdfData));
            }

            GError* error             = null;
            GInputStream* inputStream = null;
            PopplerDocument* document = null;
            PopplerPage* page         = null;

            try
            {
                fixed (byte* ptr = pdfData)
                {
                    inputStream = g_memory_input_stream_new_from_data(ptr, (nint)pdfData.Length, &DummyDestroyNotify);
                    document    = poppler_document_new_from_stream(inputStream, 0, password: null, cancellable: null, &error);
                    page        = RenderCore(document, pageIndex, cr, printing, flags);
                }
            }
            finally
            {
                if (page is not null)
                {
                    g_object_unref(page);
                }

                if (document is not null)
                {
                    g_object_unref(document);
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

    private static PopplerPage* RenderCore(PopplerDocument* document, int pageIndex, CairoContext cr, bool printing, PopplerAnnotFlag flags)
    {
        PopplerPage* page = poppler_document_get_page(document, pageIndex);

        poppler_page_render_full(page, cr.NativeContext, printing, flags);

        return page;
    }
}
