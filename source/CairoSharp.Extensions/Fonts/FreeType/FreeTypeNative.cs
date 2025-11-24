// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Cairo.Extensions.Fonts.FreeType;

internal static unsafe partial class FreeTypeNative
{
    public const string LibFreeType = "freetype";

    // Can't use StringMarshalling here, as that would try to free the native
    // return result, which in case here is a const char* pointing to a internal field,
    // so can't be freed at that time.
    [LibraryImport(LibFreeType)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalUsing(typeof(StaticNativeStringMarshaller))]
    internal static partial string? FT_Error_String(FTError error_code);

    [LibraryImport(LibFreeType)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FTError FT_Init_FreeType(FT_Library* alibrary);

    [LibraryImport(LibFreeType)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FTError FT_Done_FreeType(FT_Library library);

    [LibraryImport(LibFreeType)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void FT_Library_Version(FT_Library library, out int amajor, out int aminor, out int apatch);

    [LibraryImport(LibFreeType, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FTError FT_New_Face(FT_Library library, string filepathname, FT_Long face_index, out FT_Face aface);

    [LibraryImport(LibFreeType)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FTError FT_New_Memory_Face(FT_Library library, byte* file_base, FT_Long file_size, FT_Long face_index, out FT_Face aface);

    [LibraryImport(LibFreeType)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial FTError FT_Done_Face(FT_Face face);
}
