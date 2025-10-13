# Native Build

## cairo

### Linux

For Linux _cairo_ is a prerequisite and must be [installed](https://www.cairographics.org/download/), e.g. for Debian-like:
```bash
sudo apt install libcairo2-bin

# or to install the dev-package (the bin package is sufficient usually)
sudo apt install libcairo2-dev
```

### Windows

As no current _cairo_ DLLs are available, the _cairo_ source is included as git-submodule, and can be built via _cairo_'s meson build system.

Options for the build are listed in [meson.options](https://gitlab.freedesktop.org/cairo/cairo/-/blob/master/meson.options?ref_type=heads).

#### x64

For a build that outputs a single `cairo-2.dll` use:
```cmd
rem Run in "x64 Native Tools Command Prompt for VS 2022"

cd native\cairo

meson setup --buildtype=release --default-library=both --default-both-libraries=shared -Dtests=disabled ..\..\artifacts\native\win-x64\cairo\build
ninja -C ..\..\artifacts\native\win-x64\cairo\build

rem build will likely not complete, but the desired artifact is there...
rem meson install -C ..\..\artifacts\native\win-x64\cairo\build --destdir ..\out
```

> [!NOTE]
> Freetype isn't available in this build.

Thus when Freetype should be available, then a build like the following has to be done.
This output several native DLLs, so all of them have to be included in the NuGet-package.
```cmd
rem Run in "x64 Native Tools Command Prompt for VS 2022"

cd native\cairo

meson setup --buildtype=release --default-library=shared -Dfontconfig=enabled -Dfreetype=enabled -Dtests=disabled ..\..\artifacts\native\win-x64\cairo\build
ninja -C ..\..\artifacts\native\win-x64\cairo\build
meson install -C ..\..\artifacts\native\win-x64\cairo\build --destdir ..\out
```

#### x86

```cmd
rem Run in "x86 Native Tools Command Prompt for VS 2022"

cd native\cairo

meson setup --buildtype=release --default-library=shared -Dfontconfig=enabled -Dfreetype=enabled -Dtests=disabled ..\..\artifacts\native\win-x86\cairo\build
ninja -C ..\..\artifacts\native\win-x86\cairo\build
meson install -C ..\..\artifacts\native\win-x86\cairo\build --destdir ..\out
```

### Mac OS X

_cairo_ must be [installed](https://www.cairographics.org/download/), and as I don't have any experience with Macs I don't know if stubs are needed or not.
If you know more, please file and issue or create a PR, that adds these stubs, etc.
