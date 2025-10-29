// (c) gfoidl, all rights reserved

using System.Diagnostics;
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
app.OnActivate += static (Gio.Application app, EventArgs args) =>
{
    MainWindow window = new((Application)app);
    window.Show();
};
app.OnStartup += static (Gio.Application app, EventArgs args) =>
{
    using Builder builder = new("demo.4.ui");
    Gio.MenuModel? mainMenu = builder.GetObject("mainMenu") as Gio.MenuModel;

    Debug.Assert(mainMenu is not null);
    ((Application)app).SetMenubar(mainMenu);
};

return app.RunWithSynchronizationContext(args);
