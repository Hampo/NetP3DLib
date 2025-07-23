using System;
using System.Text;

namespace NetP3DLib.P3D.Exceptions;
internal class InvalidP3DLongStringException : Exception
{
    public string Variable { get; }
    public string Value { get; }
    public int ByteCount { get; }

    public InvalidP3DLongStringException(string variable, string str) : base($"Invalid P3D string for \"{variable}\". Strings cannot be null and must not exceed {int.MaxValue - 4} bytes.")
    {
        Value = str;
        ByteCount = Value is null ? 0 : Encoding.UTF8.GetByteCount(str);
    }
}
