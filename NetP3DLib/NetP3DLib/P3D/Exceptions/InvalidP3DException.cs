using System;

namespace NetP3DLib.P3D.Exceptions;
public class InvalidP3DException : Exception
{
    public InvalidP3DException() : base() { }

    public InvalidP3DException(string message) : base(message) { }

    public InvalidP3DException(string message, Exception innerException) : base(message, innerException) { }
}
