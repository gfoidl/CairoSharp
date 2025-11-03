// (c) gfoidl, all rights reserved

using Gtk;
using Gtk4Demo;

if (OperatingSystem.IsWindows())
{
    // GTK 4 is installed via https://www.gtk.org/docs/installations/windows/#using-gtk-from-msys2-packages
    // For simplicity we just append the PATH so that Windows knows where to look for the DLLs.
    string path = Environment.GetEnvironmentVariable("PATH")!;
    path        = $@"C:\Program Files\msys64\ucrt64\bin;{path}";
    Environment.SetEnvironmentVariable("PATH", path);
}

using Application app = Application.New("at.gfoidl.cairo.gtk4.demo", Gio.ApplicationFlags.FlagsNone);
app.OnActivate += static (Gio.Application gioApp, EventArgs args) =>
{
    Application app = (Application)gioApp;
    Window window   = app.ActiveWindow ?? new MainWindow(app);

    window.Show();
};

#if UI_FROM_RESOURCE
using (Gio.Resource resource = Gio.Resource.Load("gtk4demo.gresource"))
{
    resource.Register();
}
#endif

return app.RunWithSynchronizationContext(args);
