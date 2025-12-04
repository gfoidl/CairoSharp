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

## Fonts

### FreeType

Loading of FreeType fonts either from
* file
* byte array
* stream

### Default Fonts

| Type        | Font        |
|-------------|-------------|
| `SansSerif` | Helvetica   |
| `MonoSpace` | Inconsolata |

Both fonts are also available as bold variant.

## Pango

Support for `PangoLayout` is given, but not the whole Pango API is implemented.

See [ReadMe in Pango](./Pango/ReadMe.md) for further information.

## Loading

PDF, and SVG can be parsed and the drawing loading into a cairo context, for further use.

See [ReadMe in loading](./Loading/ReadMe.md) for further details.

## Pixels

Extension methods for `ImageSurface` to allow easy and fast operation on the pixel data.

## Colors

### Color spaces

The following color spaces are available:
* sRGB (the default `Color` struct as used in _cairo_)
* HSV
* CIE-L\*a\*b\*

There are also methods to convert between these color spaces. See [Colors](./Colors/ReadMe.md) for further information.

### KnownColors

Are based on the color in `System.Drawing` which are equal to the web named colors (and they are equal to the SVG named colors). See [known_colors.svg](../../images/colors/known_colors.svg) for an image that uses all these colors.

### Color maps

See [Color maps](./Colors/ColorMaps/ReadMe.md) for information.
