// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using Cairo.Extensions.GObject;

namespace Cairo.Extensions.Pango;

internal static unsafe partial class PangoFontMapNative
{
    // https://docs.gtk.org/PangoCairo/type_func.FontMap.get_default.html
    [LibraryImport(PangoNative.LibPangoCairoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial pango_font_map* pango_cairo_font_map_get_default();

    // https://docs.gtk.org/Pango/method.FontMap.add_font_file.html
    [LibraryImport(PangoNative.LibPangoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool pango_font_map_add_font_file(pango_font_map* fontmap, string filename, GError** error);

    // https://docs.gtk.org/Pango/method.FontMap.get_family.html
    [LibraryImport(PangoNative.LibPangoName, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial pango_font_family* pango_font_map_get_family(pango_font_map* fontmap, string name);

    // https://docs.gtk.org/Pango/method.FontMap.list_families.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_font_map_list_families(pango_font_map* fontmap, pango_font_family*** families, int* n_families);
}
