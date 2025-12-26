## CairoSharp Demos

### Preface

The "official" [cairo samples](https://www.cairographics.org/samples/) were ported to C# in the [older repository](https://github.com/zwcloud/CairoSharp), then got updated to modern .NET and enhanced by me.

### CairoDemo

Shows various examples, and demos ono how to use _cairo_.

There are also some enhanced features, like setting tags, and links in a PDF, creating user fonts, and that like.

`CairoDemo.NuGet` is the same, but not with a `<ProjectReference>` rather with a `<PackageReference>` (to test the NuGet-package).

### AspNetCoreDemo

This demo has a simple minimal api endpoint, that uses _cairo_ to render a SVG and write it to the output without the need of temporary files. This is done by either using the streaming API or registering a callback at surface construction.

See [project's ReadMe](./AspNetCoreDemo/ReadMe.md) for further info.

### CairoExtensionsDemo

Shows some features that the _CairoSharp.Extensions_ package provides.

### FreeTypeDemo

Uses FreeType fonts either loaded from file, a byte array or from an embedded resource.

### PangoDemo

Shows some features that [Pango](https://www.gtk.org/docs/architecture/pango) offers for _cairo_.

### SvgPdfLoading

Loads SVG or PDF files into a _cairo_ context, so they can be used in further drawing.

### GTK demos

#### Gtk3Demo

Shows how to draw with _cairo_ in GTK 3 in a simple way and without any external dependency (like [GtkSharp](https://github.com/GtkSharp/GtkSharp)).

#### Gtk4Demo

Shows the [cairo examples](https://www.cairographics.org/samples/) in GTK 4, plus some little extras added by me.

For more information see [Gtk4Demo ReadMe](./GTK/Gtk4Demo/ReadMe.md).

A Windows version is in [WinFormsDemo](#winformsdemo).

#### Gtk4Animation

Shows how to display an animation, drawed via _cairo_ in GTK 4. For each iteration an image can be stored to the output folder, and in the [ReadMe](./Gtk/Gtk4Animation/ReadMe.md) over there is a command on how to create a `mp4`-video out of the created image files.

There are some variants of this demo:
* [Gtk4AnimationWithBuilder](./GTK/Gtk4AnimationWithBuilder/ReadMe.md) shows how to use GTK 4's UI builder
* [Gtk4AnimationOptimized](./GTK/Gtk4AnimationOptimized/ReadMe.md) uses optimizations for more efficient rendering

A Windows version is in [WinFormsAnimation](#winformsanimation).

#### Gtk4FunctionPlotDemo

Plotting of pixel data (peaks function) and displaying current $z = f(x, y)$ values on mouse move.

Also shows multi-line text layout for charts.

See [ReadMe of project](./GTK/Gtk4FunctionPlotDemo/ReadMe.md) for more infos.

### Windows demos

#### WinFormsDemo

Shows the [cairo examples](https://www.cairographics.org/samples/) in WinForms, plus some little extras added by me.

Even when you don't care about WinForms[^1], the _cairo_ stuff can be interesting. In this case, just ignore WinForms stuff and focus your eye on _cairo_.

[^1]: actually I don't use WinForms extensively nowadays. Just here and there for some quick ad-hoc visualization of simulations, and that like when I don't use GTK 4.

#### WinFormsAnimation

Shows how to display an animation, drawed via _cairo_ on WinForms. For each iteration an image can be stored to the output folder, and in the [ReadMe](./Windows/WinFormsAnimation/ReadMe.md) over there is a command on how to create a `mp4`-video out of the created image files.

A Linux / GTK version is in [Gtk4Animation](#gtk4animation).

#### WpfDemo

Shows how to draw in _cairo_, then how to put the image data into a custom `BitmapSource`, which will be shown in the UI.
