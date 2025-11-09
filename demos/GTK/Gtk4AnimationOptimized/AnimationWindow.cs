// (c) gfoidl, all rights reserved

#define USE_TICK_CALLBACK
#define DUMP_FPS

using System.Diagnostics;
using System.Globalization;
using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Gtk;
using Gtk4.Extensions;
using static Gtk4.Extensions.Gtk4Constants;
using Format         = Cairo.Surfaces.Format;
using ImageSurface   = Cairo.Surfaces.Images.ImageSurface;
using RadialGradient = Cairo.Drawing.Patterns.RadialGradient;

namespace Gtk4Animation;

public sealed class AnimationWindow : ApplicationWindow
{
    private const int BallSize = 20;

#pragma warning disable CS0649 // field is never assigned to
    [Connect] private readonly CheckButton _showTrajectoryCheckButton;
    [Connect] private readonly CheckButton _showCrosshairsCheckButton;
    [Connect] private readonly CheckButton _saveImagesCheckButton;
    [Connect] private readonly DrawingArea _drawingArea;
    [Connect] private readonly Label       _iterationLabel;
    [Connect] private readonly Label       _fpsLabel;
#pragma warning restore CS0649

    private ImageSurface? _ballSurface;
    private ImageSurface? _trajectorySurface;
    private CairoContext? _trajectoryContext;

    private double _curX;
    private double _curY;
    private int    _moves;
    private PointD _lastPoint;

#if USE_TICK_CALLBACK
    private long _lastFrameTimeMicros;
#endif

#if DUMP_FPS
    private readonly StreamWriter _fpsWriter;
#endif

    public AnimationWindow(Application app) : this(app, new Builder("AnimationWindow.4.ui"), "animationWindow") { }

    private AnimationWindow(Application app, Builder builder, string name)
        : base(new Gtk.Internal.ApplicationWindowHandle(builder.GetPointer(name), ownsHandle: false))
    {
        this.Application = app;

        builder.Connect(this);
        builder.Dispose();

        Debug.Assert(_showTrajectoryCheckButton is not null);
        Debug.Assert(_showCrosshairsCheckButton is not null);
        Debug.Assert(_saveImagesCheckButton     is not null);
        Debug.Assert(_drawingArea               is not null);
        Debug.Assert(_iterationLabel            is not null);
        Debug.Assert(_fpsLabel                  is not null);

        _drawingArea.SetDrawFunc(this.Draw);

        _curX = Random.Shared.Next(0, _drawingArea.ContentWidth);
        _curY = Random.Shared.Next(0, _drawingArea.ContentHeight);

#if !USE_TICK_CALLBACK
        GLib.Functions.TimeoutAdd(priority: 0, interval: 50, this.OnTimeout);
#else
        this.AddTickCallback((Widget widget, Gdk.FrameClock frameClock) =>
        {
            long frameTimeMicros = frameClock.GetFrameTime();

            if (frameTimeMicros - _lastFrameTimeMicros > 50 * 1000)
            {
                double fps = frameClock.GetFps();
                _fpsLabel.SetText($"FPS: {fps:N1}");

#if DUMP_FPS
                _fpsWriter?.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{_moves};{fps}"));
#endif

                _lastFrameTimeMicros = frameTimeMicros;
                return this.OnTimeout();
            }

            return SourceContinue;
        });
#endif

#if DUMP_FPS
        _fpsWriter = File.CreateText("fps.csv");
        _fpsWriter.WriteLine("Moves;FPS");
#endif
    }

    private bool OnTimeout()
    {
        Debug.WriteLine("Timeout triggered");

        _drawingArea.QueueDraw();

        _iterationLabel.SetText($"Iteration: {_moves:D3}");
        this.CalculateNextPosition();

        return SourceContinue;
    }

