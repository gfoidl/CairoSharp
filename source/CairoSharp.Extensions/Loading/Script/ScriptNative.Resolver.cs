// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Loading.Script;

partial class ScriptNative
{
    private static readonly Native.LibNames s_libNames = new(
        "libcairo-script-interpreter.so.2",
        "cairo-script-interpreter-2.dll",
        "libcairo-script-interpreter.2.dylib");

    private static nint s_libHandle;

    static ScriptNative()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (libraryName, assembly, searchPath) =>
        {
            if (libraryName == LibCairoScriptInterpreter)
            {
                nint libHandle = Volatile.Read(ref s_libHandle);

                if (libHandle == 0)
                {
                    libHandle = Native.GetLibHandle(s_libNames);
                    Volatile.Write(ref s_libHandle, libHandle);
                }

                return libHandle;
            }

            return default;
        });
    }
}
