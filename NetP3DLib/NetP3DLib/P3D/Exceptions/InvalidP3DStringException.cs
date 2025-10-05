﻿using System;
using System.Text;

namespace NetP3DLib.P3D.Exceptions;
internal class InvalidP3DStringException : Exception
{
    public string Variable { get; }
    public string? Value { get; }
    public int ByteCount { get; }

    public InvalidP3DStringException(string variable, string? str) : base($"Invalid P3D string for \"{variable}\". Strings cannot be null and must not exceed 255 bytes.")
    {
        Variable = variable;
        Value = str;
        ByteCount = Value is null ? 0 : Encoding.UTF8.GetByteCount(str);
    }
}
