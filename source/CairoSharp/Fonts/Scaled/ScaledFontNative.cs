// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Cairo.Drawing.Text;

namespace Cairo.Fonts.Scaled;

// https://www.cairographics.org/manual/cairo-cairo-scaled-font-t.html

internal struct cairo_scaled_font_t;

internal static unsafe partial class ScaledFontNative
{
    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_scaled_font_t* cairo_scaled_font_create(cairo_font_face_t* font_face, ref Matrix font_matrix, ref Matrix ctm, cairo_font_options_t* options);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial cairo_scaled_font_t* cairo_scaled_font_reference(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_destroy(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial Status cairo_scaled_font_status(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_extents(cairo_scaled_font_t* scaled_font, out FontExtents extents);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_text_extents(cairo_scaled_font_t* scaled_font, string text, out TextExtents extents);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_text_extents(cairo_scaled_font_t* scaled_font, [MarshalUsing(typeof(Utf8SpanMarshaller))] ReadOnlySpan<byte> utf8, out TextExtents extents);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_glyph_extents(cairo_scaled_font_t* scaled_font, Glyph* glyphs, int num_glyphs, out TextExtents extents);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_scaled_font_text_to_glyphs(cairo_scaled_font_t* scaled_font, double x, double y, string text, int text_len, out Glyph* glyphs, out int num_glyphs, TextCluster** clusters, int* num_clusters, ClusterFlags* flags);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_scaled_font_text_to_glyphs(cairo_scaled_font_t* scaled_font, double x, double y, ReadOnlySpan<byte> utf8, int utf8_len, out Glyph* glyphs, out int num_glyphs, TextCluster** clusters, int* num_clusters, ClusterFlags* flags);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_scaled_font_get_font_face(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_font_options(cairo_scaled_font_t* scaled_font, cairo_font_options_t* options);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_font_matrix(cairo_scaled_font_t* scaled_font, out Matrix font_matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_ctm(cairo_scaled_font_t* scaled_font, out Matrix ctm);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_scaled_font_get_scale_matrix(cairo_scaled_font_t* scaled_font, out Matrix scale_matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FontType cairo_scaled_font_get_type(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial uint cairo_scaled_font_get_reference_count(cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_scaled_font_set_user_data(cairo_scaled_font_t* scaled_font, ref UserDataKey key, void* user_data, cairo_destroy_func_t destroy);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void* cairo_scaled_font_get_user_data(cairo_scaled_font_t* scaled_font, ref UserDataKey key);
}
