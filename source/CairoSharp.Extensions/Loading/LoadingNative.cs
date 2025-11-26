// (c) gfoidl, all rights reserved

global using unsafe GDestroyNotify = delegate* unmanaged[Cdecl]<void*, void>;
global using RsvgRectangle         = Cairo.Rectangle;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Cairo.Extensions.Loading.PDF;
using Cairo.Extensions.Loading.SVG;

namespace Cairo.Extensions.Loading;

public static unsafe partial class LoadingNative
{
    public const string LibGLibName    = "libglib-2.0.so.0";
    public const string LibGObjectName = "libgobject-2.0.so.0";
    public const string LibGioName     = "libgio-2.0.so.0";
    public const string LibRSvgName    = "librsvg-2.so.2";
    public const string LibPopplerName = "libpoppler-glib.so.8";
    //-------------------------------------------------------------------------
    public static string? GetLibRsvgVersion()
    {
        // https://github.com/GNOME/librsvg/blob/0cca5327a5ebbe83013a74047181703e7ea402e2/librsvg-c/build.rs#L74-L81
        // So they're exposed as static variables, thus PInvoke can't be used.

        if (!NativeLibrary.TryLoad(LibRSvgName, out nint libHandle) && DllImportResolver is not null)
        {
            libHandle = DllImportResolver(LibRSvgName, Assembly.GetExecutingAssembly(), null);
        }

        if (libHandle != 0)
        {
            if (NativeLibrary.TryGetExport(libHandle, "rsvg_major_version", out nint majorAddress)
             && NativeLibrary.TryGetExport(libHandle, "rsvg_minor_version", out nint minorAddress)
             && NativeLibrary.TryGetExport(libHandle, "rsvg_micro_version", out nint microAddress))
            {
                int major = *(int*)majorAddress;
                int minor = *(int*)minorAddress;
                int micro = *(int*)microAddress;

                return $"{major}.{minor}.{micro}";
            }
        }

        return null;
    }

    public static string? GetPopplerVersion() => poppler_get_version();

    public static int PopplerVersion
    {
        get
        {
            if (field == 0)
            {
                field = GetPopplerVersionAsInt();
            }

            return field;
        }
    }

    private static int GetPopplerVersionAsInt()
    {
        ReadOnlySpan<char> version = GetPopplerVersion();

        if (version.IsEmpty)
        {
            return -1;
        }

        int idx = version.IndexOf('.');
        if (!int.TryParse(version[..idx], out int major))
        {
            return -1;
        }

        version = version.Slice(idx + 1);
        idx     = version.IndexOf('.');
        if (!int.TryParse(version[..idx], out int minor))
        {
            return -1;
        }

        version = version.Slice(idx + 1);
        if (!int.TryParse(version, out int patch))
        {
            return -1;
        }

        return CairoAPI.VersionEncode(major, minor, patch);
    }
    //-------------------------------------------------------------------------
    [LibraryImport(LibGLibName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void g_free(void* mem);

    [LibraryImport(LibGLibName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void g_error_free(GError* error);

    [LibraryImport(LibGObjectName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void g_object_unref(void* @object);
    //-------------------------------------------------------------------------
    [LibraryImport(LibGioName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial GFile* g_file_new_for_path(string path);

    [LibraryImport(LibGioName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial GInputStream* g_memory_input_stream_new_from_data(void* data, nint size, GDestroyNotify destroy);
    //-------------------------------------------------------------------------
    // librsvg
    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial RsvgHandle* rsvg_handle_new_from_gfile_sync(GFile* file, RsvgHandleFlags flags, void* cancellable, GError** error);

    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial RsvgHandle* rsvg_handle_new_from_data(byte* data, nint data_len, GError** error);

    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial RsvgHandle* rsvg_handle_new_from_stream_sync(GInputStream* input_stream, GFile* base_file, RsvgHandleFlags flags, void* cancellable, GError** error);

    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void rsvg_handle_set_dpi(RsvgHandle* handle, double dpi);

    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool rsvg_handle_render_document(RsvgHandle* handle, cairo_t* cr, RsvgRectangle* viewport, GError** error);

#if LOADING_SVG_RENDER_ELEMENTS
    [LibraryImport(LibRSvgName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool rsvg_handle_render_element(RsvgHandle* handle, cairo_t* cr, string id, RsvgRectangle* element_viewport, GError** error);

    [LibraryImport(LibRSvgName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool rsvg_handle_render_layer(RsvgHandle* handle, cairo_t* cr, string id, RsvgRectangle* viewport, GError** error);
#endif

    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string? rsvg_handle_get_base_uri(RsvgHandle* handle);

    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool rsvg_handle_get_intrinsic_size_in_pixels(RsvgHandle* handle, out double out_width, out double out_height);

    [LibraryImport(LibRSvgName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void rsvg_handle_get_intrinsic_dimensions(RsvgHandle* handle, out uint out_has_width, out SvgLength out_width, out uint out_has_height, out SvgLength out_height, out uint out_has_viewbox, RsvgRectangle* out_viewbox);
    //-------------------------------------------------------------------------
    // Poppler
    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string? poppler_get_version();

    [LibraryImport(LibPopplerName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PopplerDocument* poppler_document_new_from_gfile(GFile* file, string? password, void* cancellable, GError** error);

    [LibraryImport(LibPopplerName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PopplerDocument* poppler_document_new_from_stream(GInputStream* stream, long length, string? password, void* cancellable, GError** error);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PopplerPage* poppler_document_get_page(PopplerDocument* document, int index);

    [LibraryImport(LibPopplerName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PopplerPage* poppler_document_get_page_by_label(PopplerDocument* document, string label);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void poppler_page_render_full(PopplerPage* page, cairo_t* cairo, [MarshalAs(UnmanagedType.U4)] bool printing, PopplerAnnotFlag flags);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void poppler_page_render(PopplerPage* page, cairo_t* cairo);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void poppler_page_render_for_printing(PopplerPage* page, cairo_t* cairo);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(GCharMarshaller))]
    internal static partial string? poppler_document_get_author(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(GCharMarshaller))]
    internal static partial string? poppler_document_get_creator(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(GCharMarshaller))]
    internal static partial string? poppler_document_get_metadata(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int poppler_document_get_n_pages(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void poppler_document_get_pdf_version(PopplerDocument* document, out uint major_version, out uint minor_version);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(GCharMarshaller))]
    internal static partial string? poppler_document_get_pdf_version_string(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(GCharMarshaller))]
    internal static partial string? poppler_document_get_producer(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(GCharMarshaller))]
    internal static partial string? poppler_document_get_subject(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(GCharMarshaller))]
    internal static partial string? poppler_document_get_title(PopplerDocument* document);

    [LibraryImport(LibPopplerName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void poppler_page_get_size(PopplerPage* page, out double width, out double height);
}
