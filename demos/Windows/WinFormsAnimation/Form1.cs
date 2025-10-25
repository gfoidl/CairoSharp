// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo;
using Cairo.Drawing.Patterns;
using Cairo.Extensions;
using Cairo.Surfaces.Win32;
using Color     = Cairo.Color;
using Rectangle = Cairo.Rectangle;

namespace WinFormsAnimation;

public partial class Form1 : Form
{
    private const int BallSize = 20;

    private readonly List<PointD> _points;

    private double _curX;
    private double _curY;

    public Form1()
    {
        InitializeComponent();

        _curX = Random.Shared.Next(0, panel1.Width);
        _curY = Random.Shared.Next(0, panel1.Height);

        _points = [new PointD(_curX, _curY)];
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Directory.Exists("output"))
        {
            Directory.Delete("output", recursive: true);
        }

        animationTimer.Enabled = true;
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
        Debug.WriteLine($"Paint client rectangle: {e.ClipRectangle.Left}, {e.ClipRectangle.Top}, {e.ClipRectangle.Width}, {e.ClipRectangle.Height}");

        Rectangle clientRectangle = new(e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height);
        nint hdc = e.Graphics.GetHdc();

        try
        {
            this.Draw(hdc, clientRectangle);
        }
        finally
        {
            e.Graphics.ReleaseHdc(hdc);
        }

        iterationStripStatusLabel.Text = $"Iteration: {_points.Count:D3}";
        this.CalculateNextPosition(clientRectangle);
    }

    private void Draw(IntPtr hdc, Rectangle clientRectangle)
    {
        using Win32Surface surface = new(hdc);
        using CairoContext cr      = new(surface);

        // Background
        using (cr.Save())
        {
            cr.Rectangle(clientRectangle);
            cr.Color = Color.FromRgbaBytes(162, 250, 159, 51);  // https://rgbacolorpicker.com/
            cr.FillPreserve();

            cr.Color     = Color.Default;
            cr.LineWidth = 5;
            cr.Stroke();
        }

        if (showCrossHairsCheckBox.Checked)
        {
            using (cr.Save())
            {
                cr.Color = Color.Default;
                cr.SetDash(10, 5, 2, 5);    // 10 long, 5 off, 2 long, 5 off and repeat

                cr.Hairline = true;

                cr.MoveTo(_curX, 0);
                cr.RelLineTo(0, clientRectangle.Height);
                cr.Stroke();

                cr.MoveTo(0, _curY);
                cr.RelLineTo(clientRectangle.Width, 0);
                cr.Stroke();
            }
        }

        if (showTrajectoryCheckBox.Checked && _points.Count > 0)
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

        if (saveImagesCheckBox.Checked)
        {
            Directory.CreateDirectory("output");
            surface.WriteToPng($"output/img{_points.Count:000}.png");
        }
    }

    private void CalculateNextPosition(Rectangle clientRectangle)
    {
        _curX += Random.Shared.Next(-BallSize, BallSize);
        _curY += Random.Shared.Next(-BallSize, BallSize);

        _curX = Math.Clamp(_curX, 0 + BallSize, clientRectangle.Width  - BallSize);
        _curY = Math.Clamp(_curY, 0 + BallSize, clientRectangle.Height - BallSize);

        _points.Add(new PointD(_curX, _curY));
    }

    private void animationTimer_Tick(object sender, EventArgs e)
    {
        panel1.Invalidate();
    }
}
