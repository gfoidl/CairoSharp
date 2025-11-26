// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Drawing.Transformations;

// https://www.cairographics.org/manual/cairo-Transformations.html

internal static unsafe partial class TransformationsNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_translate(cairo_t* cr, double tx, double ty);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_scale(cairo_t* cr, double sx, double sy);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_rotate(cairo_t* cr, double angle);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_transform(cairo_t* cr, ref Matrix matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_set_matrix(cairo_t* cr, ref Matrix matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_get_matrix(cairo_t* cr, out Matrix matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_identity_matrix(cairo_t* cr);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_user_to_device(cairo_t* cr, ref double x, ref double y);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_user_to_device_distance(cairo_t* cr, ref double dx, ref double dy);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_device_to_user(cairo_t* cr, ref double x, ref double y);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void cairo_device_to_user_distance(cairo_t* cr, ref double dx, ref double dy);
}
