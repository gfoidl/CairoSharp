// (c) gfoidl, all rights reserved

#define CAIRO_USE_CALLBACK

using System.Net.Mime;
using Cairo;
using Cairo.Drawing.Patterns;
using Cairo.Extensions;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Fonts;
using Cairo.Fonts;
using Cairo.Fonts.FreeType;
using Cairo.Surfaces;
using Cairo.Surfaces.SVG;

#if CAIRO_USE_CALLBACK
using System.IO.Pipelines;
#else
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
#endif

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app            = builder.Build();

app.MapGet("/", GetSvg);

app.Run();

static async ValueTask GetSvg(HttpResponse response, bool showText = false)
{
    response.StatusCode  = 200;
    response.ContentType = MediaTypeNames.Image.Svg;

#if CAIRO_USE_CALLBACK
    DrawSvgViaCallback(response.BodyWriter, showText);

    _ = await response.BodyWriter.FlushAsync();
#else
    IHttpBodyControlFeature? httpBodyControlFeature = response.HttpContext.Features.Get<IHttpBodyControlFeature>();
    Debug.Assert(httpBodyControlFeature is not null);

    httpBodyControlFeature.AllowSynchronousIO = true;

    DrawSvgViaStream(response.Body, showText);

    await response.Body.FlushAsync();
#endif
}

#if CAIRO_USE_CALLBACK
static void DrawSvgViaCallback(PipeWriter bodyWriter, bool showText, int size = 500)
{
    using SvgSurface surface = new(static (state, data) =>
    {
        PipeWriter writer = (state as PipeWriter)!;
        Span<byte> buffer = writer.GetSpan(data.Length);

        data.CopyTo(buffer);
        writer.Advance(data.Length);

    }, bodyWriter, size, size);

    DrawCore(surface, size, showText);
}
#else
static void DrawSvgViaStream(Stream stream, bool showText, int size = 500)
{
    using SvgSurface surface = new(stream, size, size);
    DrawCore(surface, size, showText);
}
#endif

static void DrawCore(SvgSurface surface, int size, bool showText)
{
    using CairoContext cr = new(surface);

    using (cr.Save())
    {
        cr.Rectangle(0, 0, size, size);
        cr.LineWidth = 5;
        cr.Stroke();
    }

    using (cr.Save())
    {
        cr.Scale(size, size);

        using Gradient radpat = new RadialGradient(0.25, 0.25, 0.1, 0.5, 0.5, 0.5);
        radpat.AddColorStop(0, new Color(1.0, 0.8, 0.8));
        radpat.AddColorStop(1, new Color(0.9, 0.0, 0.0));

        for (int i = 1; i < 10; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                cr.Rectangle(i / 10d - 0.04, j / 10d - 0.04, 0.08, 0.08);
            }
        }

        cr.SetSource(radpat);
        cr.Fill();

        using Gradient linpat = new LinearGradient(0.25, 0.35, 0.75, 0.65);
        linpat.AddColorStop(0.00, new Color(1, 1, 1, 0));
        linpat.AddColorStop(0.25, new Color(0, 1, 0, 0.5));
        linpat.AddColorStop(0.50, new Color(1, 1, 1, 0));
        linpat.AddColorStop(0.75, new Color(0, 0, 1, 0.5));
        linpat.AddColorStop(1.00, new Color(1, 1, 1, 0));

        cr.Rectangle(0, 0, 1, 1);
        cr.SetSource(linpat);
        cr.Fill();
    }

    if (showText)
    {
        cr.FontFace = DefaultFonts.SansSerifBold;
        cr.SetFontSize(64);
        (_, double y) = cr.TextAlignCenter("CairoSharp"u8, size, size, out TextExtents textExtents, moveCurrentPoint: true);
        cr.ShowText("CairoSharp"u8);

        string fontFile                = Path.Combine(AppContext.BaseDirectory, "fonts", "SanRemo.ttf");
        using FreeTypeFont sanRemoFont = FreeTypeFont.LoadFromFile(fontFile);
        using (sanRemoFont.SetSynthesize(Synthesize.Bold))
        {
            ReadOnlySpan<byte> text = "<3 cairo"u8;

            cr.FontFace   = sanRemoFont;
            (double x, _) = cr.TextAlignCenter(text, size, size, out textExtents);
            cr.FontExtents(out FontExtents fontExtents);
            cr.MoveTo(x, y + fontExtents.Height);
            cr.TextPath(text);

            cr.Color = KnownColors.Blue;
            cr.FillPreserve();
            cr.Color = KnownColors.Yellow;
            cr.Stroke();
        }
    }
}
