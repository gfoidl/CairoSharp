// (c) gfoidl, all rights reserved

#define CAIRO_USE_CALLBACK

using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Mime;
using Cairo;
using Cairo.Drawing.Patterns;
using Cairo.Surfaces;
using Cairo.Surfaces.SVG;
using Microsoft.AspNetCore.Http.Features;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app            = builder.Build();

app.MapGet("/", GetSvg);

app.Run();

static async ValueTask GetSvg(HttpResponse response)
{
    response.StatusCode  = 200;
    response.ContentType = MediaTypeNames.Image.Svg;

#if CAIRO_USE_CALLBACK
    DrawSvgViaCallback(response.BodyWriter);

    _ = await response.BodyWriter.FlushAsync();
#else
    IHttpBodyControlFeature? httpBodyControlFeature = response.HttpContext.Features.Get<IHttpBodyControlFeature>();
    Debug.Assert(httpBodyControlFeature is not null);

    httpBodyControlFeature.AllowSynchronousIO = true;

    DrawSvgViaStream(response.Body);

    await response.Body.FlushAsync();
#endif
}

#if CAIRO_USE_CALLBACK
static void DrawSvgViaCallback(PipeWriter bodyWriter, int size = 500)
{
    using SvgSurface surface = new(static (state, data) =>
    {
        PipeWriter writer = (state as PipeWriter)!;
        Span<byte> buffer = writer.GetSpan(data.Length);

        data.CopyTo(buffer);
        writer.Advance(data.Length);

    }, bodyWriter, size, size);

    DrawCore(surface, size);
}
#else
static void DrawSvgViaStream(Stream stream, int size = 500)
{
    using SvgSurface surface = new(stream, size, size);
    DrawCore(surface, size);
}
#endif

static void DrawCore(SvgSurface surface, int size)
{
    using CairoContext context = new(surface);

    using (context.Save())
    {
        context.Rectangle(0, 0, size, size);
        context.LineWidth = 5;
        context.Stroke();
    }

    context.Scale(size, size);

    using Gradient radpat = new RadialGradient(0.25, 0.25, 0.1, 0.5, 0.5, 0.5);
    radpat.AddColorStop(0, new Color(1.0, 0.8, 0.8));
    radpat.AddColorStop(1, new Color(0.9, 0.0, 0.0));

    for (int i = 1; i < 10; i++)
    {
        for (int j = 1; j < 10; j++)
        {
            context.Rectangle(i / 10d - 0.04, j / 10d - 0.04, 0.08, 0.08);
        }
    }

    context.SetSource(radpat);
    context.Fill();

    using Gradient linpat = new LinearGradient(0.25, 0.35, 0.75, 0.65);
    linpat.AddColorStop(0.00, new Color(1, 1, 1, 0));
    linpat.AddColorStop(0.25, new Color(0, 1, 0, 0.5));
    linpat.AddColorStop(0.50, new Color(1, 1, 1, 0));
    linpat.AddColorStop(0.75, new Color(0, 0, 1, 0.5));
    linpat.AddColorStop(1.00, new Color(1, 1, 1, 0));

    context.Rectangle(0, 0, 1, 1);
    context.SetSource(linpat);
    context.Fill();
}
