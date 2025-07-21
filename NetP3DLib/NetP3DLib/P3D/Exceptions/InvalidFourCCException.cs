using System;
using System.Text;

namespace NetP3DLib.P3D.Exceptions;
internal class InvalidFourCCException : Exception
{
    public string Variable { get; }
    public string Value { get; }
    public int ByteCount { get; }

    public InvalidFourCCException(string variable, string str) : base($"Invalid FourCC for \"{variable}\". FourCCs cannot be null and must not exceed 4 bytes.")
    {
        Value = str;
        ByteCount = Value is null ? 0 : Encoding.UTF8.GetByteCount(str);
    }
}
