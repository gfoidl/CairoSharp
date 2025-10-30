# Gtk4.Extensions

[![NuGet](https://img.shields.io/nuget/v/gfoidl.CairoSharp.Extensions.Gtk4.svg?style=flat-square)](https://www.nuget.org/packages/CairoSharp.Extensions.Gtk4/)

## General

Working with .NET and GTK 4 is done via [GirCore](https://github.com/gircore/gir.core).

GirCore has it's own build of _cairo_, but that _cairo_-wrapper
* misses some features in the _cairo_ API-surface
* isn't documented
* misses some convenience methods

Thus these extensions package for GTK 4 uses CairoSharp.

## Extensions

### `ActionExtensions`

Help with registering [actions](https://docs.gtk.org/gtk4/actions.html) in the form of
```c#
public sealed class MainWindow : ApplicationWindow
{
    public MainWindow(/* ... */)
    {
        this.AddAction("saveAsPng", this.SaveAsPng);
    }

    private void SaveAsPng()
    {
        // ...
    }
}
```

### `DrawingAreaExtensions`

Allows setting [DrawingArea.SetDrawFunc](https://docs.gtk.org/gtk4/method.DrawingArea.set_draw_func.html) for CairoSharp (instead of the GirCore _cairo_ wrapper).
