// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Cairo.Surfaces.Images.PngSupportNative;
using unsafe NativeFactory = delegate*<delegate*<void*, byte*, uint, Cairo.Status>, void*, double, double, void*>;

namespace Cairo.Surfaces;

internal static unsafe class StreamHelper
{
    public static (IntPtr, GCHandle) CreateForWriteStream(Stream stream, double width, double height, NativeFactory factory)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("Stream must be writeable");
        }

        GCHandle streamHandle        = GCHandle.Alloc(stream, GCHandleType.Normal);
        void* state                  = GCHandle.ToIntPtr(streamHandle).ToPointer();
        cairo_write_func_t writeFunc = &WriteFunc;

        void* handle = factory(writeFunc, state, width, height);

        return (new IntPtr(handle), streamHandle);

        static Status WriteFunc(void* state, byte* data, uint length)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr((nint)state);
            Debug.Assert(gcHandle.IsAllocated);

            Stream stream           = (Stream)gcHandle.Target!;
            ReadOnlySpan<byte> span = new(data, (int)length);

            stream.Write(span);

            return Status.Success;
        }
    }

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    public static void* CreateForPngData(ReadOnlySpan<byte> pngData)
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
