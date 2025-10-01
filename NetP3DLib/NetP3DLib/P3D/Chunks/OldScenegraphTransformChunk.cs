using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphTransformChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Scenegraph_Transform;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) * 16;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldScenegraphTransformChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var numChildren = br.ReadUInt32();
        Transform = br.ReadMatrix4x4();
    }

    public OldScenegraphTransformChunk(string name, Matrix4x4 transform) : base(ChunkID)
    {
        Name = name;
        Transform = transform;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumChildren);
        bw.Write(Transform);
    }

    protected override Chunk CloneSelf() => new OldScenegraphTransformChunk(Name, Transform);
}