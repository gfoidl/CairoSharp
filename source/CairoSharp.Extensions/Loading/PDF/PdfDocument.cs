// (c) gfoidl, all rights reserved

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Cairo.Extensions.Loading.LoadingNative;

namespace Cairo.Extensions.Loading.PDF;

public sealed unsafe class PdfDocument : Document
{
    private Dictionary<int, nint>? _pages;
    private void*                  _fileOrStream;
    private PopplerDocument*       _document;

    /// <summary>
    /// Loads the PDF file.
    /// </summary>
    /// <param name="fileName">PDF file</param>
    /// <param name="password">password to unlock the file with, or <c>null</c></param>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <c>null</c></exception>
    /// <exception cref="PopplerException">an error occured</exception>
    public PdfDocument(string fileName, string? password = null)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        GError* error;

        _fileOrStream = g_file_new_for_path(fileName);
        _document     = poppler_document_new_from_gfile((GFile*)_fileOrStream, password, cancellable: null, &error);

        this.FinishConstruction(error);
    }

    /// <summary>
    /// Loads the PDF data
    /// </summary>
    /// <param name="pdfData">PDF data</param>
    /// <param name="password">password to unlock the file with, or <c>null</c></param>
    /// <exception cref="ArgumentException"><paramref name="pdfData"/> is empty</exception>
    /// <exception cref="PopplerException">an error occured</exception>
    public PdfDocument(ReadOnlySpan<byte> pdfData, string? password = null)
    {
        if (pdfData.IsEmpty)
        {
            throw new ArgumentException("no data given", nameof(pdfData));
        }

        GError* error;

        fixed (byte* ptr = pdfData)
        {
            _fileOrStream = g_memory_input_stream_new_from_data(ptr, (nint)pdfData.Length, &DummyDestroyNotify);
            _document     = poppler_document_new_from_stream((GInputStream*)_fileOrStream, pdfData.Length, password, cancellable: null, &error);
        }

        this.FinishConstruction(error);

        // Let the GC do it's thing.
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void DummyDestroyNotify(void* _) { }
    }

    private void FinishConstruction(GError* error)
    {
        if (_document is null)
        {
            throw new PopplerException(error);
        }
    }

    protected override void DisposeCore()
    {
        if (_pages is not null)
        {
            foreach (PopplerPage* page in _pages.Values)
            {
                g_object_unref(page);
            }

            _pages = null;
        }

        if (_document is not null)
        {
            g_object_unref(_document);
            _document = null;
        }

        if (_fileOrStream is not null)
        {
            g_object_unref(_fileOrStream);
            _fileOrStream = null;
        }
    }

    protected override void CheckNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_document     is null, this);
        ObjectDisposedException.ThrowIf(_fileOrStream is null, this);
    }

    internal PopplerDocument* Handle => _document;

    [return: NotNull]
    internal PopplerPage* GetPage(int pageIndex)
    {
        this.CheckNotDisposed();

        _pages ??= [];

        ref nint handle = ref CollectionsMarshal.GetValueRefOrAddDefault(_pages, pageIndex, out bool exists);

        if (!exists)
        {
            handle = (nint)poppler_document_get_page(_document, pageIndex);

            if (handle == 0)
            {
                throw new PopplerException($"""
                    PDF page at index {pageIndex} could not be loaded.
                    Note: the page index is 0-based.
                    """);
            }
        }

        return (PopplerPage*)handle;
    }

    /// <summary>
    /// Gets the size of page given by <paramref name="pageIndex"/> at the current scale and rotation.
    /// </summary>
    /// <param name="pageIndex">a page index (zero-based)</param>
    /// <param name="width">the width of the page</param>
    /// <param name="height">the height of the page</param>
    public void GetPageSize(int pageIndex, out double width, out double height)
    {
        this.CheckNotDisposed();

        PopplerPage* page = this.GetPage(pageIndex);
        poppler_page_get_size(page, out width, out height);
    }

    /// <summary>
    /// Returns the author of the document.
    /// </summary>
    public string? Author
    {
        get
        {
            this.CheckNotDisposed();

            return poppler_document_get_author(_document);
        }
    }

    /// <summary>
    /// Returns the creator of the document. If the document was converted from another format, the creator
    /// is the name of the product that created the original document from which it was converted.
    /// </summary>
    public string? Creator
    {
        get
        {
            this.CheckNotDisposed();

            return poppler_document_get_creator(_document);
        }
    }

    /// <summary>
    /// Returns the XML metadata string of the document.
    /// </summary>
    public string? MetaData
    {
        get
        {
            this.CheckNotDisposed();

            return poppler_document_get_metadata(_document);
        }
    }

    /// <summary>
    /// Returns the number of pages in a loaded document.
    /// </summary>
    public int NumberOfPages
    {
        get
        {
            this.CheckNotDisposed();
            return poppler_document_get_n_pages(_document);
        }
    }

    /// <summary>
    /// Gets the major and minor PDF versions.
    /// </summary>
    /// <param name="majorVersion">the PDF major version number</param>
    /// <param name="minorVersion">the PDF minor version number</param>
    public void GetPdfVersion(out int majorVersion, out int minorVersion)
    {
        this.CheckNotDisposed();

        poppler_document_get_pdf_version(_document, out uint ma, out uint mi);
        majorVersion = (int)ma;
        minorVersion = (int)mi;
    }

    /// <summary>
    /// Returns the PDF version of document as a string (e.g. PDF-1.6).
    /// </summary>
    public string? PdfVersion
    {
        get
        {
            this.CheckNotDisposed();

            return poppler_document_get_pdf_version_string(_document);
        }
    }

    /// <summary>
    /// Returns the producer of the document. If the document was converted from another format, the producer
    /// is the name of the product that converted it to PDF.
    /// </summary>
    public string? Producer
    {
        get
        {
            this.CheckNotDisposed();

            return poppler_document_get_producer(_document);
        }
    }

    /// <summary>
    /// Returns the subject of the document.
    /// </summary>
    public string? Subject
    {
        get
        {
            this.CheckNotDisposed();

            return poppler_document_get_subject(_document);
        }
    }

    /// <summary>
    /// Returns the document's title.
    /// </summary>
    public string? Title
    {
        get
        {
            this.CheckNotDisposed();

            return poppler_document_get_title(_document);
        }
    }
}
