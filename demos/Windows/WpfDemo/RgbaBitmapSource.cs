// (c) gfoidl, all rights reserved

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfDemo;

internal sealed class RgbaBitmapSource : BitmapSource
{
    public override PixelFormat Format => PixelFormats.Pbgra32;
    public override double DpiX => 96d;
    public override double DpiY => 96d;

    public override int PixelWidth  => field;
    public override int PixelHeight => field;

    public override double Width  => this.PixelWidth;
    public override double Height => this.PixelHeight;

    private readonly byte[] _rgbaBuffer;

    public RgbaBitmapSource(ReadOnlySpan<byte> rgbaBuffer, int pixelWidth)
    {
        _rgbaBuffer      = rgbaBuffer.ToArray();
        this.PixelWidth  = pixelWidth;
        this.PixelHeight = rgbaBuffer.Length / (4 * pixelWidth);
    }

    public override void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
    {
        ReadOnlySpan<byte> rgbaBuffer = _rgbaBuffer;

        for (int y = sourceRect.Y; y < sourceRect.Y + sourceRect.Height; ++y)
        {
            for (int x = sourceRect.X; x < sourceRect.X + sourceRect.Width; ++x)
            {
                int idx = stride * y + 4 * x;

                byte a = rgbaBuffer[idx + 3];
                byte r = (byte)(int)(rgbaBuffer[idx + 0] * a / 256d);
                byte g = (byte)(int)(rgbaBuffer[idx + 1] * a / 256d);
                byte b = (byte)(int)(rgbaBuffer[idx + 2] * a / 256d);

                pixels.SetValue(b, idx + offset + 0);
                pixels.SetValue(g, idx + offset + 1);
                pixels.SetValue(r, idx + offset + 2);
                pixels.SetValue(a, idx + offset + 3);
            }
        }
    }

    protected override Freezable CreateInstanceCore() => new RgbaBitmapSource(_rgbaBuffer, this.PixelWidth);

#pragma warning disable CS0067              // The event is never used
    public override event EventHandler<DownloadProgressEventArgs>? DownloadProgress;
    public override event EventHandler? DownloadCompleted;
    public override event EventHandler<ExceptionEventArgs>? DownloadFailed;
    public override event EventHandler<ExceptionEventArgs>? DecodeFailed;
#pragma warning restore CS0067
}
