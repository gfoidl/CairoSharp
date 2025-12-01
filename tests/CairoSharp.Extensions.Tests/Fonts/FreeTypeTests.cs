// (c) gfoidl, all rights reserved

using System.Text;
using Cairo;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Fonts;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.Recording;
using Cairo.Surfaces.SVG;

namespace CairoSharp.Extensions.Tests.Fonts;

[TestFixture]
public class FreeTypeTests
{
    private static readonly byte[] s_expectedSvgDataFontOptions = File.ReadAllBytes("Fonts/font-options.svg");
    private static readonly byte[] s_expectedSvgDataText        = File.ReadAllBytes("Fonts/text.svg");

    private static readonly string s_expectedSvgStringFontOptions = Encoding.UTF8.GetString(s_expectedSvgDataFontOptions).Replace("\r\n", "\n");
    private static readonly string s_expectedSvgStringText        = Encoding.UTF8.GetString(s_expectedSvgDataText)       .Replace("\r\n", "\n");
    //-------------------------------------------------------------------------
    [Test]
    public unsafe void Face_info___OK()
    {
        using FreeTypeFont sanRemoFont = LoadFreeTypeFontFromFile("SanRemo.ttf");
        FT_FaceRec_* face              = sanRemoFont.LockFace();

        using (Assert.EnterMultipleScope())
        {
            
        }

        sanRemoFont.UnlockFace();
    }
    //-------------------------------------------------------------------------
    [Test]
    public void FontOptions_demo___OK()
    {
        byte[] actual = DrawFontOptions();

        AssertSvg(s_expectedSvgDataFontOptions, s_expectedSvgStringFontOptions, actual);
    }
    //-------------------------------------------------------------------------
    [Test]
    public void DrawText_demo___OK()
    {
        byte[] actual = DrawText();

        AssertSvg(s_expectedSvgDataText, s_expectedSvgStringText, actual);
    }
    //-------------------------------------------------------------------------
    [Test]
    public void FontOptions_demo_multiple_times___OK()
    {
        // 20 was chosen as debugging showed that at 14 iterations
        // FreeTypeExtensions.DestroyFunc is called.

        for (int i = 0; i < 20; ++i)
        {
            byte[] actual = DrawFontOptions();

            AssertSvg(s_expectedSvgDataFontOptions, s_expectedSvgStringFontOptions, actual);
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void DrawText_demo_multiple_times___OK()
    {
        // 20 was chosen as debugging showed that at 14 iterations
        // FreeTypeExtensions.DestroyFunc is called.

        for (int i = 0; i < 20; ++i)
        {
            byte[] actual = DrawText();

            AssertSvg(s_expectedSvgDataText, s_expectedSvgStringText, actual);
        }
    }
    //-------------------------------------------------------------------------
    [Test, CancelAfter(1_000)]
    public async Task DrawText_demo_parallel___OK(CancellationToken cancellationToken)
    {
        using CountdownEvent cde       = new(Environment.ProcessorCount);
        using ManualResetEventSlim mre = new();

        Task managerTask = Task.Run(() =>
        {
            cde.Wait();
            mre.Set();
        }, cancellationToken);

        int id = 0;
        ParallelOptions parallelOptions = new() { CancellationToken = cancellationToken };

        Parallel.For(0, Environment.ProcessorCount, parallelOptions, (_, state) =>
        {
            int loopId = Interlocked.Increment(ref id);
            TestContext.Out.WriteLine($"T-ID (entry): {Environment.CurrentManagedThreadId,2}, Loop-ID: {loopId,2}");

            cde.Signal();
            mre.Wait();

            for (int i = 0; i < 100; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (state.ShouldExitCurrentIteration)
                {
                    break;
                }

                byte[] actualText = DrawText();

                AssertSvg(s_expectedSvgDataText, s_expectedSvgStringText, actualText);
            }

            TestContext.Out.WriteLine($"T-ID (exit): {Environment.CurrentManagedThreadId,2}, Loop-ID: {loopId,2}");
        });

        await managerTask;
    }
    //-------------------------------------------------------------------------
    [Test, CancelAfter(1_500)]
    [Repeat(10)]    // test is flaky
    public async Task Font_create_in_one_thread_and_dispose_in_another_thread(CancellationToken cancellationToken)
    {
        int count                      = Environment.ProcessorCount;
        using CountdownEvent cde       = new(count);
        using ManualResetEventSlim mre = new();

        Task loopTask = Parallel.ForAsync(0, count, async (_, ct) =>
        {
            cde.Signal();
            mre.Wait(ct);

            TestContext.Out.WriteLine($"T-ID: {Environment.CurrentManagedThreadId,2}, create font");
            FreeTypeFont sanRemoFont = LoadFreeTypeFontFromFile("SanRemo.ttf");

            await Task.Run(sanRemoFont.Dispose, ct);
        });

        cde.Wait(cancellationToken);
        mre.Set();

        await loopTask;
    }
    //-------------------------------------------------------------------------
    private static void AssertSvg(ReadOnlySpan<byte> expectedData, string expectedString, byte[] actual)
    {
        if (expectedData.SequenceEqual(actual))
        {
            return;
        }

        string actualSvg = Encoding.UTF8.GetString(actual).Replace("\r\n", "\n");
        Assert.That(actualSvg, Is.EqualTo(expectedString));
    }
    //-------------------------------------------------------------------------
    private static byte[] DrawFontOptions()
    {
        // Code is taken from the FreeTypeDemo, just with only SVG surface (instead of Tee with PDF, and PNG).

        const double FontSize = 36;
        const double PaddingX = 10;
        const double PaddingY = FontSize / 2;

        // Just to get the bounding box in a lazy way...
        using RecordingSurface recordingSurface = new();
        using (CairoContext cr                  = new(recordingSurface))
        {
            Core(cr);
        }

        Rectangle surfaceExtents = recordingSurface.GetInkExtents();
        double width             = surfaceExtents.Width  + 2 * PaddingX;
        double height            = surfaceExtents.Height + 2 * PaddingY;

        MemoryStream resultStream = new();
        using (SvgSurface svg     = new(resultStream, width, height))
        using (CairoContext cr    = new(svg))
        {
            cr.Color = KnownColors.White;
            cr.Paint();

            cr.Color = Color.Default;
            cr.Rectangle(0, 0, width, height);
            cr.Stroke();

            // Due to font synthesizing / font options can't use the recording surface
            // as source surface, instead we need to draw it again manually.
            // I don't know if this is by design or a bug in cairo.
            Core(cr);
        }

        return resultStream.ToArray();
        //---------------------------------------------------------------------
        static void Core(CairoContext cr)
        {
            cr.SetFontSize(FontSize);

            cr.Translate(PaddingX, PaddingY);
            double curY = 0;

            using (FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("SplineSans-Regular.otf"))
            {
                cr.FontFace = freeTypeFont;

                ReadOnlySpan<byte> text = "SplineSans regular, no options set"u8;
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }

            using (FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("SplineSans-Regular.otf"))
            using (freeTypeFont.SetSynthesize(Synthesize.Bold | Synthesize.Oblique))
            {
                cr.FontFace = freeTypeFont;

                ReadOnlySpan<byte> text = "SplineSans regular, synthesized bold | oblique"u8;
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }

            using FontOptions defaultFontOptions = new();
            cr.FontFace                          = DefaultFonts.SansSerif;

            using (FontOptions fontOptions = defaultFontOptions.Copy())
            {
                fontOptions.Antialias = Antialias.Best;     // for pixel graphics only
                fontOptions.HintStyle = HintStyle.Full;

                cr.SetFontOptions(fontOptions);

                ReadOnlySpan<byte> text = "Helvetica, anti-alias best, hint-style full"u8;
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }

            cr.SetFontOptions(defaultFontOptions);

            using (FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf"))
            {
                cr.FontFace = freeTypeFont;

                ReadOnlySpan<byte> text = "Fraunces variable font, default"u8;
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }

            foreach (int fontWeight in (ReadOnlySpan<int>)[100, 200, 400, 600, 800, 1000])
            {
                using FreeTypeFont freeTypeFont = LoadFreeTypeFontFromFile("Fraunces-VariableFont_SOFT,WONK,opsz,wght.ttf");
                using FontOptions fontOptions   = new();

                // Settings on the font options must be set completely before assigning them to cr!
                fontOptions.Variations = $"wght={fontWeight}";

                cr.FontFace = freeTypeFont;
                cr.SetFontOptions(fontOptions);

                string text = $"Fraunces variable font, wght={fontWeight}";
                cr.TextExtents(text, out TextExtents textExtents);
                cr.MoveTo(0, curY + textExtents.Height);
                cr.ShowTextGlyphs(text);

                curY += textExtents.Height + PaddingY;
            }
        }
    }
    //-------------------------------------------------------------------------
    private static byte[] DrawText()
    {
        const double Size = 500;

        MemoryStream resultStream = new();
        using (SvgSurface svg     = new(resultStream, Size, Size))
        using (CairoContext cr    = new(svg))
        {
            cr.FontFace = DefaultFonts.SansSerifBold;
            cr.SetFontSize(64);
            (_, double y) = cr.TextAlignCenter("CairoSharp"u8, Size, Size, out TextExtents textExtents, moveCurrentPoint: true);
            cr.ShowText("CairoSharp"u8);

            using FreeTypeFont sanRemoFont = LoadFreeTypeFontFromFile("SanRemo.ttf");
            using (sanRemoFont.SetSynthesize(Synthesize.Bold))
            {
                ReadOnlySpan<byte> text = "<3 cairo"u8;

                cr.FontFace   = sanRemoFont;
                (double x, _) = cr.TextAlignCenter(text, Size, Size, out textExtents);
                cr.FontExtents(out FontExtents fontExtents);
                cr.MoveTo(x, y + fontExtents.Height);
                cr.TextPath(text);

                cr.Color = KnownColors.Blue;
                cr.FillPreserve();
                cr.Color = KnownColors.Yellow;
                cr.Stroke();
            }
        }

        return resultStream.ToArray();
    }
    //-------------------------------------------------------------------------
    private static FreeTypeFont LoadFreeTypeFontFromFile(string fontName) => FreeTypeFont.LoadFromFile($"Fonts/fonts/{fontName}");
}
