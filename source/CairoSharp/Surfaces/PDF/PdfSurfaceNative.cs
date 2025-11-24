// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.Surfaces.PDF;

// https://www.cairographics.org/manual/cairo-PDF-Surfaces.html

internal static unsafe partial class PdfSurfaceNative
{
    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_pdf_surface_create(string? filename, double width_in_points, double height_in_points);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_pdf_surface_create_for_stream(cairo_write_func_t write_func, void* closure, double width_in_points, double height_in_points);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pdf_surface_restrict_to_version(cairo_surface_t* surface, PdfVersion version);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pdf_get_versions(out PdfVersion* versions, out int num_versions);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string? cairo_pdf_version_to_string(PdfVersion version);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pdf_surface_set_size(cairo_surface_t* surface, double width_in_points, double height_in_points);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int cairo_pdf_surface_add_outline(cairo_surface_t* surface, int parent_id, string utf8, string link_attribs, PdfOutlineFlags flags);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pdf_surface_set_metadata(cairo_surface_t* surface, PdfMetadata metadata, string utf8);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pdf_surface_set_custom_metadata(cairo_surface_t* surface, string name, string? value);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pdf_surface_set_page_label(cairo_surface_t* surface, string utf8);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pdf_surface_set_thumbnail_size(cairo_surface_t* surface, int width, int height);
}
