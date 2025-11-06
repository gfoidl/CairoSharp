// (c) gfoidl, all rights reserved

#define USE_TICK_CALLBACK

using System.Diagnostics;
using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Gtk;
using Gtk4.Extensions;
using static Gtk4.Extensions.Gtk4Constants;
using RadialGradient = Cairo.Drawing.Patterns.RadialGradient;

namespace Gtk4Animation;

public sealed class AnimationWindow : ApplicationWindow
{
    private const int BallSize = 20;

#pragma warning disable CS0649 // field is never assigned to
    [Connect] private readonly CheckButton _showTrajectoryCheckBox;
    [Connect] private readonly CheckButton _showCrosshairsCheckBox;
    [Connect] private readonly CheckButton _saveImagesCheckBox;
    [Connect] private readonly DrawingArea _drawingArea;
    [Connect] private readonly Label       _iterationLabel;
    [Connect] private readonly Label       _fpsLabel;
#pragma warning restore CS0649

    private readonly List<PointD> _points;

    private double _curX;
    private double _curY;

#if USE_TICK_CALLBACK
    private long _frameTimeMicros;
#endif

    public AnimationWindow(Application app) : this(app, new Builder("AnimationWindow.4.ui"), "animationWindow") { }

    private AnimationWindow(Application app, Builder builder, string name)
        : base(new Gtk.Internal.ApplicationWindowHandle(builder.GetPointer(name), ownsHandle: false))
    {
        this.Application = app;

        builder.Connect(this);
        builder.Dispose();

        Debug.Assert(_showTrajectoryCheckBox is not null);
        Debug.Assert(_showCrosshairsCheckBox is not null);
        Debug.Assert(_saveImagesCheckBox     is not null);
        Debug.Assert(_drawingArea            is not null);
        Debug.Assert(_iterationLabel         is not null);
        Debug.Assert(_fpsLabel               is not null);

        _drawingArea.SetDrawFunc(this.Draw);

        _curX = Random.Shared.Next(0, _drawingArea.ContentWidth);
        _curY = Random.Shared.Next(0, _drawingArea.ContentHeight);

        _points = [new PointD(_curX, _curY)];

#if !USE_TICK_CALLBACK
        GLib.Functions.TimeoutAdd(priority: 0, interval: 50, this.OnTimeout);
#else
        this.AddTickCallback((Widget widget, Gdk.FrameClock frameClock) =>
        {
            long frameTimeMicros = frameClock.GetFrameTime();

            if (frameTimeMicros - _frameTimeMicros > 50 * 1000)
            {
                double fps = frameClock.GetFps();
                _fpsLabel.SetText($"FPS: {fps:N1}");

                _frameTimeMicros = frameTimeMicros;
                return this.OnTimeout();
            }

            return SourceContinue;
        });
#endif
    }

    private bool OnTimeout()
    {
        Debug.WriteLine("Timeout triggered");

        _drawingArea.QueueDraw();

        _iterationLabel.SetText($"Iteration: {_points.Count:D3}");
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

        _points.Add(new PointD(_curX, _curY));
    }

    private unsafe void Draw(DrawingArea drawingArea, CairoContext cr, int width, int height)
    {
        Debug.WriteLine($"Drawing {drawingArea.Handle.DangerousGetHandle()} with context {(nint)cr.NativeContext} and w x h = {width} x {height}");

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

        if (_showCrosshairsCheckBox.Active)
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

        if (_showTrajectoryCheckBox.Active && _points.Count > 0)
        {
            cr.MoveTo(_points[0]);

            for (int i = 1; i < _points.Count; ++i)
            {
                cr.LineTo(_points[i]);
            }

            cr.Color = new Color(0, 0, 1);
            cr.LineWidth = 2;
            cr.Stroke();

            // Starting point
            cr.Color = new Color(0, 1, 0);
            cr.Arc(_points[0], 5, 0, 2 * Math.PI);
            cr.Fill();
        }

        // Bullet
        using (cr.Save())
        {
            using RadialGradient radialGradient = new(0, 0, BallSize * 0.8, 0, 0, 0);
            radialGradient.AddColorStopRgba(0, 1, 0, 0, 1.0);
            radialGradient.AddColorStopRgba(1, 1, 1, 1, 1.0);

            cr.Translate(_curX, _curY);
            cr.SetSource(radialGradient);
            cr.Arc(0, 0, BallSize, 0, 2 * Math.PI);
            cr.Fill();
        }

        if (_saveImagesCheckBox.Active)
        {
            Directory.CreateDirectory("output");
            cr.Target.WriteToPng($"output/img{_points.Count:000}.png");
        }
    }
}
