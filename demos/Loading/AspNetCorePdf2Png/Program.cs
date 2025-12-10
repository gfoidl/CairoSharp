// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices;
using Cairo.Extensions.Loading;
using Cairo.Extensions.Loading.PDF;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using IOPath = System.IO.Path;

FixupEnvironment();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app            = builder.Build();

app.MapGet(""      , GetPreviewOfPdfPageDirect);
app.MapGet("stream", GetPreviewOfPdfPageViaStream);

app.Run();
//-----------------------------------------------------------------------------
static async ValueTask GetPreviewOfPdfPageDirect(HttpResponse response, int page = 0)
{
    try
    {
        string path           = IOPath.Combine(AppContext.BaseDirectory, "Sample_two_page_pager.pdf");
        using PdfDocument pdf = new(path);

        pdf.ValidatePageIndex(page);

        response.StatusCode  = 200;
        response.ContentType = MediaTypeNames.Image.Png;

        pdf.RenderToPng(response.BodyWriter, page);

        _ = await response.BodyWriter.FlushAsync();
    }
    catch (PopplerException ex)
    {
        response.StatusCode = StatusCodes.Status400BadRequest;
        await response.WriteAsync(ex.Message);
    }
}
//-----------------------------------------------------------------------------
static Results<PushStreamHttpResult, BadRequest<string>> GetPreviewOfPdfPageViaStream(HttpResponse response, int page = 0)
{
    IHttpBodyControlFeature? httpBodyControlFeature = response.HttpContext.Features.Get<IHttpBodyControlFeature>();
    Debug.Assert(httpBodyControlFeature is not null);
    httpBodyControlFeature.AllowSynchronousIO = true;

    string path       = IOPath.Combine(AppContext.BaseDirectory, "Sample_two_page_pager.pdf");
    PdfDocument pdf   = new(path);
    int numberOfPages = pdf.NumberOfPages;

    if (page >= numberOfPages)
    {
        return TypedResults.BadRequest($"Pages in PDF = {numberOfPages}, page = {page}");
    }

    return TypedResults.Stream(stream =>
    {
        pdf.RenderToPng(stream, page);
        pdf.Dispose();
        return Task.CompletedTask;
    }, MediaTypeNames.Image.Png);
}
//-----------------------------------------------------------------------------
static void FixupEnvironment()
{
    if (OperatingSystem.IsWindows())
    {
        LoadingNative.DllImportResolver = static (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
        {
            string? path = libraryName switch
            {
                // For simplicity we re-use the DLLs from Inkscape.
                // Could also be C:\Program Files\msys64\ucrt64\bin, cf. https://www.gtk.org/docs/installations/windows/#using-gtk-from-msys2-packages
                LoadingNative.LibPopplerName => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libpoppler-glib-8.dll"),
                LoadingNative.LibGLibName    => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libglib-2.0-0.dll"),
                LoadingNative.LibGObjectName => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgobject-2.0-0.dll"),
                LoadingNative.LibGioName     => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgio-2.0-0.dll"),
                _                            => null
            };

            if (path is not null && NativeLibrary.TryLoad(path, out nint handle))
            {
                return handle;
            }

            return default;
        };
    }
}
