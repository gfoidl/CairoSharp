// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Surfaces.SVG;

// https://www.cairographics.org/manual/cairo-SVG-Surfaces.html

internal static unsafe partial class SvgSurfaceNative
{
    [LibraryImport(NativeMethods.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_svg_surface_create(string? fileName, double width_in_points, double height_in_points);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_svg_surface_create_for_stream(cairo_write_func_t write_func, void* closure, double width_in_points, double height_in_points);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial SvgUnit cairo_svg_surface_get_document_unit(void* surface);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_svg_surface_set_document_unit(void* surface, SvgUnit unit);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_svg_surface_restrict_to_version(void* surface, SvgVersion version);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_svg_get_versions(out SvgVersion* versions, out int num_versions);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial sbyte* cairo_svg_version_to_string(SvgVersion version);
}
