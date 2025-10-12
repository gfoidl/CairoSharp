// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IOPath = System.IO.Path;

namespace Cairo;

internal static class Native
{
    public const string LibCairo = "cairo";

#pragma warning disable CA2255      // The 'ModuleInitializer' attribute should not be used in libraries
    private static nint s_libHandle;

    // The module initializer is needed as every "feature" has it's own native class.
    // Therefore w/o this initializer the DllImportResolver would not be set.
    [ModuleInitializer]
    public static void ModuleInitializer()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (libraryName, assembly, searchPath) =>
        {
            if (libraryName == LibCairo)
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
        // E.g. with the stub we have:
        // $ ldd libcairo.so
        // Then the dependencies from the OS loader's PoV are listed.
        const string WinLibName = "cairo-2.dll";

        string path = GetLocalLibraryPathWithRid(WinLibName);

        if (NativeLibrary.TryLoad(path, out nint handle))
        {
            return handle;
        }

        return default;
    }

    private static nint GetLibHandleLinux()
    {
        const string LinuxLibName = "libcairo.so.2";
        const string StubLibName  = "libcairo.so";

        // Try the native loader first
        if (NativeLibrary.TryLoad(LinuxLibName, out nint handle))
        {
            return handle;
        }

        // When not found, try via stub
        string path = GetLocalLibraryPathWithRid(StubLibName);

        if (NativeLibrary.TryLoad(path, out handle))
        {
            return handle;
        }

        return default;
    }

    private static nint GetLibHandleMacOS()
    {
        const string MacOSLibName = "libcairo.2.dylib";

        if (NativeLibrary.TryLoad(MacOSLibName, out nint handle))
        {
            return handle;
        }

        return default;
    }

    internal static string GetLocalLibraryPathWithRid(string libName)
    {
        string runtimeIdentifier = GetRuntimeIdentifier();

        return IOPath.Combine(
            AppContext.BaseDirectory,
            "runtimes",
            runtimeIdentifier,
            "native",
            libName);
    }

    private static string GetRuntimeIdentifier()
    {
        // RuntimeInformation.RuntimeIdentifier returns e.g. 'win10-x64', but we just
        // need win-x64, etc.

        if (OperatingSystem.IsWindows())
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "win-x64",
                Architecture.X86 => "win-x86",
                _                => throw new PlatformNotSupportedException()
            };
        }
        else if (OperatingSystem.IsLinux())
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64   => "linux-x64",
                Architecture.X86   => "linux-x86",
                Architecture.Arm   => "linux-arm",
                Architecture.Arm64 => "linux-arm64",
                _                  => throw new PlatformNotSupportedException()
            };
        }
        else if (OperatingSystem.IsMacOS())
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64   => "osx-x64",
                Architecture.Arm64 => "osx-arm64",
                _                  => throw new PlatformNotSupportedException()
            };
        }

        throw new PlatformNotSupportedException();
    }

#if !NET5_0_OR_GREATER && DEBUG
    [ModuleInitializer]
    public static void ModuleInitializer()
    {
        string path   = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        string dllDir = System.IO.Path.GetDirectoryName(typeof(NativeMethods).Assembly.Location);

        if (Environment.Is64BitProcess)
        {
            dllDir = System.IO.Path.Combine(dllDir, "runtimes", "win-x64", "native");
        }
        else
        {
            dllDir = System.IO.Path.Combine(dllDir, "runtimes", "win-x86", "native");
        }

        path += ";" + dllDir;

        Environment.SetEnvironmentVariable("PATH", path);
    }
#endif
#pragma warning restore CA2255      // The 'ModuleInitializer' attribute should not be used in libraries
}
