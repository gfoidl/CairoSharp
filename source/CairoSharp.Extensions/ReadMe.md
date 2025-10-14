# CairoSharp.Extensions

[![NuGet](https://img.shields.io/nuget/v/gfoidl.CairoSharp.Extensions.svg?style=flat-square)](https://www.nuget.org/packages/gfoidl.CairoSharp.Extensions/)

Provides some common extensions like drawing shapes, and other helpful[^1] stuff for [CairoSharp](https://github.com/gfoidl/CairoSharp).

[^1]: at least for me :wink:

## Shapes

* Shape (abstract)
* Circle
* Square
* Hexagon

## Arrows

* Arrow
* Vector (arrow head on one side only)
* circle arrow head
* open arrow head

## KnownColors

Are based on the color in `System.Drawing`. See [known_colors.svg](../../known_colors.svg) for an image that uses all these colors.

## Fonts

### FreeType

Loading of FreeType fonts either from
* file
* byte array
* stream

## Loading

PDF, and SVG can be parsed and the drawing loading into a cairo context, for further use.

See [ReadMe in loading](./Loading/ReadMe.md) for further details.
