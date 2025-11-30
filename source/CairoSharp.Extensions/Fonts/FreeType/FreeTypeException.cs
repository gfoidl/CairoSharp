// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Fonts.FreeType;

[Serializable]
public class FreeTypeException : Exception
{
    public FTError ErrorCode { get; }

    public FreeTypeException() { }
    public FreeTypeException(string message) : base(message) { }
    public FreeTypeException(string message, Exception inner) : base(message, inner) { }

    public FreeTypeException(FTError errorCode) : this(errorCode.GetString() ?? $"FTError: {errorCode}") => this.ErrorCode = errorCode;
}
