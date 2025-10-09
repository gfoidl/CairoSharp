// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Cairo.Drawing.Text;
using Cairo.Utilities;

namespace Cairo.Fonts.Scaled;

// https://www.cairographics.org/manual/cairo-cairo-scaled-font-t.html

internal static unsafe partial class ScaledFontNative
{
    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_scaled_font_create(void* font_face, ref Matrix font_matrix, ref Matrix ctm, void* options);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_scaled_font_reference(void* scaled_font);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_destroy(void* scaled_font);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_scaled_font_status(void* scaled_font);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_extents(void* scaled_font, out FontExtents extents);

    [LibraryImport(NativeMethods.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_text_extents(void* scaled_font, string utf8, out TextExtents extents);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_glyph_extents(void* scaled_font, Glyph* glyphs, int num_glyphs, out TextExtents extents);

    [LibraryImport(NativeMethods.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_scaled_font_text_to_glyphs(void* scaled_font, double x, double y, string utf8, int utf8_len, out Glyph* glyphs, out int num_glyphs, out TextCluster* clusters, out int num_clusters, out ClusterFlags flags);

    [LibraryImport(NativeMethods.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_scaled_font_text_to_glyphs(void* scaled_font, double x, double y, string utf8, int utf8_len, out Glyph* glyphs, out int num_glyphs, TextCluster** clusters, int* num_clusters, ClusterFlags* flags);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_scaled_font_get_font_face(void* scaled_font);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_font_options(void* scaled_font, void* options);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_font_matrix(void* scaled_font, out Matrix font_matrix);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_ctm(void* scaled_font, out Matrix ctm);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_scale_matrix(void* scaled_font, out Matrix scale_matrix);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FontType cairo_scaled_font_get_type(void* scaled_font);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial uint cairo_scaled_font_get_reference_count(void* scaled_font);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_scaled_font_set_user_data(void* scaled_font, ref UserDataKey key, void* user_data, cairo_destroy_func_t destroy);

    [LibraryImport(NativeMethods.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_scaled_font_get_user_data(void* scaled_font, ref UserDataKey key);
}
