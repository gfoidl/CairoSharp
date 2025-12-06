// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.Extensions.Pango;

internal static unsafe partial class FontFamilyNative
{
    // https://docs.gtk.org/Pango/method.FontFamily.get_name.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    internal static partial string pango_font_family_get_name(pango_font_family* family);

    // https://docs.gtk.org/Pango/method.FontFamily.is_monospace.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool pango_font_family_is_monospace(pango_font_family* family);

    // https://docs.gtk.org/Pango/method.FontFamily.is_variable.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    internal static partial bool pango_font_family_is_variable(pango_font_family* family);
}
