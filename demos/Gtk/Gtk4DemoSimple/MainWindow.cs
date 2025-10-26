// (c) gfoidl, all rights reserved

// This won't work on Windows (why?), but it works on Linux.
#define USE_FRAME_WIDGET

using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Shapes;
using Gtk;
using GtkCairo       = Cairo.Context;
using LinearGradient = Cairo.Drawing.Patterns.LinearGradient;
using RadialGradient = Cairo.Drawing.Patterns.RadialGradient;

namespace Gtk4DemoSimple;

public sealed class MainWindow : ApplicationWindow
{
    public MainWindow(Application app)
    {
        this.SetApplication(app);
        this.SetTitle("CairoSharp");
        //this.SetDefaultSize(500, 500);    // set below for the drawingArea

        DrawingArea drawingArea = DrawingArea.New();
        drawingArea.SetSizeRequest(500, 500);
        drawingArea.SetDrawFunc(Draw);

#if USE_FRAME_WIDGET
        Frame frame        = Frame.New(label: null);
        frame.MarginStart  = 8;
        frame.MarginTop    = 8;
        frame.MarginEnd    = 8;
        frame.MarginBottom = 8;

        frame.SetChild(drawingArea);
        this.SetChild(frame);
#else
        drawingArea.MarginStart  = 8;
        drawingArea.MarginTop    = 8;
        drawingArea.MarginEnd    = 8;
        drawingArea.MarginBottom = 8;

        this.SetChild(drawingArea);
#endif
    }

    private static void Draw(DrawingArea drawingArea, GtkCairo gtkCairoContext, int width, int height)
    {
        using CairoContext cr = new(gtkCairoContext.Handle.DangerousGetHandle());

        cr.Color = KnownColors.OldLace;
        cr.Paint();

        cr.Color = KnownColors.Black;

        using (LinearGradient pat = new(0.0, 0.0, 0.0, 256.0))
        {
            pat.AddColorStopRgba(0, 1, 1, 1, 1);
            pat.AddColorStopRgba(1, 0, 0, 0, 1);

            cr.Rectangle(0, 0, 256, 256);
            cr.SetSource(pat);
            cr.Fill();
        }

        using (RadialGradient pat = new(115.2, 102.4, 25.6,
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
            cr.Color = KnownColors.DarkGreen;
            cr.LineWidth = 5;
            cr.Stroke();
        }
    }
}
