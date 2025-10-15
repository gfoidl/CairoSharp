// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Surfaces.Tee;

// https://www.cairographics.org/manual/cairo-Tee-surface.html

internal static unsafe partial class TeeSurfaceNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_tee_surface_create(cairo_surface_t* primary);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_tee_surface_add(cairo_surface_t* abstract_surface, cairo_surface_t* target);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_surface_t* cairo_tee_surface_index(cairo_surface_t* abstract_surface, uint index);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_tee_surface_remove(cairo_surface_t* abstract_surface, cairo_surface_t* target);
}
