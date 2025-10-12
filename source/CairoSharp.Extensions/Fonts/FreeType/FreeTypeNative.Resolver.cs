// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Fonts.FreeType;

partial class FreeTypeNative
{
    private static nint s_libHandle;

    static FreeTypeNative()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (libraryName, assembly, searchPath) =>
        {
            if (libraryName == LibFreeType)
            {
                nint libHandle = Volatile.Read(ref s_libHandle);

                return libHandle != 0 ? libHandle : GetLibHandle();
            }

            return default;
        });
    }

    private static nint GetLibHandle()
    {
        nint libHandle;

        if (OperatingSystem.IsWindows())
        {
            libHandle = GetLibHandleWin();
        }
        else if (OperatingSystem.IsLinux())
        {
            libHandle = GetLibHandleLinux();
        }
        else if (OperatingSystem.IsMacOS())
        {
            libHandle = GetLibHandleMacOS();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        if (libHandle != 0)
        {
            Volatile.Write(ref s_libHandle, libHandle);
        }

        return libHandle;
    }

    private static nint GetLibHandleWin()
    {
        const string WinLibName = "freetype-6.dll";

        string path = Native.GetLocalLibraryPathWithRid(WinLibName);

        if (NativeLibrary.TryLoad(path, out nint handle))
        {
            return handle;
        }

        return default;
    }

    private static nint GetLibHandleLinux()
    {
        const string LinuxLibName = "libfreetype.so.6";

        if (NativeLibrary.TryLoad(LinuxLibName, out nint handle))
        {
            return handle;
        }

        return default;
    }

    private static nint GetLibHandleMacOS()
    {
        const string MacOSLibName = "libfreetype.6.dylib";

        if (NativeLibrary.TryLoad(MacOSLibName, out nint handle))
        {
            return handle;
        }

        return default;
    }
}
