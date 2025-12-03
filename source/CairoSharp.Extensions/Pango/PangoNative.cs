// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Cairo.Fonts;

namespace Cairo.Extensions.Pango;

internal struct PangoContext;
internal struct PangoLayout;

public static unsafe partial class PangoNative
{
    public const string LibPangoName      = "libpango.so.1";
    public const string LibPangoCairoName = "libpangocairo.so.1";

    // https://docs.gtk.org/PangoCairo/func.context_set_font_options.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_context_set_font_options(PangoContext* context, cairo_font_options_t* options);

    // https://docs.gtk.org/PangoCairo/func.context_set_resolution.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_context_set_resolution(PangoContext* context, double dpi);

    // https://docs.gtk.org/PangoCairo/func.create_context.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PangoContext* pango_cairo_create_context(cairo_t* cr);

    // https://docs.gtk.org/PangoCairo/func.create_layout.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial PangoLayout* pango_cairo_create_layout(cairo_t* cr);

    // https://docs.gtk.org/PangoCairo/func.layout_path.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_layout_path(cairo_t* cr, PangoLayout* layout);

    // https://docs.gtk.org/PangoCairo/func.show_layout.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_show_layout(cairo_t* cr, PangoLayout* layout);

    // https://docs.gtk.org/PangoCairo/func.update_context.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_update_context(cairo_t* cr, PangoContext* context);

    // https://docs.gtk.org/PangoCairo/func.update_layout.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_cairo_update_layout(cairo_t* cr, PangoLayout* layout);

    // https://docs.gtk.org/Pango/method.Layout.get_size.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_get_size(PangoLayout* layout, out int width, out int height);

    // https://docs.gtk.org/Pango/method.Layout.get_baseline.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial int pango_layout_get_baseline(PangoLayout* layout);

    // https://docs.gtk.org/Pango/method.Layout.get_text.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string pango_layout_get_text(PangoLayout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_text.html
    [LibraryImport(LibPangoCairoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_text(PangoLayout* layout, string text, int length);

    // https://docs.gtk.org/Pango/method.Layout.set_markup.html
    [LibraryImport(LibPangoCairoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_markup(PangoLayout* layout, string markup, int length);
}
