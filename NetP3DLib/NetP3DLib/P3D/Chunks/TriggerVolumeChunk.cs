using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TriggerVolumeChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Trigger_Volume;
    
    public enum Types
    {
        Sphere,
        Rectangle
    }

    public Types Type { get; set; }
    public Vector3 HalfExtents { get; set; }
    public Matrix4x4 Matrix { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes((uint)Type));
            data.AddRange(BinaryExtensions.GetBytes(HalfExtents));
            data.AddRange(BinaryExtensions.GetBytes(Matrix));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 16;

    public TriggerVolumeChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Type = (Types)br.ReadUInt32();
        HalfExtents = br.ReadVector3();
        Matrix = br.ReadMatrix4x4();
    }

    public TriggerVolumeChunk(string name, Types type, Vector3 halfExtents, Matrix4x4 matrix) : base(ChunkID)
    {
        Name = name;
        Type = type;
        HalfExtents = halfExtents;
        Matrix = matrix;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write((uint)Type);
        bw.Write(HalfExtents);
        bw.Write(Matrix);
    }

    protected override Chunk CloneSelf() => new TriggerVolumeChunk(Name, Type, HalfExtents, Matrix);
}