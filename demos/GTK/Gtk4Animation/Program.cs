// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Gtk;
using Gtk4.Extensions;
using Gtk4Animation;

if (OperatingSystem.IsWindows())
{
    // Renders are more beautiful Window w/o any artifacts from the rounded edges on Windows.
    Environment.SetEnvironmentVariable("GSK_RENDERER", "vulkan");

    // GTK 4 is installed via https://www.gtk.org/docs/installations/windows/#using-gtk-from-msys2-packages
    // For simplicity we just append the PATH so that Windows knows where to look for the DLLs.
    string path = Environment.GetEnvironmentVariable("PATH")!;
    path        = $@"C:\Program Files\msys64\ucrt64\bin;{path}";
    Environment.SetEnvironmentVariable("PATH", path);
}

using Application app = Application.New("at.gfoidl.cairo.gtk4.animation", Gio.ApplicationFlags.FlagsNone);
app.OnActivate += static (Gio.Application gioApp, EventArgs args) =>
{
    if (Directory.Exists("output"))
    {
        Directory.Delete("output", recursive: true);
    }

    Application app = (Application)gioApp;
    Window window   = app.ActiveWindow ?? new AnimationWindow(app);

    window.Present();
};

#if USE_CSS
using CssProvider cssProvider = CssProvider.New();

#if CSS_THROW_ON_PARSING_ERROR
cssProvider.OnParsingError += static (CssProvider cssProvider, CssProvider.ParsingErrorSignalArgs args) =>
{
    throw new Exception($"""
                Section: {args.Section.ToString()}
                Error:   {args.Error.Message}
                """);
};
#endif

cssProvider.LoadFromString("""
    box.main {
        margin: 12px;
    }
    """);

Gdk.Display? display = Gdk.Display.GetDefault();
Debug.Assert(display is not null);
StyleContext.AddProviderForDisplay(display, cssProvider, Gtk4Constants.StyleProviderPriorityUser - 1);
#endif

return app.RunWithSynchronizationContext(args);
