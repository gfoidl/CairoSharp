// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.Surfaces.SVG;

// https://www.cairographics.org/manual/cairo-SVG-Surfaces.html

internal static unsafe partial class SvgSurfaceNative
{
    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_svg_surface_create(string? fileName, double width_in_points, double height_in_points);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_svg_surface_create_for_stream(cairo_write_func_t write_func, void* closure, double width_in_points, double height_in_points);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial SvgUnit cairo_svg_surface_get_document_unit(cairo_surface_t* surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_svg_surface_set_document_unit(cairo_surface_t* surface, SvgUnit unit);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_svg_surface_restrict_to_version(cairo_surface_t* surface, SvgVersion version);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_svg_get_versions(out SvgVersion* versions, out int num_versions);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string? cairo_svg_version_to_string(SvgVersion version);
}
