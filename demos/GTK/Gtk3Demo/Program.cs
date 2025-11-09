// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo;
using Cairo.Drawing.Patterns;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Shapes;
using Gtk3Demo;
using static Gtk3Demo.Native;

unsafe
{
    GtkApplication* app = gtk_application_new("at.gfoidl.cairo.gtk3.demo", 0);
    int status          = 0;

    try
    {
        g_signal_connect(app, "activate", &Activate, null);
        status = g_application_run(app, args.Length, args);
    }
    finally
    {
        g_object_unref(app);
    }

    return status;
}

static unsafe void Activate(void* type, void* userData)
{
    GtkApplication* app = (GtkApplication*)type;
    GtkWidget* window   = gtk_application_window_new(app);

    gtk_window_set_title(window, "CairoSharp");
    //gtk_window_set_default_size(window, 500, 500);    // set below for the drawingArea
    gtk_container_set_border_width(window, 8);

    g_signal_connect(window, "destroy", &CloseWindow, null);

    GtkWidget* frame = gtk_frame_new(label: null);
    gtk_frame_set_shadow_type(frame, /*  GTK_SHADOW_IN */ 1);
    gtk_container_add(window, frame);

    GtkWidget* drawingArea = gtk_drawing_area_new();
    gtk_widget_set_size_request(drawingArea, 500, 500);
    gtk_container_add(frame, drawingArea);

    g_signal_connect(drawingArea, "draw", &Draw, null);

    gtk_widget_show_all(window);
}

static unsafe void CloseWindow()
{
    // Clean up
}

static unsafe bool Draw(GtkWidget* widget, cairo_t* crNative, void* data)
{
    Debug.WriteLine("GTK draw is called");

    using CairoContext cr = new(crNative);

    cr.Color = KnownColors.OldLace;
    cr.Paint();

    cr.Color = KnownColors.Black;

    using (Gradient pat = new LinearGradient(0.0, 0.0, 0.0, 256.0))
    {
        pat.AddColorStopRgba(0, 1, 1, 1, 1);
        pat.AddColorStopRgba(1, 0, 0, 0, 1);

        cr.Rectangle(0, 0, 256, 256);
        cr.SetSource(pat);
        cr.Fill();
    }

    using (Gradient pat = new RadialGradient(115.2, 102.4, 25.6,
                                             102.4, 102.4, 128.0))
    {
        pat.AddColorStopRgba(0, 1, 1, 1, 1);
        pat.AddColorStopRgba(1, 0, 0, 0, 1);

        cr.SetSource(pat);
        cr.Arc(128.0, 128.0, 76.8, 0, 2 * Math.PI);
        cr.Fill();
    }

    cr.Hexagon(350, 250, 50);
    cr.Color = KnownColors.LightGreen;
    cr.FillPreserve();
    using (cr.Save())
    {
        cr.Color     = KnownColors.DarkGreen;
        cr.LineWidth = 5;
        cr.Stroke();
    }

    return false;
}
