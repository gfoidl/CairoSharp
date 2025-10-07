// (c) gfoidl, all rights reserved

using System.Net.Mime;
using Cairo;
using Cairo.Drawing.Patterns;
using Cairo.Surfaces;
using Cairo.Surfaces.SVG;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AllowSynchronousIO = true);

WebApplication app = builder.Build();

app.MapGet("/", GetSvg);

app.Run();

static Task GetSvg(HttpResponse response)
{
    response.StatusCode  = 200;
    response.ContentType = MediaTypeNames.Image.Svg;

    DrawSvg(response.Body);

    return response.Body.FlushAsync();
}

static void DrawSvg(Stream stream, int size = 500)
{
    using SvgSurface surface   = new(stream, size, size);
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

    context.Source = radpat;
    context.Fill();

    using Gradient linpat = new LinearGradient(0.25, 0.35, 0.75, 0.65);
    linpat.AddColorStop(0.00, new Color(1, 1, 1, 0));
    linpat.AddColorStop(0.25, new Color(0, 1, 0, 0.5));
    linpat.AddColorStop(0.50, new Color(1, 1, 1, 0));
    linpat.AddColorStop(0.75, new Color(0, 0, 1, 0.5));
    linpat.AddColorStop(1.00, new Color(1, 1, 1, 0));

    context.Rectangle(0, 0, 1, 1);
    context.Source = linpat;
    context.Fill();
}
