// (c) gfoidl, all rights reserved

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

    private static readonly double[][]   s_funcData;
    private static readonly ImageSurface s_functionImageSurface;

    private DevicePosition _mousePosition;
    private bool           _mouseIsInDrawingArea;
    //-------------------------------------------------------------------------
    static MainWindow()
    {
        s_funcData             = Calculator.CalculateData<PeaksFunction>(out double funcMin, out double funcMax);
        s_functionImageSurface = Plotter.CreateFunctionSurface<TurboColorMap>(s_funcData, funcMin, funcMax);
    }
    //-------------------------------------------------------------------------
    public MainWindow(Application app)
    {
        this.Application = app;
        this.Title       = "Peaks function plot";
        this.Resizable   = false;

        this.Child = this.CreateDrawingArea();
    }
    //-------------------------------------------------------------------------
    private DrawingArea CreateDrawingArea()
    {
        DrawingArea drawingArea   = DrawingArea.New();
        drawingArea.ContentWidth  = Size;
        drawingArea.ContentHeight = Size;

        drawingArea.MarginStart   = 10;
        drawingArea.MarginTop     = 10;
        drawingArea.MarginEnd     = 10;
        drawingArea.MarginBottom  = 10;

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

        using (cr.Save())
        {
            cr.Rectangle(0, 0, width, height);
            cr.LineWidth = 1d;
            cr.Stroke();
        }

        cr.SetSourceSurface(s_functionImageSurface, 0, 0);
        cr.Paint();

        if (_mouseIsInDrawingArea)
        {
            Plotter.DrawCrosshairs(cr, _mousePosition, width, height);

            if (Calculator.TryGetCoordinatesAndFuncValue(s_funcData, _mousePosition, out double x, out double y, out double z))
            {
                Plotter.DrawCurrentValue(cr, width, height, _mousePosition, x, y, z);
            }
        }
    }
}
