// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Fonts.FreeType;

partial class FreeTypeNative
{
    private static readonly Native.LibNames s_freeTypeLibNames = new("libfreetype.so.6", "freetype-6.dll", "libfreetype.6.dylib");

    private static nint s_libHandle;

    static FreeTypeNative()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (libraryName, assembly, searchPath) =>
        {
            if (libraryName == LibFreeType)
            {
                nint libHandle = Volatile.Read(ref s_libHandle);

                if (libHandle == 0)
                {
                    libHandle = Native.GetLibHandle(s_freeTypeLibNames);
                    Volatile.Write(ref s_libHandle, libHandle);
                }

                return libHandle;
            }

            return default;
        });
    }
}
