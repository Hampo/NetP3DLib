using System;

namespace NetP3DLib.P3D;

[AttributeUsage(AttributeTargets.Class)]
public class ChunkAttributes : Attribute
{
    public uint Identifier { get; }

    public ChunkAttributes(uint identifier)
    {
        Identifier = identifier;
    }
}
