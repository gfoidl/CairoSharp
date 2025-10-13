# Loading of drawings into _cairo_

## Why would one do this?

* load SVG icons to be used inside your own drawing
* load SVG or PDF and convert it to PNG without the need for separate services
* ...

## Used native libraries

There exist a couple of libraries that can load drawings into cairo, most notably

| Type | Library                                                          | Description                                                         |
|------|------------------------------------------------------------------|---------------------------------------------------------------------|
| SVG  | [librsvg](https://wiki.gnome.org/Projects/LibRsvg)               | A library to render SVG images to Cairo surfaces.                   |
| PDF  | [poppler](https://poppler.freedesktop.org/)                      | Poppler is a PDF rendering library based on the xpdf-3.0 code base. |
| PS*  | [spectre](https://www.freedesktop.org/wiki/Software/libspectre/) | libspectre is a small library for rendering Postscript documents    |

\* not implemented / supported yet

> [!NOTE]
> CairoSharp.Extensions does not aim to be full .NET wrapper around these libraries, rather only the minimum needed functionality is wrapped and used internally.
> Therefore CairoSharp.Extensions won't expose any of these APIs publicly.

## Design decision and `DllImportResolver`

CairoSharp is a .NET wrapper around _cairo_, so when the package is installed it should just work[^1]. Thus for Windows the native dependencies are bundled into the package, as otherrwise it's painful to find recent ones.

[^1]: on non-Windows it's easy to install the needed dependencies, see [platforms section in repo root](https://github.com/gfoidl/CairoSharp?tab=readme-ov-file#supported-platforms)

For the CairoSharp.Extensions and in special regards to `librsvg` and `poppler` it's a bit different, as these extensions are not the core functionality that this project aims for (drawing with _cairo_), so no native dependencies are bundled in this package.

* on Linux the dependencies are easy to install (when not already there)
* the package size would grow as e.g. librsvg includes / has a dependency to pango, gobject, and others
  * as these are only needed for Windows, it's unnecessary bloat for Linux users[^2]
  * to keep the CairoSharp.Extensions package small, a possible solution could be to have another project like CairoSharp.Extensions.SVG, where these dependencies reside
    * also bloat for Linux users
    * for PDF there should be another similar named package
    * the name should rather be CairoSharp.Extensions.Loading.SVG
    * having another package for a rather niche use case...just no
* building e.g. librsvg with all it's dependencies on Windows isn't trivial, especially when an already built _cairo_ should be re-used

[^2]: that also applies to CairoSharp itself, but there's it's core functionality, thus that trade-off was made

So instead of bundling the native dependencies you have to locate your already installed native libraries.

## Installation of native dependencies

### Linux / Ubuntu

```bash
sudo apt update

# SVG
sudo apt install librsvg2-bin

# PDF
sudo apt install libpoppler-glib-dev
```

Note: poppler is for C++, thus the `libpoppler-glib8` is needed which provides C-bindings.

### Windows

#### SVG

I don't know if there any standalone librsvg build / download for Windows exists. So please use your favorite search engine to search for it.

Another possibility is to re-use the libraries from an already installed application, that uses _cairo_, and librsvg.

> [!CAUTION]
> Always check the license before it this is allowed. You can't blame me for any misdemeanor.

One such application is the great [Inkscape](https://inkscape.org).

#### PDF

Can be downloaded e.g. from  Owen Schwartz 's [poppler-windows](https://github.com/oschwartz10612/poppler-windows).

## Usage

As you have to locate the native libraries, you have to set a [DllImportResolver](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportresolver) for the native libraries, so they know where to find the native libraries.

> [!IMPORTANT]
> The `DllImportResolver` should be set before the first use of the methods provided by CairoSharp.Extensions

E.g. for SVG loading with librsvg:
```c#
using System.Reflection;
using System.Runtime.InteropServices;
using Cairo.Extensions.Loading.SVG;
using IOPath = System.IO.Path;

if (OperatingSystem.IsWindows())
{
    LoadingNative.DllImportResolver = static (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
    {
        string? path = libraryName switch
        {
            LoadingNative.LibRSvgName    => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "librsvg-2-2.dll"),
            LoadingNative.LibPopplerName => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libpoppler-glib-8.dll"),
            LoadingNative.LigGObjectName => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgio-2.0-0.dll"),
            LoadingNative.LibGioName     => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgobject-2.0-0.dll"),
            _                            => null
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
> The `LoadingNative.LibXYZ` names are the standard so-names on Linux, thus for a standard installation no `DllImportResolver` has to be set on Linux

In the above example the native DLLs from an Inkscape installation are used, so the resolver points to that place.

When either librsvg or poppler isn't needed, then no resolver for these names has to be given.
The resolvers for `LigGObjectName` and `LibGioName` must be given anyway (once loading features are used).
