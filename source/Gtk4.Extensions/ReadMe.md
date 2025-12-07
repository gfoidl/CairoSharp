# Gtk4.Extensions

[![NuGet](https://img.shields.io/nuget/v/gfoidl.Gtk4.Extensions.svg?style=flat-square)](https://www.nuget.org/packages/gfoidl.Gtk4.Extensions/)

## General

Working with .NET and GTK 4 is done via [GirCore](https://github.com/gircore/gir.core).

GirCore has it's own build of _cairo_, but that _cairo_-wrapper
* misses some features in the _cairo_ API-surface
* isn't documented
* misses some convenience methods

Thus these extensions package for GTK 4 uses CairoSharp.

## MSBuild properties

These properties are optional, and by default (`false`) they do nothing.

```xml
<PropertyGroup>
    <GtkWinNativeDir>C:\Program Files\msys64\ucrt64\bin</GtkWinNativeDir>
    <GtkWinCopyNativeLibs>false</GtkWinCopyNativeLibs>
</PropertyGroup>
```

When `GtkWinCopyNativeLibs` is set to `true`, then the native binaries are copied from the directory given by `GtkWinNativeDir` to the output directory.

This is for convenience when you want to package the application.

## Extensions

### ActionExtensions

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

### DrawingAreaExtensions

* setting the [DrawingArea.SetDrawFunc](https://docs.gtk.org/gtk4/method.DrawingArea.set_draw_func.html) for CairoSharp (instead of the GirCore _cairo_ wrapper)
* `SaveAsPng` to save the drawing area as PNG file
* `SaveAsPngWithFileDialog` similar to the previous, just with a file dialog to choose the filename
* `AddContextMenuForFontChooser` adds a context menu ("right-click menu") to allow choosing a (Pango) font

### DropDownExtensions

* `OnNotifySelected` registers a signal the is triggered when the selected properrty is changed
* `SetExpression` set an expression with a `Func<IntPtr, string> propertyAccessor`

### Gtk4Constants

Provides some constants for GTK 4. See [Gtk4Constants](./Gtk4Constants.cs) for the values.

### WindowExtensions

* `HintAlignToParent` tries to dock the windows to a parent on the given side (note: this won't work on Wayland, as there's no such functionality)
* `GetSurface` gets the surface associated with the window
