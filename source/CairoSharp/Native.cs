// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cairo;

internal static class Native
{
    public const string LibCairo = "cairo";

#pragma warning disable CA2255      // The 'ModuleInitializer' attribute should not be used in libraries
#if NET5_0_OR_GREATER && DEBUG      // only needed for local DEV, via NuGet / .NET SDK package reference the deps are setup correctly
    private static nint s_libHandle;

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
        string path = System.IO.Path.Combine(
            AppContext.BaseDirectory,
            "runtimes",
            GetRuntimeIdentifier(),
            "native",
            GetLibraryName());

        if (NativeLibrary.TryLoad(path, out nint libHandle))
        {
            Volatile.Write(ref s_libHandle, libHandle);
            return libHandle;
        }

        return 0;

        // Not strictly necessary, as the runtime will do this for us otherwise.
        static string GetLibraryName() => OperatingSystem.IsLinux() ? $"lib{LibCairo}.so" : $"{LibCairo}.dll";

        static string GetRuntimeIdentifier()
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

            throw new PlatformNotSupportedException();
        }
    }
#endif
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
