// (c) gfoidl, all rights reserved

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Cairo.Extensions.GObject;

namespace Cairo.Extensions.Pango;

static partial class PangoNative
{
    private static readonly Native.LibNames s_pangoLibNames = new(
        "libpango-1.0.so.0",            // Linux
        "libpango-1.0-0.dll",           // Windows
        "libpango-1.0.0.dylib");        // MacOS

    private static readonly Native.LibNames s_pangoCairoLibNames = new(
        "libpangocairo-1.0.so.0",       // Linux
        "libpangocairo-1.0-0.dll",      // Windows
        "libpangocairo-1.0.0.dylib");   // MacOS

    private static readonly Native.LibNames s_gobjectLibNames = new(
        GObjectNative.LibGObjectName,   // Linux
        "libgobject-2.0-0.dll",         // Windows
        "libgobject-2.0.0.dylib");      // MacOS

    private static nint s_libPangoHandle;
    private static nint s_libPangoCairoHandle;
    private static nint s_libGObjectHandle;

    [DisallowNull]
    public static DllImportResolver? DllImportResolver { get; set; } = static (libraryName, assembly, searchPath) =>
    {
        switch (libraryName)
        {
            case LibPangoName:
            {
                nint libHandle = Volatile.Read(ref s_libPangoHandle);

                if (libHandle == 0)
                {
                    libHandle = Native.GetLibHandle(s_pangoLibNames);
                    Volatile.Write(ref s_libPangoHandle, libHandle);
                }

                return libHandle;
            }
            case LibPangoCairoName:
            {
                nint libHandle = Volatile.Read(ref s_libPangoCairoHandle);

                if (libHandle == 0)
                {
                    libHandle = Native.GetLibHandle(s_pangoCairoLibNames);
                    Volatile.Write(ref s_libPangoCairoHandle, libHandle);
                }

                return libHandle;
            }
            case GObjectNative.LibGObjectName:
            {
                nint libHandle = Volatile.Read(ref s_libGObjectHandle);

                if (libHandle == 0)
                {
                    libHandle = Native.GetLibHandle(s_gobjectLibNames);
                    Volatile.Write(ref s_libGObjectHandle, libHandle);
                }

                return libHandle;
            }
            default:
                return default;
        }
    };
}
