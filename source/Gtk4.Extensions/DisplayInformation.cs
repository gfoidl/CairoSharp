// (c) gfoidl, all rights reserved

using Gdk;
using Gio;

namespace Gtk4.Extensions;

public record DisplayInformation(string DisplayName)
{
    public required IReadOnlyCollection<MonitorInformation> MonitorInformations { get; init; }

    public static DisplayInformation? GetForDefaultDisplay()
    {
        Display? defaultDisplay = Display.GetDefault();

        if (defaultDisplay is null)
        {
            return null;
        }

        ListModel monitors                           = defaultDisplay.GetMonitors();
        List<MonitorInformation> monitorInformations = [];
        uint monitorCount                            = monitors.GetNItems();

        for (uint i = 0; i < monitorCount; ++i)
        {
            if (monitors.GetObject(i) is not Gdk.Monitor monitor)
            {
                continue;
            }

            monitorInformations.Add(new MonitorInformation(
                monitor.Manufacturer,
                monitor.Description,
                monitor.Model,
                monitor.RefreshRate,
                monitor.WidthMm,
                monitor.HeightMm,
                monitor.Scale,
                monitor.ScaleFactor,
                monitor.SubpixelLayout));
        }

        return new DisplayInformation(defaultDisplay.GetName())
        {
            MonitorInformations = monitorInformations
        };
    }
}

public record MonitorInformation(
    string?        Manufacturer,
    string?        Description,
    string?        Model,
    int            RefreshRate,
    int            WidthInMillimeters,
    int            HeightInMillimeterrs,
    double         Scale,
    int            ScaleFactor,
    SubpixelLayout SubpixelLayout);
