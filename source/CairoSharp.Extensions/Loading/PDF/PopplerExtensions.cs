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
        /// <param name="pageIndex">a page index (zero-based)</param>
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
            using PdfDocument pdf = new(fileName);
            RenderCore(pdf, pageIndex, cr, printing, flags);
        }

        /// <summary>
        /// Loads the PDF data and renders it in cairo.
        /// </summary>
        /// <param name="pdfData">PDF data</param>
        /// <param name="pageIndex">a page index (zero-based)</param>
        /// <param name="printing"><c>true</c> for printing mode, <c>false</c> otherwise</param>
        /// <param name="flags">flags which allow to select which annotations to render</param>
        /// <remarks>
        /// See <see cref="LoadPdf(CairoContext, string, int, bool, PopplerAnnotFlag)"/> for the meaning of the
        /// <paramref name="printing"/> argument.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="pdfData"/> is empty</exception>
        /// <exception cref="PopplerException">an error occured</exception>
        public void LoadPdf(ReadOnlySpan<byte> pdfData, int pageIndex, bool printing = false, PopplerAnnotFlag flags = PopplerAnnotFlag.Unknown)
        {
            using PdfDocument pdf = new(pdfData);
            RenderCore(pdf, pageIndex, cr, printing, flags);
        }

        /// <summary>
        /// Loads the <paramref name="pdf"/> and renders it in cairo.
        /// </summary>
        /// <param name="pdf">PDF document</param>
        /// <param name="pageIndex">a page index (zero-based)</param>
        /// <param name="printing"><c>true</c> for printing mode, <c>false</c> otherwise</param>
        /// <param name="flags">flags which allow to select which annotations to render</param>
        /// <remarks>
        /// See <see cref="LoadPdf(CairoContext, string, int, bool, PopplerAnnotFlag)"/> for the meaning of the
        /// <paramref name="printing"/> argument.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="pdf"/> is <c>null</c></exception>
        /// <exception cref="PopplerException">an error occured</exception>
        public void LoadPdf(PdfDocument pdf, int pageIndex, bool printing = false, PopplerAnnotFlag flags = PopplerAnnotFlag.Unknown)
        {
            ArgumentNullException.ThrowIfNull(pdf);

            RenderCore(pdf, pageIndex, cr, printing, flags);
        }
    }

    private static void RenderCore(PdfDocument pdf, int pageIndex, CairoContext cr, bool printing, PopplerAnnotFlag flags)
    {
        PopplerPage* page = pdf.GetPage(pageIndex);

        if (PopplerVersion > CairoAPI.VersionEncode(25, 2, 0))
        {
            poppler_page_render_full(page, cr.NativeContext, printing, flags);
        }
        else
        {
            if (printing)
            {
                poppler_page_render_for_printing(page, cr.NativeContext);
            }
            else
            {
                poppler_page_render(page, cr.NativeContext);
            }
        }
    }
}
