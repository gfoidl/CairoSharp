// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;

namespace Cairo;

internal static class NativeMethods
{
    public const string LibCairo = "cairo";

#if NET5_0_OR_GREATER && DEBUG      // only needed for local DEV, via NuGet / .NET SDK package reference the deps are setup correctly
    private static nint s_libHandle;

    static NativeMethods()
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
        string path = Path.Combine(AppContext.BaseDirectory, "runtimes", GetRuntimeIdentifier(), GetLibraryName());

        if (NativeLibrary.TryLoad(path, out nint libHandle))
        {
            Volatile.Write(ref s_libHandle, libHandle);
            return libHandle;
        }

        return 0;

        static string GetLibraryName() => OperatingSystem.IsLinux() ? $"lib{LibCairo}.so" : LibCairo;

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

#if !NET5_0_OR_GREATER
    static NativeMethods()
    {
        string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
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
}
