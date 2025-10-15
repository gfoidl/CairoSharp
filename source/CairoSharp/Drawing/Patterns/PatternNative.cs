// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Cairo.Drawing.Path;
using Cairo.Surfaces;

namespace Cairo.Drawing.Patterns;

// https://www.cairographics.org/manual/cairo-cairo-pattern-t.html

internal static unsafe partial class PatternNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_add_color_stop_rgb(cairo_pattern_t* pattern, double offset, double red, double green, double blue);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_add_color_stop_rgba(cairo_pattern_t* pattern, double offset, double red, double green, double blue, double alpha);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_get_color_stop_count(cairo_pattern_t* pattern, out int count);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_get_color_stop_rgba(cairo_pattern_t* pattern, int index, double* offset, double* red, double* green, double* blue, double* alpha);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_create_rgb(double red, double green, double blue);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_create_rgba(double red, double green, double blue, double alpha);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_get_rgba(cairo_pattern_t* pattern, double* red, double* green, double* blue, double* alpha);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_create_for_surface(cairo_surface_t* surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_get_surface(cairo_pattern_t* pattern, out cairo_surface_t* surface);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_create_linear(double x0, double y0, double x1, double y1);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_get_linear_points(cairo_pattern_t* pattern, double* x0, double* y0, double* x1, double* y1);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_create_radial(double cx0, double cy0, double radius0, double cx1, double cy1, double radius1);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_get_radial_circles(cairo_pattern_t* pattern, double* x0, double* y0, double* r0, double* x1, double* y1, double* r1);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_create_mesh();

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_begin_patch(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_end_patch(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_move_to(cairo_pattern_t* pattern, double x, double y);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_line_to(cairo_pattern_t* pattern, double x, double y);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_curve_to(cairo_pattern_t* pattern, double x1, double y1, double x2, double y2, double x3, double y3);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_set_control_point(cairo_pattern_t* pattern, uint point_num, double x, double y);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_set_corner_color_rgb(cairo_pattern_t* pattern, uint corner_num, double red, double green, double blue);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_mesh_pattern_set_corner_color_rgba(cairo_pattern_t* pattern, uint corner_num, double red, double green, double blue, double alpha);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_mesh_pattern_get_patch_count(cairo_pattern_t* pattern, out uint count);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PathRaw* cairo_mesh_pattern_get_path(cairo_pattern_t* pattern, uint patch_num);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_mesh_pattern_get_control_point(cairo_pattern_t* pattern, uint patch_num, uint point_num, double* x, double* y);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_mesh_pattern_get_corner_color_rgba(cairo_pattern_t* pattern, uint patch_num, uint corner_num, double* red, double* green, double* blue, double* alpha);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_pattern_t* cairo_pattern_reference(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_destroy(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_status(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_set_extend(cairo_pattern_t* pattern, Extend extend);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Extend cairo_pattern_get_extend(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_set_filter(cairo_pattern_t* pattern, Filter filter);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Filter cairo_pattern_get_filter(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_set_matrix(cairo_pattern_t* pattern, ref Matrix matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_get_matrix(cairo_pattern_t* pattern, out Matrix matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PatternType cairo_pattern_get_type(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial uint cairo_pattern_get_reference_count(cairo_pattern_t* pattern);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_pattern_set_user_data(cairo_pattern_t* pattern, ref UserDataKey key, void* user_data, cairo_destroy_func_t destroy);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_pattern_get_user_data(cairo_pattern_t* pattern, ref UserDataKey key);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_pattern_set_dither(cairo_pattern_t* pattern, Dither dither);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Dither cairo_pattern_get_dither(cairo_pattern_t* pattern);
}
