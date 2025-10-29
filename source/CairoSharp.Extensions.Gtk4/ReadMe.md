# CairoSharp.Extensions.Gtk4

[![NuGet](https://img.shields.io/nuget/v/gfoidl.CairoSharp.Extensions.Gtk4.svg?style=flat-square)](https://www.nuget.org/packages/CairoSharp.Extensions.Gtk4/)

Working with .NET and GTK 4 is done via [GirCore](https://github.com/gircore/gir.core).

GirCore has it's own build of _cairo_, but that _cairo_-wrapper
* misses some features in the _cairo_ API-surface
* isn't documented
* misses some convenience methods

Thus these extensions package for GTK 4 uses CairoSharp.
