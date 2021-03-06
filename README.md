[![NuGet](https://img.shields.io/nuget/v/CairoSharp.svg?style=flat-square)](https://www.nuget.org/packages/CairoSharp)

# CairoSharp
A C# wrapper (.net standard 1.6, and .net 4.5) of [cairo](https://www.cairographics.org/). All its dependcies included.

Cairo is a 2D graphics library with support for multiple output devices.

Cairo is designed to produce consistent output on all output media while taking advantage of display hardware acceleration when available.
The cairo API provides operations similar to the drawing operators of PostScript and PDF.
Operations in cairo including stroking and filling cubic B�zier splines, transforming and compositing translucent images, and antialiased text rendering.
All drawing operations can be transformed by any affine transformation (scale, rotation, shear, etc.).

# Platforms

[A former release](https://github.com/zwcloud/CairoSharp/releases/tag/dotnet_4.5) works on Windows desktop (Winform/WPF/Console Application) on .NET 4.5 and linux (tested on Ubuntu 12.04) on mono 4.2. CairoSharp was ported to .NET Core (.NET Standard 1.6) after that release.

Supported platforms:  

* windows 32bit / 64bit
* linux 64bit (tested on Ubuntu 16.04-server)

Other platforms that support .net core may work, but have not been tested.

On Linux _libcairo_ is a prerequisite. `sudo apt-get update && sudo apt-get install -y libcairo2`

# [Documentation](https://github.com/zwcloud/CairoSharp/wiki)

# Copying/License
__LGPLv3__  
Project CairoSharp (not the native cairo lib but the C# one) is licensed under the LGPL v3 license.

    CairoSharp, A C# wrapper of cairo which is a 2D vector rendering library
    Copyright (C) 2017 gfoidl
    Copyright (C) 2015-2017  Zou Wei, zwcloud@hotmail.com, https://zwcloud.net
    Copyright (C) 2007-2015  Xamarin, Inc.
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

The C# code files in CairoSharp project was taken from Mono/[GTK#](https://github.com/mono/gtk-sharp/tree/master/cairo)(Version 3.0.0), licensed under the GNU LGPL. So CairoSharp uses LGPL as well.


## Native libraries

The [Native project files](https://github.com/zwcloud/CairoSharp/tree/master/Native/projects) is generated according to a VS2015 Solution from [Cairo-VS](https://github.com/DomAmato/Cairo-VS).

* [cairo](http://www.cairographics.org/)
  Version 1.15.6
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/cairo/COPYING)

* [libpng](http://libmng.com/pub/png/libpng.html)
  Version 1.6.18
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/libpng/LICENSE)

* [pixman](http://www.pixman.org/) 
  Version 0.32.6
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/pixman/COPYING)

* [zlib](http://www.zlib.net/)
  Version 1.2.8
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/zlib/README)

* [freetype](http://www.freetype.org/)
  Version 2.6
  [COPYING Info](https://github.com/zwcloud/CairoSharp/blob/master/Native/freetype/docs/LICENSE.TXT)

These libraries will be updated if new verisons are released.<br/>
Last update date of these Native libraries: 2017-07-16
