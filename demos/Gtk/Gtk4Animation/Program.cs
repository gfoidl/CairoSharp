// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Gtk;
using Gtk4Animation;

if (OperatingSystem.IsWindows())
{
    // GTK 4 is installed via https://www.gtk.org/docs/installations/windows/#using-gtk-from-msys2-packages
    // For simplicity we just append the PATH so that Windows knows where to look for the DLLs.
    string path = Environment.GetEnvironmentVariable("PATH")!;
    path        = $@"C:\Program Files\msys64\ucrt64\bin;{path}";
    Environment.SetEnvironmentVariable("PATH", path);
}

using Application app = Application.New("at.gfoidl.cairo.gtk4.animation", Gio.ApplicationFlags.FlagsNone);
app.OnActivate += static (Gio.Application app, EventArgs args) =>
{
    if (Directory.Exists("output"))
    {
        Directory.Delete("output", recursive: true);
    }

    AnimationWindow window = new((Application)app);
    window.Show();
};

#if USE_CSS
using CssProvider cssProvider = CssProvider.New();
cssProvider.LoadFromString("""
    .main-box {
        margin: 12px;
    }
    """);

Gdk.Display? display = Gdk.Display.GetDefault();
Debug.Assert(display is not null);
StyleContext.AddProviderForDisplay(display, cssProvider, 0);
#endif

return app.RunWithSynchronizationContext(args);
