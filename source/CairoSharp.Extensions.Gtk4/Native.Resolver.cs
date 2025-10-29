// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Gtk4;

static partial class Native
{
    private static readonly Cairo.Native.LibNames s_gtkLibNames = new(
        "libgtk-4.so.1",
        "libgtk-4-1.dll",
        "libgtk-4.1.dylib");

    private static nint s_libGtkHandle;

    static Native()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (libraryName, assembly, searchPath) =>
        {
            return libraryName switch
            {
                LibGtkName => ResolveGtk(),
                _          => throw new InvalidOperationException($"No resolver for library {libraryName} available")
            };
        });
    }

    private static nint ResolveGtk()
    {
        nint libHandle = Volatile.Read(ref s_libGtkHandle);

        if (libHandle == 0)
        {
            libHandle = Cairo.Native.GetLibHandle(s_gtkLibNames);
            Volatile.Write(ref s_libGtkHandle, libHandle);
        }

        return libHandle;
    }
}
