This is a fork of https://github.com/mono/gtk-sharp and is maintained completely separately from that project.



Tutorial:
https://zetcode.com/gfx/cairo/



[![NuGet](https://img.shields.io/nuget/v/gfoidl.CairoSharp.svg?style=flat-square)](https://www.nuget.org/packages/gfoidl.CairoSharp/)

## CairoSharp

CairoSharp is a .NET 8+ wrapper for [cairo](https://www.cairographics.org/).

Cairo is a 2D graphics library with support for multiple output devices.

Cairo is designed to produce consistent output on all output media while taking advantage of display hardware acceleration when available.
The cairo API provides operations similar to the drawing operators of PostScript and PDF.
Operations in cairo including stroking and filling cubic Bézier splines, transforming and compositing translucent images, and antialiased text rendering.
All drawing operations can be transformed by any affine transformation (scale, rotation, shear, etc.).

## Platforms

[A former release](https://github.com/zwcloud/CairoSharp/releases/tag/dotnet45_v1) works on .NET Framework (Windows desktop, and Winform / WPF)
on .NET 4.5 and Linux (tested on Ubuntu 12.04) on Mono 4.2.
CairoSharp was ported to .NET Core (.NET Standard 1.6) after that release, and later on (in this repo) to move on it only supports .NET 8+.

Supported platforms:

* Windows 32bit / 64bit
* Linux 64bit (tested on Ubuntu 16.04-server)
* Linux Arm

Other platforms that support .NET may work, but have not been tested.

On Windows all dependencies are included.

On Linux _libcairo_ is a prerequisite. See https://www.cairographics.org/download/ on how to install.
Cairo version 1.17.2 is minimum.

# [Documentation](https://github.com/gfoidl/CairoSharp/wiki)

# Copying/License

**LGPLv3** Project CairoSharp (not the native cairo lib but the C# one) is licensed under the LGPL v3 license.

```
CairoSharp, A C# wrapper of cairo which is a 2D vector rendering library
Copyright (C) 2017-2025 gfoidl
Copyright (C) 2015-2020 Zou Wei, zwcloud@hotmail.com, https://zwcloud.net
Copyright (C) 2007-2015 Xamarin, Inc.
Copyright (C) 2006 Alp Toker
Copyright (C) 2005 John Luke
Copyright (C) 2004 Novell, Inc (http://www.novell.com)
Copyright (C) Ximian, Inc. 2003

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
```

The C# code files in CairoSharp project was taken from Mono / [GTK#](https://github.com/mono/gtk-sharp/tree/master/cairo)(Version 3.0.0), licensed under the GNU LGPL.
So CairoSharp uses LGPL as well.

I, @gfoidl, forked the repo from @zwcloud, made a lot of changes, which got then merged upstream by @zwcloud. Now for several years the upstream repo
from @zwcloud seems to be stale, so I made again a lot of changes in this fork here. At the time of this writing there's only very little in common with
the upstream repo -- it's a complete rewrite for modern .NET.

## Native libraries

https://gitlab.com/saiwp/cairo.git

https://mesonbuild.com/SimpleStart.html


From cairo repo https://gitlab.com/saiwp/cairo/-/blob/master/INSTALL:
x64:
```cmd
rem Run in "x64 Native Tools Command Prompt for VS 2022"

cd native\cairo

meson setup --buildtype=release --default-library=both --default-both-libraries=shared -Dtests=disabled ..\..\artifacts\native\win-x64\build
ninja -C ..\..\artifacts\native\win-x64\build

rem build will likely not complete, but the desired artifact is there...
rem meson install -C ..\..\artifacts\native\win-x64\build --destdir ..\out
```
x86:
```cmd
rem Run in "x86 Native Tools Command Prompt for VS 2022"

cd native\cairo

meson setup --buildtype=release --default-library=both --default-both-libraries=shared -Dtests=disabled ..\..\artifacts\native\win-x86\build
ninja -C ..\..\artifacts\native\win-x86\build

rem build will likely not complete, but the desired artifact is there...
rem meson install -C ..\..\artifacts\native\win-x86\build --destdir ..\out
```


The native project files for Windows are taken from https://github.com/preshing/cairo-windows, currently using cairo version 1.17.2.

* [cairo](http://www.cairographics.org/)
  Version 1.17.2
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/cairo/COPYING)

* [libpng](http://libmng.com/pub/png/libpng.html)
  Version from cairo-project dependency
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/libpng/LICENSE)

* [pixman](http://www.pixman.org/)
  Version from cairo-project dependency
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/pixman/COPYING)

* [zlib](http://www.zlib.net/)
  Version from cairo-project dependency
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/zlib/README)

* [freetype](http://www.freetype.org/)
  Version from cairo-project dependency
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/freetype/docs/LICENSE.TXT)
