// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Surfaces.Tee;

// https://www.cairographics.org/manual/cairo-Tee-surface.html

internal static unsafe partial class TeeSurfaceNative
{
    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_tee_surface_create(void* primary);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_tee_surface_add(void* abstract_surface, void* target);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_tee_surface_index(void* abstract_surface, uint index);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_tee_surface_remove(void* abstract_surface, void* target);
}
