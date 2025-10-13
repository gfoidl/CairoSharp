// (c) gfoidl, all rights reserved

global using unsafe GDestroyNotify = delegate*<void*, void>;
global using RsvgRectangle         = Cairo.Rectangle;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Loading.SVG;

public static unsafe partial class LibRSvgNative
{
    public const string LibRSvgName    = "librsvg-2.so";
    public const string LigGObjectName = "libgobject-2.0.so";
    public const string LibGioName     = "libgio-2.0.so";

    [DisallowNull]
    public static DllImportResolver? DllImportResolver
    {
        get => field;
        set
        {
            field = value;
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), value);
        }
    }
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
                int* major = (int*)majorAddress;
                int* minor = (int*)minorAddress;
                int* micro = (int*)microAddress;

                return $"{*major}.{*minor}.{*micro}";
            }
        }

        return null;
    }
    //-------------------------------------------------------------------------
    [LibraryImport(LibGioName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial GFile* g_file_new_for_path(string path);

    [LibraryImport(LibGioName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial GInputStream* g_memory_input_stream_new_from_data(void* data, nint size, GDestroyNotify destroy);
    //-------------------------------------------------------------------------
    [LibraryImport(LigGObjectName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void g_object_unref(void* @object);
    //-------------------------------------------------------------------------
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
}
