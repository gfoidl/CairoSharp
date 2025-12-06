// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.Extensions.Pango;

internal static unsafe partial class PangoFontFaceNative
{
    // https://docs.gtk.org/Pango/method.FontFace.describe.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial pango_font_description* pango_font_face_describe(pango_font_face* face);

    // https://docs.gtk.org/Pango/method.FontFace.get_face_name.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(NativeConstCharMarshaller))]
    [SuppressGCTransition]
    internal static partial string pango_font_face_get_face_name(pango_font_face* face);

    // https://docs.gtk.org/Pango/method.FontFace.is_synthesized.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.U4)]
    [SuppressGCTransition]
    internal static partial bool pango_font_face_is_synthesized(pango_font_face* face);

    // https://docs.gtk.org/Pango/method.FontFace.list_sizes.html
    [LibraryImport(PangoNative.LibPangoName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void pango_font_face_list_sizes(pango_font_face* face, int** sizes, int* n_sizes);
}
