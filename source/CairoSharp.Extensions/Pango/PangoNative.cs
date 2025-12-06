// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Cairo.Extensions.GObject;

namespace Cairo.Extensions.Pango;

public static unsafe partial class PangoNative
{
    public const string LibPangoName      = "libpango.so.1";
    public const string LibPangoCairoName = "libpangocairo.so.1";
    public const string LibGObjectName    = GObjectNative.LibGObjectName;
    public const string LibGLibName       = GObjectNative.LibGLibName;

    // https://docs.gtk.org/PangoCairo/func.create_layout.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial pango_layout* pango_cairo_create_layout(cairo_t* cr);

    // https://docs.gtk.org/Pango/method.Layout.set_font_description.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_font_description(pango_layout* layout, pango_font_description* desc);

    // https://docs.gtk.org/Pango/type_func.FontDescription.from_string.html
    [LibraryImport(LibPangoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial pango_font_description* pango_font_description_from_string(string str);

    // https://docs.gtk.org/Pango/method.FontDescription.free.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_font_description_free(pango_font_description* desc);

    // https://docs.gtk.org/Pango/method.Layout.get_size.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_get_size(pango_layout* layout, out int width, out int height);

    // https://docs.gtk.org/Pango/method.Layout.get_line_count.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int pango_layout_get_line_count(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.get_character_count.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int pango_layout_get_character_count(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.get_baseline.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int pango_layout_get_baseline(pango_layout* layout);

    // https://docs.gtk.org/PangoCairo/func.layout_path.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_layout_path(cairo_t* cr, pango_layout* layout);

    // https://docs.gtk.org/PangoCairo/func.show_layout.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_show_layout(cairo_t* cr, pango_layout* layout);

    // https://docs.gtk.org/PangoCairo/func.update_layout.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_update_layout(cairo_t* cr, pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.get_text.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string pango_layout_get_text(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_text.html
    [LibraryImport(LibPangoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_text(pango_layout* layout, string text, int length);

    // https://docs.gtk.org/Pango/method.Layout.set_text.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_text(pango_layout* layout, [MarshalUsing(typeof(Utf8SpanMarshaller))] ReadOnlySpan<byte> text, int length);

    // https://docs.gtk.org/Pango/method.Layout.set_markup.html
    [LibraryImport(LibPangoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_markup(pango_layout* layout, string markup, int length);

    // https://docs.gtk.org/Pango/method.Layout.set_markup.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_markup(pango_layout* layout, [MarshalUsing(typeof(Utf8SpanMarshaller))] ReadOnlySpan<byte> markup, int length);

    // https://docs.gtk.org/Pango/method.Layout.set_attributes.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_attributes(pango_layout* layout, pango_attr_list* attrs);

    // https://docs.gtk.org/Pango/method.Layout.get_context.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial pango_context* pango_layout_get_context(pango_layout* layout);

    // https://docs.gtk.org/PangoCairo/func.context_get_resolution.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial double pango_cairo_context_get_resolution(pango_context* context);

    // https://docs.gtk.org/PangoCairo/func.context_set_resolution.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_cairo_context_set_resolution(pango_context* context, double dpi);

    // https://docs.gtk.org/Pango/method.Layout.get_width.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial int pango_layout_get_width(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_width.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_width(pango_layout* layout, int width);

    // https://docs.gtk.org/Pango/method.Layout.get_height.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial int pango_layout_get_height(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_height.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_height(pango_layout* layout, int height);

    // https://docs.gtk.org/Pango/method.Layout.is_ellipsized.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool pango_layout_is_ellipsized(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.get_ellipsize.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial EllipsizeMode pango_layout_get_ellipsize(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_ellipsize.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_ellipsize(pango_layout* layout, EllipsizeMode ellipsize);

    // https://docs.gtk.org/Pango/method.Layout.is_wrapped.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool pango_layout_is_wrapped(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.get_wrap.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial WrapMode pango_layout_get_wrap(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_wrap.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_wrap(pango_layout* layout, WrapMode wrap);

    // https://docs.gtk.org/Pango/method.Layout.get_alignment.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial Alignment pango_layout_get_alignment(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_alignment.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_alignment(pango_layout* layout, Alignment alignment);

    // https://docs.gtk.org/Pango/method.Layout.get_justify.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool pango_layout_get_justify(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_justify.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_justify(pango_layout* layout, [MarshalAs(UnmanagedType.U4)] bool justify);

    // https://docs.gtk.org/Pango/method.Layout.get_justify_last_line.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool pango_layout_get_justify_last_line(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_justify_last_line.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_justify_last_line(pango_layout* layout, [MarshalAs(UnmanagedType.U4)] bool justify);

    // https://docs.gtk.org/Pango/method.Layout.get_line_spacing.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial float pango_layout_get_line_spacing(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_line_spacing.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_line_spacing(pango_layout* layout, float factor);

    // https://docs.gtk.org/Pango/method.Layout.get_spacing.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial int pango_layout_get_spacing(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_spacing.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_spacing(pango_layout* layout, int spacing);

    // https://docs.gtk.org/Pango/method.Layout.get_indent.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial int pango_layout_get_indent(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_indent.html
    [LibraryImport(LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [SuppressGCTransition]
    internal static partial void pango_layout_set_indent(pango_layout* layout, int indent);
}
