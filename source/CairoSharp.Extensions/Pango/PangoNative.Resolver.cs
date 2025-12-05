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
    //-------------------------------------------------------------------------
    [DisallowNull]
    public static DllImportResolver? DllImportResolver { get; set; } = static (libraryName, assembly, searchPath) =>
    {
        return libraryName switch
        {
            LibPangoName                 => ResolveCore(ref s_libPangoHandle     , s_pangoLibNames),
            LibPangoCairoName            => ResolveCore(ref s_libPangoCairoHandle, s_pangoCairoLibNames),
            GObjectNative.LibGObjectName => ResolveCore(ref s_libGObjectHandle   , s_gobjectLibNames),
            _                            => default
        };
    };
    //-------------------------------------------------------------------------
    private static nint ResolveCore(ref nint handle, Native.LibNames libNames)
    {
        nint libHandle = Volatile.Read(ref handle);

        if (libHandle == 0)
        {
            libHandle = Native.GetLibHandle(libNames);
            Volatile.Write(ref handle, libHandle);
        }

        return libHandle;
    }
}
