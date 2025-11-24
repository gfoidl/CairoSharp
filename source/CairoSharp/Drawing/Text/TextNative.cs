// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Cairo.Fonts;
using Cairo.Fonts.Scaled;

namespace Cairo.Drawing.Text;

// https://www.cairographics.org/manual/cairo-text.html

internal static unsafe partial class TextNative
{
    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_select_font_face(cairo_t* cr, string family, FontSlant slant, FontWeight weight);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_set_font_size(cairo_t* cr, double size);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_set_font_matrix(cairo_t* cr, ref Matrix matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_get_font_matrix(cairo_t* cr, out Matrix matrix);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_set_font_options(cairo_t* cr, cairo_font_options_t* options);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_get_font_options(cairo_t* cr, cairo_font_options_t* options);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_set_font_face(cairo_t* cr, cairo_font_face_t* fontFace);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_get_font_face(cairo_t* cr);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_set_scaled_font(cairo_t* cr, cairo_scaled_font_t* scaled_font);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_scaled_font_t* cairo_get_scaled_font(cairo_t* cr);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_show_text(cairo_t* cr, string text);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_show_text(cairo_t* cr, [MarshalUsing(typeof(Utf8SpanMarshaller))] ReadOnlySpan<byte> utf8);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_show_glyphs(cairo_t* cr, Glyph* glyphs, int num_glyphs);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_show_text_glyphs(cairo_t* cr, string text, int utf8_len, Glyph* glyphs, int num_glyphs, TextCluster* clusters, int num_clusters, ClusterFlags cluster_flags);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_show_text_glyphs(cairo_t* cr, ReadOnlySpan<byte> utf8, int utf8_len, Glyph* glyphs, int num_glyphs, TextCluster* clusters, int num_clusters, ClusterFlags cluster_flags);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_font_extents(cairo_t* cr, out FontExtents extents);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_text_extents(cairo_t* cr, string? text, out TextExtents extents);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_text_extents(cairo_t* cr, [MarshalUsing(typeof(Utf8SpanMarshaller))] ReadOnlySpan<byte> utf8, out TextExtents extents);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_glyph_extents(cairo_t* cr, Glyph* glyphs, int num_glyphs, out TextExtents extents);

    [LibraryImport(Native.LibCairo, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_font_face_t* cairo_toy_font_face_create(string family, FontSlant slant, FontWeight weight);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string? cairo_toy_font_face_get_family(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FontSlant cairo_toy_font_face_get_slant(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FontWeight cairo_toy_font_face_get_weight(cairo_font_face_t* font_face);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Glyph* cairo_glyph_allocate(int num_glyphs);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_glyph_free(Glyph* glyphs);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial TextCluster* cairo_text_cluster_allocate(int num_clusters);

    [LibraryImport(Native.LibCairo)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_text_cluster_free(TextCluster* clusters);
}
