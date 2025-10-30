// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo;
using Cairo.Extensions;
using Gtk;
using Gtk4.Extensions;
using RadialGradient = Cairo.Drawing.Patterns.RadialGradient;

namespace Gtk4Animation;

public sealed class AnimationWindow : ApplicationWindow
{
    private const int BallSize = 20;

    private readonly List<PointD> _points;
    private readonly CheckButton  _showTrajectoryCheckBox;
    private readonly CheckButton  _showCrosshairsCheckBox;
    private readonly CheckButton  _saveImagesCheckBox;
    private readonly DrawingArea  _drawingArea;
    private readonly Label        _iterationLabel;

    private double _curX;
    private double _curY;

    public AnimationWindow(Application app)
    {
        this.Application = app;
        this.Title       = "Cairo Animation Demo";
        this.Resizable   = false;

        Box checkBoxHorizontalBox = Box.New(Orientation.Horizontal, 0);
        {
            _showTrajectoryCheckBox        = CheckButton.NewWithLabel("show trajectory");
            _showTrajectoryCheckBox.Active = true;
            checkBoxHorizontalBox.Append(_showTrajectoryCheckBox);

            _showCrosshairsCheckBox        = CheckButton.NewWithLabel("show crosshairs");
            _showCrosshairsCheckBox.Active = true;
            checkBoxHorizontalBox.Append(_showCrosshairsCheckBox);

            _saveImagesCheckBox         = CheckButton.NewWithLabel("save images");
            _saveImagesCheckBox.Hexpand = true;
            _saveImagesCheckBox.Halign  = Align.End;
            checkBoxHorizontalBox.Append(_saveImagesCheckBox);
        }

        _drawingArea = DrawingArea.New();
        {
            //_drawingArea.SetSizeRequest(600, 400);
            _drawingArea.ContentWidth  = 600;
            _drawingArea.ContentHeight = 400;
            _drawingArea.SetDrawFunc(this.Draw);
        }

        _iterationLabel = Label.New(str: null);
        {
            _iterationLabel.Halign         = Align.Start;
            _iterationLabel.SingleLineMode = true;
        }

        Box mainBox = Box.New(Orientation.Vertical, 12);
        mainBox.Append(checkBoxHorizontalBox);
        mainBox.Append(_drawingArea);
        mainBox.Append(_iterationLabel);

        this.Child = mainBox;

#if USE_CSS
        mainBox.AddCssClass("main-box");
#else
        mainBox.MarginStart  = 12;
        mainBox.MarginTop    = 12;
        mainBox.MarginEnd    = 12;
        mainBox.MarginBottom = 12;
#endif

        _curX = Random.Shared.Next(0, _drawingArea.ContentWidth);
        _curY = Random.Shared.Next(0, _drawingArea.ContentHeight);

        _points = [new PointD(_curX, _curY)];

        GLib.Functions.TimeoutAdd(priority: 0, interval: 50, this.OnTimeout);
    }

    private bool OnTimeout()
    {
        Debug.WriteLine("Timeout triggered");

        const bool SourceContinue = true;
        const bool SourceRemove   = false;

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
