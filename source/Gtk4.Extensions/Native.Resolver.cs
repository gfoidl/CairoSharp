// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;

namespace Gtk4.Extensions;

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
#pragma warning disable CA1416 // Validate platform compatibility
            return libraryName switch
            {
                LibGtkName          => ResolveGtk(),
                LinuxX11.LibX11Name => default,
                Windows.User32Dll   => default,
                _                   => throw new InvalidOperationException($"No resolver for library {libraryName} available")
            };
#pragma warning restore CA1416 // Validate platform compatibility
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
