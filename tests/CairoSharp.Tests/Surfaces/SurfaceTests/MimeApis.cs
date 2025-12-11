// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cairo;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;

namespace CairoSharp.Tests.Surfaces.SurfaceTests;

[TestFixture]
public unsafe class MimeApis
{
    [Test]
    public void Set_and_get_mime_data___OK_and_destroy_func_is_called()
    {
        // Based on https://gitlab.freedesktop.org/cairo/cairo/-/blob/master/test/mime-surface-api.c?ref_type=heads

        const string MimeType    = "text/x-uri";
        ReadOnlySpan<byte> data1 = "https://www.cairographics.org"u8;
        ReadOnlySpan<byte> data2 = "https://cairographics.org/examples/"u8;
        bool destroy1Called      = false;
        bool destroy2Called      = false;

        using (ImageSurface surface = new(Format.Argb32, 0, 0))
        {
            CheckMimeData(surface, MimeType, default);

            SetAndCheckMimeData(surface, MimeType, data1, &destroy1Called);
            Assert.That(destroy1Called, Is.False, "MIME data 1 destroyed too early");

            SetAndCheckMimeData(surface, MimeType, data2, &destroy2Called);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(destroy1Called          , "MIME data 1 destroy callback not called");
                Assert.That(destroy2Called, Is.False, "MIME data 2 destroyed too early");
            }
        }

        Assert.That(destroy2Called, "MIME data 2 destroy callback not called");

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void MimeDataDestroyFunc(void* data)
        {
            *(bool*)data = true;
        }

        static void CheckMimeData(Surface surface, string mimeType, ReadOnlySpan<byte> data)
        {
            ReadOnlySpan<byte> actual = surface.GetMimeData(mimeType);

            Assert.That(actual.Length, Is.EqualTo(data.Length));

            // Address of the data is the same.
            ref byte dataPtr   = ref MemoryMarshal.GetReference(data);
            ref byte actualPtr = ref MemoryMarshal.GetReference(actual);

            Assert.That(Unsafe.AreSame(ref dataPtr, ref actualPtr));
        }

        static void SetAndCheckMimeData(Surface surface, string mimeType, ReadOnlySpan<byte> data, bool* destroyCalled)
        {
            surface.SetMimeData(mimeType, data, &MimeDataDestroyFunc, destroyCalled);
            CheckMimeData(surface, mimeType, data);
        }
    }
    //-------------------------------------------------------------------------
    [Test]
    public void Mime_data_embedding___OK()
    {
        // Based on https://gitlab.freedesktop.org/cairo/cairo/-/blob/master/test/mime-data.c?ref_type=heads

        using ImageSurface surface = new(Format.Argb32, 200, 300);
        using CairoContext cr      = new(surface);

        PaintFile(cr, "jpeg.jpg", MimeTypes.Jpeg, 0,   0);
        PaintFile(cr, "png.png" , MimeTypes.Png , 0,  50);
        PaintFile(cr, "jp2.jp2" , MimeTypes.Jp2 , 0, 100);

        static void PaintFile(CairoContext cr, string fileName, string mimeType, int x, int y)
        {
            // Deliberately use a non-matching MIME images, so that we can identify when the
            // MIME representation is used in preference to the plain image surface.

            byte[] mimeData = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "images", fileName));

            using ImageSurface image = new(Format.Rgb24, 200, 50);
            image.SetMimeData(mimeType, mimeData);

            cr.SetSourceSurface(image, x, y);
            cr.Paint();
        }
    }
}
