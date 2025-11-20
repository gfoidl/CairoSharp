// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Loading.Script;

static partial class ScriptNative
{
    private static readonly Native.LibNames s_libNames = new(
        "libcairo-script-interpreter.so.2",
        "cairo-script-interpreter-2.dll",
        "libcairo-script-interpreter.2.dylib")
    {
        WindowsAltLibName = "libcairo-script-interpreter-2.dll"
    };

    private static nint s_libHandle;

    internal static DllImportResolver Resolver { get; } = static (libraryName, assembly, searchPath) =>
    {
        Debug.Assert(libraryName == LibCairoScriptInterpreter);

        nint libHandle = Volatile.Read(ref s_libHandle);

        if (libHandle == 0)
        {
            libHandle = Native.GetLibHandle(s_libNames);
            Volatile.Write(ref s_libHandle, libHandle);
        }

        return libHandle;
    };
}
