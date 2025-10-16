// (c) gfoidl, all rights reserved

using System.Diagnostics;
using static Cairo.Surfaces.Images.PngSupportNative;

namespace Cairo.Surfaces.Images;

internal static unsafe class PngHelper
{
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    public static cairo_surface_t* CreateForPngData(ReadOnlySpan<byte> pngData)
    {
        ReadState readState        = new(pngData);
        cairo_read_func_t readFunc = &ReadFunc;

        return cairo_image_surface_create_from_png_stream(readFunc, &readState);

        static Status ReadFunc(void* closure, byte* bufferData, uint bufferLength)
        {
            Debug.WriteLine($"closure: 0x{(nint)closure:x2}\tbuffer: 0x{(nint)bufferData:x2}\tlen: {bufferLength}");

            Span<byte> buffer      = new(bufferData, (int)bufferLength);
            ReadState* readState   = (ReadState*)closure;
            ReadOnlySpan<byte> png = readState->PngData[readState->Read..];

            if (png.Length > bufferLength)
            {
                png = png[..(int)bufferLength];
            }

            png.CopyTo(buffer);
            readState->Read += (int)bufferLength;

            return Status.Success;
        }
    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

    private ref struct ReadState(ReadOnlySpan<byte> pngData)
    {
        public ReadOnlySpan<byte> PngData = pngData;
        public int Read;
    }
}
