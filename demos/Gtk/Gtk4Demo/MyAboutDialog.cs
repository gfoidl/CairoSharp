// (c) gfoidl, all rights reserved

using System.Reflection;
using Gtk;

namespace Gtk4Demo;

public sealed class MyAboutDialog : AboutDialog
{
    public MyAboutDialog()
    {
        this.ProgramName  = "Gtk4Demo";
        this.Authors      = ["gfoidl"];
        this.Comments     = "Demo application for CairoSharp and GTK 4";
        this.Copyright    = "© gfoidl";
        this.Version      = typeof(MyAboutDialog).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        this.Website      = "https://github.com/gfoidl/CairoSharp";
        this.WebsiteLabel = "Visit https://github.com/gfoidl/CairoSharp";
        this.LicenseType  = Gtk.License.Lgpl30;
        this.Logo         = Gdk.Texture.NewFromResource("/at/gfoidl/cairo/gtk4/demo/icons/gtk4demo.png");
    }
}
