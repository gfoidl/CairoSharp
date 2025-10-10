# Native Build

## Linux

For Linux _cairo_ is a prerequisite and must be [installed](https://www.cairographics.org/download/), e.g. for Debian-like:
```bash
sudo apt install libcairo2-dev
```

We use [stubs](./stubs/ReadMe.md), which make handling of the versioned _cairo_ library easier.

## Windows

As no current _cairo_ DLLs are available, the _cairo_ source is included as git-submodule, and can be built via _cairo_'s meson build system.

### x64

```cmd
rem Run in "x64 Native Tools Command Prompt for VS 2022"

cd native\cairo

meson setup --buildtype=release --default-library=both --default-both-libraries=shared -Dtests=disabled ..\..\artifacts\native\win-x64\build
ninja -C ..\..\artifacts\native\win-x64\build

rem build will likely not complete, but the desired artifact is there...
rem meson install -C ..\..\artifacts\native\win-x64\build --destdir ..\out
```

### x86

```cmd
rem Run in "x86 Native Tools Command Prompt for VS 2022"

cd native\cairo

meson setup --buildtype=release --default-library=both --default-both-libraries=shared -Dtests=disabled ..\..\artifacts\native\win-x86\build
ninja -C ..\..\artifacts\native\win-x86\build

rem build will likely not complete, but the desired artifact is there...
rem meson install -C ..\..\artifacts\native\win-x86\build --destdir ..\out
```

## Mac OS X

_cairo_ must be [installed](https://www.cairographics.org/download/), and as I don't have any experience with Macs I don't know if stubs are needed or not.
If you know more, please file and issue or create a PR, that adds these stubs, etc.
