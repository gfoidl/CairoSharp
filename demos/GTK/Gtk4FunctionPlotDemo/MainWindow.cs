// (c) gfoidl, all rights reserved

//#define USE_NEW_IMAGE_SURFACE_FOR_DATA

extern alias CairoSharp;

using System.Diagnostics;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Extensions.Colors.ColorMaps.Optimized;
using Cairo.Extensions.Pixels;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using Gtk;
using Gtk4.Extensions;
using Gtk4Demo.Pixels;

namespace Gtk4FunctionPlotDemo;

public sealed class MainWindow : ApplicationWindow
{
    internal const int Size       = 600;
    internal const double SizeInv = 1d / (Size - 1);

    private static readonly double[][] s_funcData = Calculator.CalculateData<PeaksFunction>(out s_funcMin, out s_funcMax);
    private static readonly double     s_funcMin;
    private static readonly double     s_funcMax;

    private readonly CheckButton _annotationDarkSchemeCheckButton;

    private ImageSurface? _functionImageSurface;
    private MousePosition _mousePosition;
    private bool          _mouseIsInDrawingArea;
    //-------------------------------------------------------------------------
    public MainWindow(Application app)
    {
        this.Application = app;
        this.Title       = "Peaks function plot";
        this.Resizable   = false;

        Box box    = Box.New(Orientation.Vertical, spacing: 10);
        this.Child = box;

        box.MarginStart  = 10;
        box.MarginTop    = 10;
        box.MarginEnd    = 10;
        box.MarginBottom = 10;

        _annotationDarkSchemeCheckButton        = CheckButton.NewWithLabel("annotation dark scheme");
        _annotationDarkSchemeCheckButton.Halign = Align.Start;
        box.Append(_annotationDarkSchemeCheckButton);

        box.Append(Separator.New(Orientation.Horizontal));
        box.Append(this.CreateDrawingArea());
    }
    //-------------------------------------------------------------------------
    private DrawingArea CreateDrawingArea()
    {
        DrawingArea drawingArea   = DrawingArea.New();
        drawingArea.ContentWidth  = Size;
        drawingArea.ContentHeight = Size;

        // Hide the mouse pointer / cursor, as we have a cross-hair.
        // Cf. https://docs.gtk.org/gtk4/method.Widget.set_cursor_from_name.html
        // and https://docs.gtk.org/gdk4/ctor.Cursor.new_from_name.html
        drawingArea.SetCursorFromName("none");

        drawingArea.SetDrawFunc(this.Draw);

        EventControllerMotion mouseMove = EventControllerMotion.New();
        drawingArea.AddController(mouseMove);

        mouseMove.OnMotion += (EventControllerMotion sender, EventControllerMotion.MotionSignalArgs args) =>
        {
            _mousePosition.X = args.X;
            _mousePosition.Y = args.Y;

            (sender.Widget as DrawingArea)?.QueueDraw();
        };

        mouseMove.OnEnter += (EventControllerMotion sender, EventControllerMotion.EnterSignalArgs args) =>
        {
            _mousePosition.X      = args.X;
            _mousePosition.Y      = args.Y;
            _mouseIsInDrawingArea = true;

            (sender.Widget as DrawingArea)?.QueueDraw();
        };

        mouseMove.OnLeave  += (EventControllerMotion sender, EventArgs args) =>
        {
            _mouseIsInDrawingArea = false;

            (sender.Widget as DrawingArea)?.QueueDraw();
        };

        return drawingArea;
    }
    //-------------------------------------------------------------------------
    private void Draw(DrawingArea drawingArea, CairoContext cr, int width, int height)
    {
        Debug.Assert(width  == Size);
        Debug.Assert(height == Size);

        if (_functionImageSurface is null)
        {
#if USE_NEW_IMAGE_SURFACE_FOR_DATA
            _functionImageSurface = new ImageSurface(Format.Argb32, Size, Size);
#else
            _functionImageSurface = cr.Target.CreateSimilarImage(Format.Argb32, Size, Size);
#endif
            Plotter.CreateFunctionSurface<TurboColorMap>(_functionImageSurface, s_funcData, s_funcMin, s_funcMax);
        }

        cr.SetSourceSurface(_functionImageSurface, 0, 0);
        cr.Paint();

        using (cr.Save())
        {
            cr.Rectangle(0, 0, width, height);
            cr.Color = Color.Default;
            cr.LineWidth = 1d;
            cr.Stroke();
        }

        if (_mouseIsInDrawingArea)
        {
            Plotter.DrawCrosshairs(cr, _mousePosition, width, height);

            if (Calculator.TryGetCoordinatesAndFuncValue(s_funcData, _mousePosition, out double x, out double y, out double z))
            {
                Plotter.DrawCurrentValue(cr, width, height, _mousePosition, x, y, z, _annotationDarkSchemeCheckButton.Active);
            }
        }
    }
}
