// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Fonts.FreeType;

static partial class FreeTypeNative
{
    private static readonly Native.LibNames s_freeTypeLibNames = new(
        "libfreetype.so.6",         // Linux
        "freetype-6.dll",           // Windows
        "libfreetype.6.dylib")      // MacOS
    {
        WindowsAltLibName = "libfreetype-6.dll"
    };

    private static nint s_libHandle;

    internal static DllImportResolver Resolver { get; } = static (libraryName, assembly, searchPath) =>
    {
        Debug.Assert(libraryName == LibFreeType);

        nint libHandle = Volatile.Read(ref s_libHandle);

        if (libHandle == 0)
        {
            libHandle = Native.GetLibHandle(s_freeTypeLibNames);
            Volatile.Write(ref s_libHandle, libHandle);
        }

        return libHandle;
    };
}
