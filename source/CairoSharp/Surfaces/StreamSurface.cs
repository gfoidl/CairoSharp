// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using unsafe NativeFactory = delegate*<delegate*<void*, byte*, uint, Cairo.Status>, void*, double, double, Cairo.Surfaces.cairo_surface_t*>;

namespace Cairo.Surfaces;

/// <summary>
/// Base class for <see cref="Surface"/> that allow stream operations.
/// </summary>
public abstract unsafe class StreamSurface : Surface
{
    protected GCHandle _stateHandle;        // mutable struct

    protected StreamSurface(cairo_surface_t* surface, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(surface, isOwnedByCairo, needsDestroy) { }

    protected StreamSurface(State state, bool isOwnedByCairo = false)
        : base(state.Surface, isOwnedByCairo)
        => _stateHandle = state.StateHandle;

    protected override void DisposeCore(cairo_surface_t* surface)
    {
        base.DisposeCore(surface);

        // Need to free the surface first, so that the write function (if any)
        // can be called on a valid handle.
        // So it's like: dispose -> write func -> stream handle free
        if (_stateHandle.IsAllocated)
        {
            _stateHandle.Free();
        }
    }

    protected static State CreateForWriteStream(Stream stream, double width, double height, NativeFactory factory)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanWrite)
        {
            throw new ArgumentException("Stream must be writeable");
        }

        GCHandle streamHandle        = GCHandle.Alloc(stream, GCHandleType.Normal);
        void* state                  = GCHandle.ToIntPtr(streamHandle).ToPointer();
        cairo_write_func_t writeFunc = &WriteFunc;

        cairo_surface_t* handle = factory(writeFunc, state, width, height);

        return new State(handle, streamHandle);

        static Status WriteFunc(void* state, byte* data, uint length)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr((nint)state);
            Debug.Assert(gcHandle.IsAllocated);

            Stream? stream = gcHandle.Target as Stream;
            Debug.Assert(stream is not null);

            ReadOnlySpan<byte> span = new(data, (int)length);

            try
            {
                stream.Write(span);
            }
            catch
            {
                return Status.WriteError;
            }

            return Status.Success;
        }
    }

    protected static State CreateForDelegate<T>(T? obj, Callback<T> callback, double width, double height, NativeFactory factory)
        where T : class
    {
        CallbackState<T> callbackState = new(obj, callback);
        GCHandle stateHandle           = GCHandle.Alloc(callbackState, GCHandleType.Normal);
        void* state                    = GCHandle.ToIntPtr(stateHandle).ToPointer();
        cairo_write_func_t writeFunc   = &WriteFunc;

        cairo_surface_t* handle = factory(writeFunc, state, width, height);

        return new State(handle, stateHandle);

        static Status WriteFunc(void* state, byte* data, uint length)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr((nint)state);
            Debug.Assert(gcHandle.IsAllocated);

            CallbackState<T>? callbackState = gcHandle.Target as CallbackState<T>;
            Debug.Assert(callbackState is not null);

            ReadOnlySpan<byte> span = new(data, (int)length);

            try
            {
                callbackState.Callback(callbackState.State, span);
            }
            catch
            {
                return Status.WriteError;
            }

            return Status.Success;
        }
    }

    /// <summary>
    /// The callback to be invoked with the content to be written.
    /// </summary>
    /// <typeparam name="T">type of <paramref name="state"/></typeparam>
    /// <param name="state">state to be passed into the callback, as given in the constructor</param>
    /// <param name="data">the data to be written</param>
    public delegate void Callback<T>(T? state, ReadOnlySpan<byte> data);
    private record CallbackState<T>(T? State, Callback<T> Callback);

    protected readonly struct State(cairo_surface_t* surface, GCHandle stateHandle)
    {
        public cairo_surface_t* Surface { get; } = surface;
        public GCHandle StateHandle     { get; } = stateHandle;
    }
}
