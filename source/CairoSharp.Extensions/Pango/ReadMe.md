# Pango

## General

See [Rendering with _cairo_](https://docs.gtk.org/PangoCairo/pango_cairo.html).

## Usage

As you have to locate the native libraries, you have to set a [DllImportResolver](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportresolver) for the native libraries, so they know where to find the native libraries.

> [!IMPORTANT]
> The `DllImportResolver` should be set before the first use of the methods provided by CairoSharp.Extensions.

```c#
using System.Reflection;
using System.Runtime.InteropServices;
using Cairo.Extensions.Pango;
using IOPath = System.IO.Path;

if (OperatingSystem.IsWindows())
{
    PangoNative.DllImportResolver = static (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
    {
        string? path = libraryName switch
        {
            PangoNative.LibPangoName      => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libpango-1.0-0.dll"),
            PangoNative.LibPangoCairoName => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libpangocairo-1.0-0.dll"),
            PangoNative.LibGObjectName    => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libgobject-2.0-0.dll"),
            PangoNative.LibGLibName       => IOPath.Combine(@"C:\Program Files\msys64\ucrt64\bin", "libglib-2.0-0.dll"),
            _                             => null
        };

        if (path is not null && NativeLibrary.TryLoad(path, out nint handle))
        {
            return handle;
        }

        return default;
    };
}
```

> [!TIP]
> The `PangoNative.LibXYZ` names are the standard so-names on Linux, thus for a standard installation no `DllImportResolver` has to be set on Linux.

> [!NOTE]
> When the Pango libraries are on `$PATH` you don't have to set the `PangoNative.DllImportResolver`.

## MSBuild properties

These properties are optional, and by default (`false`) they do nothing.

```xml
<PropertyGroup>
    <PangoWinNativeDir>C:\Program Files\msys64\ucrt64\bin</PangoWinNativeDir>
    <PangoWinCopyNativeLibs>true</PangoWinCopyNativeLibs>
</PropertyGroup>
```

When `PangoWinCopyNativeLibs` is set to `true`, then the native binaries are copied from the directory given by `PangoWinNativeDir` to the output directory.

This is for convenience when you want to package the application.