    private void CalculateNextPosition()
    {
        _curX += Random.Shared.Next(-BallSize, BallSize);
        _curY += Random.Shared.Next(-BallSize, BallSize);

        int width  = _drawingArea.ContentWidth;
        int height = _drawingArea.ContentHeight;

        _curX = Math.Clamp(_curX, 0 + BallSize, width  - BallSize);
        _curY = Math.Clamp(_curY, 0 + BallSize, height - BallSize);

        _moves++;
    }

    private static ImageSurface CreateBallSurface(Cairo.Surfaces.Surface targetSurface, int width, int height)
    {
        ImageSurface imageSurface = targetSurface.CreateSimilarImage(Format.Argb32, width, height);
        using CairoContext cr     = new(imageSurface);

        cr.Translate(BallSize, BallSize);

        using RadialGradient radialGradient = new(0, 0, BallSize * 0.8, 0, 0, 0);
        radialGradient.AddColorStopRgba(0, 1, 0, 0, 1.0);
        radialGradient.AddColorStopRgba(1, 1, 1, 1, 1.0);

        cr.SetSource(radialGradient);
        cr.Arc(0, 0, BallSize, 0, 2 * Math.PI);
        cr.Fill();

        return imageSurface;
    }

    private unsafe void Draw(DrawingArea drawingArea, CairoContext cr, int width, int height)
    {
        Debug.WriteLine($"Drawing {drawingArea.Handle.DangerousGetHandle()} with context {(nint)cr.NativeContext} and w x h = {width} x {height}");

        _ballSurface ??= CreateBallSurface(cr.Target, width, height);

        if (_trajectorySurface is null)
        {
            Debug.Assert(_trajectoryContext is null);

            _trajectorySurface = cr.Target.CreateSimilarImage(Format.Argb32, width, height);
            _trajectoryContext = new CairoContext(_trajectorySurface);

            // Starting point
            _trajectoryContext.Arc(_curX, _curY, 5, 0, 2 * Math.PI);
            _trajectoryContext.Color = new Color(0, 1, 0);
            _trajectoryContext.Fill();

            _trajectoryContext.Color     = new Color(0, 0, 1);
            _trajectoryContext.LineWidth = 2;
            _lastPoint                   = new(_curX, _curY);
        }
        Debug.Assert(_trajectoryContext is not null);

        // Background
        using (cr.Save())
        {
            cr.Rectangle(0, 0, width, height);
            cr.Color = Color.FromRgbaBytes(162, 250, 159, 51);  // https://rgbacolorpicker.com/
            cr.FillPreserve();

            cr.Color     = Color.Default;
            cr.LineWidth = 5;
            cr.Stroke();
        }

        if (_showCrosshairsCheckButton.Active)
        {
            using (cr.Save())
            {
                cr.Color = Color.Default;
                cr.SetDash(10, 5, 2, 5);    // 10 long, 5 off, 2 long, 5 off and repeat

                cr.Hairline = true;

                cr.MoveTo(_curX, 0);
                cr.RelLineTo(0, height);
                cr.Stroke();

                cr.MoveTo(0, _curY);
                cr.RelLineTo(width, 0);
                cr.Stroke();
            }
        }

        if (_moves > 1)
        {
            // It must be a Stroke, as with StrokePreserve the path gets longer, and longer,
            // so it wouldn't be a improvement.

            _trajectoryContext.MoveTo(_lastPoint);
            _lastPoint = new PointD(_curX, _curY);
            _trajectoryContext.LineTo(_lastPoint);
            _trajectoryContext.Stroke();

            if (_showTrajectoryCheckButton.Active)
            {
                cr.SetSourceSurface(_trajectorySurface, 0, 0);
                cr.Paint();
            }
        }

        // Bullet
        using (cr.Save())
        {
            cr.Translate(-BallSize, -BallSize);     // undo construction offset
            cr.SetSourceSurface(_ballSurface, _curX, _curY);
            cr.Paint();
        }

        if (_saveImagesCheckButton.Active)
        {
            Directory.CreateDirectory("output");
            cr.Target.WriteToPng($"output/img{_moves:000}.png");
        }
    }
}
