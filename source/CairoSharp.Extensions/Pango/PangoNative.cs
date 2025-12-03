// (c) gfoidl, all rights reserved

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.Extensions.Pango;

[EditorBrowsable(EditorBrowsableState.Never)]
public struct pango_layout;
internal struct pango_attr_list;

public static unsafe partial class PangoNative
{
    public const string LibPangoName      = "libpango.so.1";
    public const string LibPangoCairoName = "libpangocairo.so.1";
    public const string LibGObjectName    = "libgobject-2.0.so.0";

    // https://docs.gtk.org/PangoCairo/func.create_layout.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial pango_layout* pango_cairo_create_layout(cairo_t* cr);

    // https://docs.gtk.org/Pango/method.Layout.get_size.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_get_size(pango_layout* layout, out int width, out int height);

    // https://docs.gtk.org/Pango/method.Layout.get_baseline.html
    [LibraryImport(LibPangoCairoName)]
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

    // https://docs.gtk.org/Pango/method.Layout.set_text.html
    [LibraryImport(LibPangoCairoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_text(pango_layout* layout, string text, int length);

    // https://docs.gtk.org/Pango/method.Layout.get_text.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string pango_layout_get_text(pango_layout* layout);

    // https://docs.gtk.org/Pango/method.Layout.set_markup.html
    [LibraryImport(LibPangoCairoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_markup(pango_layout* layout, string markup, int length);

    // https://docs.gtk.org/Pango/method.Layout.set_attributes.html
    [LibraryImport(LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_layout_set_attributes(pango_layout* layout, pango_attr_list* attrs);
}
