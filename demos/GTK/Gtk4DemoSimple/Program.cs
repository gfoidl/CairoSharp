// (c) gfoidl, all rights reserved

using Gtk;
using Gtk4DemoSimple;

FixupEnvironment();

using Application app = Application.New("at.gfoidl.cairo.gtk4.demo.simple", Gio.ApplicationFlags.FlagsNone);
app.OnActivate += static (Gio.Application gioApp, EventArgs args) =>
{
    Application app = (Application)gioApp;
    Window window   = app.ActiveWindow ?? new MainWindow(app);

    window.Present();
};

return app.RunWithSynchronizationContext(args);
//-----------------------------------------------------------------------------
static void FixupEnvironment()
{
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
}
