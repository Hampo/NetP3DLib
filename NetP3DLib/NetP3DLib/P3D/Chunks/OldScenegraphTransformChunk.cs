using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Scenegraph_Transform)]
public class OldScenegraphTransformChunk : NamedChunk
{
    public uint NumChildren => (uint)Children.Count;
    public Matrix4x4 Transform { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumChildren));
            data.AddRange(BinaryExtensions.GetBytes(Transform));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) * 16;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldScenegraphTransformChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Scenegraph_Transform)
    {
        Name = br.ReadP3DString();
        var numChildren = br.ReadUInt32();
        Transform = br.ReadMatrix4x4();
    }

    public OldScenegraphTransformChunk(string name, Matrix4x4 transform) : base((uint)ChunkIdentifier.Old_Scenegraph_Transform)
    {
        Name = name;
        Transform = transform;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumChildren);
        bw.Write(Transform);
    }
}