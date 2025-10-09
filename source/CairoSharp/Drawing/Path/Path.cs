// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Cairo.Drawing.Path.PathNative;

namespace Cairo.Drawing.Path;

/// <summary>
/// Paths — Creating paths and manipulating path data
/// </summary>
/// <remarks>
/// Paths are the most basic drawing tools and are primarily used to implicitly generate simple masks.
/// </remarks>
public sealed unsafe class Path : CairoObject
{
    internal Path(PathRaw* handle) : base(handle) { }

    protected override void DisposeCore(void* handle) => cairo_path_destroy((PathRaw*)handle);

    public PathIterator GetEnumerator() => new((PathRaw*)this.Handle);

    // https://www.cairographics.org/manual/bindings-path.html
    public struct PathIterator
    {
        private readonly PathRaw* _path;
        private Buffer            _buffer;
        private int               _i;

        internal PathIterator(PathRaw* path)
        {
            _path   = path;
            _i      = -1;
            _buffer = default;
        }

        private ReadOnlySpan<PointD> BufferAsSpan(int length)
        {
            ref PointD p = ref Unsafe.As<Buffer, PointD>(ref Unsafe.AsRef(ref _buffer));
            return MemoryMarshal.CreateReadOnlySpan(ref p, length);
        }

        public PathElement Current
        {
            get
            {
                if (_i < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext() before accessing the first element");
                }

                PathData* data     = &_path->Data[_i];
                DataType dataType  = data->Header.Type;

                switch (dataType)
                {
                    case DataType.MoveTo:
                    case DataType.LineTo:
                    {
                        _buffer[0] = new PointD(data[1].Point.X, data[1].Point.Y);
                        return new PathElement(dataType, this.BufferAsSpan(1));
                    }
                    case DataType.CurveTo:
                    {
                        _buffer[0] = new PointD(data[1].Point.X, data[1].Point.Y);
                        _buffer[1] = new PointD(data[2].Point.X, data[2].Point.Y);
                        _buffer[2] = new PointD(data[3].Point.X, data[3].Point.Y);
                        return new PathElement(dataType, this.BufferAsSpan(3));
                    }
                    case DataType.ClosePath:
                    {
                        return new PathElement(dataType, []);
                    }
                    default: throw new InvalidOperationException("should not be here");
                }
            }
        }

        public bool MoveNext()
        {
            if (_i < 0)
            {
                _i = 0;
            }
            else
            {
                _i += _path->Data[_i].Header.Length;
            }

            return _i < _path->Count;
        }

        [InlineArray(3)]
        private struct Buffer
        {
            private PointD _el;
        }
    }

    public readonly ref struct PathElement
    {
        public DataType DataType           { get; }
        public ReadOnlySpan<PointD> Points { get; }

        internal PathElement(DataType dataType, ReadOnlySpan<PointD> points)
        {
            this.DataType = dataType;
            this.Points   = points;
        }
    }
}
