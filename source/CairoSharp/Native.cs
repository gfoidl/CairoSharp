// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IOPath = System.IO.Path;

namespace Cairo;

internal static class Native
{
    public const string LibCairo = "cairo";

    // The logic in this loader will be used in the extensions project too.
    public record LibNames(string LinuxLibName, string WindowsLibName, string MacOSLibName)
    {
        public string? LinuxStubName { get; init; }
    }

    // E.g. with the stub we have:
    // $ ldd libcairo.so
    // Then the dependencies from the OS loader's PoV are listed.
    private static readonly LibNames s_cairoLibNames = new("libcairo.so.2", "cairo-2.dll", "libcairo.2.dylib")
    {
        LinuxStubName = "libcairo.so"
    };

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

                if (libHandle == 0)
                {
                    libHandle = GetLibHandle(s_cairoLibNames);
                    Volatile.Write(ref s_libHandle, libHandle);
                }

                return libHandle;
            }

            return default;
        });
    }

    public static nint GetLibHandle(LibNames libNames)
    {
        if (OperatingSystem.IsWindows())
        {
            return GetLibHandleWin(libNames);
        }
        else if (OperatingSystem.IsLinux())
        {
            return GetLibHandleLinux(libNames);
        }
        else if (OperatingSystem.IsMacOS())
        {
            return GetLibHandleMacOS(libNames);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    private static nint GetLibHandleWin(LibNames libNames)
    {
        // When consuming project is built with an RFID set, then the SDK
        // flattens the native libraries.
        if (NativeLibrary.TryLoad(libNames.WindowsLibName, out nint handle))
        {
            return handle;
        }

        string path = GetLocalLibraryPathWithRid(libNames.WindowsLibName);

        if (NativeLibrary.TryLoad(path, out handle))
        {
            return handle;
        }

        return default;
    }

    private static nint GetLibHandleLinux(LibNames libNames)
    {
        // Try the native loader first
        if (NativeLibrary.TryLoad(libNames.LinuxLibName, out nint handle))
        {
            return handle;
        }

        // When not found, try via stub
        if (libNames.LinuxStubName is not null)
        {
            // When consuming project is built with an RFID set, then the SDK
            // flattens the native libraries.
            if (NativeLibrary.TryLoad(libNames.LinuxStubName, out handle))
            {
                return handle;
            }

            string path = GetLocalLibraryPathWithRid(libNames.LinuxStubName);

            if (NativeLibrary.TryLoad(path, out handle))
            {
                return handle;
            }
        }

        return default;
    }

    private static nint GetLibHandleMacOS(LibNames libNames)
    {
        if (NativeLibrary.TryLoad(libNames.MacOSLibName, out nint handle))
        {
            return handle;
        }

        return default;
    }

    private static string GetLocalLibraryPathWithRid(string libName)
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
