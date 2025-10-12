// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Fonts.FreeType;

[Serializable]
public class FreeTypeException : Exception
{
    public FTError ErrorCode { get; }

    public FreeTypeException() { }
    public FreeTypeException(string message) : base(message) { }
    public FreeTypeException(string message, Exception inner) : base(message, inner) { }

    // "generic failure" reminds on some older software, can't remember which one exactly...
    public FreeTypeException(FTError errorCode) : this(errorCode.GetString() ?? "generic failure") => this.ErrorCode = errorCode;
}
