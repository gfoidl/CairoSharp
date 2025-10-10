## CairoSharp Demos

### Preface

The "official" [cairo samples](https://www.cairographics.org/samples/) were ported to C# in the [original repository](https://github.com/zwcloud/CairoSharp), then got updated to modern .NET and enhanced by me.

### CairoDemo

Shows various examples, and demos ono how to use _cairo_.

There are also some enhanced features, like setting tags, and links in a PDF, creating user fonts, and that like.

`CairoDemo.NuGet` is the same, but not with a `<ProjectReference>` rather with a `<PackageReference>` (to test the NuGet-package).

### WpfDemo

Shows how to draw in _cairo_, then how to put the image data into a custom `BitmapSource`, which will be shown in the UI.

### WinFormsDemo

Shows the [cairo examples](https://www.cairographics.org/samples/) in WinForms, plus some little extras added by me.

Even when you don't care about WinForms[^1], the _cairo_ stuff can be interesting. In this case, just ignore WinForms stuff and focus your eye on _cairo_.

[^1]: actually I don't use WinForms extensively nowadays. Just here and there for some quick ad-hoc visualization of simulations, and that like.

### WinFormsAnimation

Shows how to display an animation, drawed via _cairo_ on WinForms. For each iteration an image can be stored to the output folder, and in the [ReadMe](./WinFormsAnimation/ReadMe.md) over there is a command on how to create a `mp4`-video out of the created image files.

### AspNetCoreDemo

This demo has a simple minimal api endpoint, that uses _cairo_ to render a SVG and write it to the output without the need of temporary files. This is done by either using the streaming API or registering a callback at surface construction.
