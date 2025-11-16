// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Gtk;
using Gtk4.Extensions;
using Gtk4Demo;
using Spectre.Console;

#if USE_LIB_ADWAITA
using Application = Adw.Application;
#else
using Application = Gtk.Application;
#endif

if (OperatingSystem.IsWindows())
{
#if !USE_LIB_ADWAITA
    // client-side decorations for a more Windows-like look and feel, cf. https://docs.gtk.org/gtk4/running.html#gtk_csd
    // See also https://docs.gtk.org/gtk4/method.HeaderBar.set_use_native_controls.html
    Environment.SetEnvironmentVariable("GTK_CSD", "0");
#endif

    // Renders are more beautiful Window w/o any artifacts from the rounded edges on Windows.
    Environment.SetEnvironmentVariable("GSK_RENDERER", "vulkan");

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

    window.Present();
};

#if UI_FROM_RESOURCE
using (Gio.Resource resource = Gio.Resource.Load("gtk4demo.gresource"))
{
    resource.Register();
}
#endif

AddCss();
PrintDisplayInformation();

return app.RunWithSynchronizationContext(args);
//-----------------------------------------------------------------------------
static void AddCss()
{
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

    cssProvider.LoadFromResource("/at/gfoidl/cairo/gtk4/demo/styles/builder/main.css");

    Gdk.Display? display = Gdk.Display.GetDefault();
    Debug.Assert(display is not null);

    StyleContext.AddProviderForDisplay(display, cssProvider, Gtk4Constants.StyleProviderPriorityUser - 1);
}
//-----------------------------------------------------------------------------
static void PrintDisplayInformation()
{
    DisplayInformation? displayInformation = DisplayInformation.GetForDefaultDisplay();

    if (displayInformation is not null)
    {
        Tree tree = new("Display information");
        tree.AddNode($"name:        {displayInformation.DisplayName}");
        tree.AddNode($"GDK_BACKEND: {Environment.GetEnvironmentVariable("GDK_BACKEND") ?? "not set"}");

        Table table = new();
        table
            .Title("Monitors")
            .AddColumns("Manufacturer", "Description", "Model", "Refresh rate", "Width [[mm]]", "Height [[mm]]", "Scale", "Scale factor", "Subpixel layout");

        foreach (MonitorInformation monitorInformation in displayInformation.MonitorInformations)
        {
            table.AddRow(
                monitorInformation.Manufacturer ?? "-",
                monitorInformation.Description  ?? "-",
                monitorInformation.Model        ?? "-",
                monitorInformation.RefreshRate         .ToString(),
                monitorInformation.WidthInMillimeters  .ToString(),
                monitorInformation.HeightInMillimeterrs.ToString(),
                monitorInformation.Scale               .ToString(),
                monitorInformation.ScaleFactor         .ToString(),
                monitorInformation.SubpixelLayout      .ToString());
        }

        tree.AddNode(table);

        AnsiConsole.Write(tree);
        Console.WriteLine();
    }
}
