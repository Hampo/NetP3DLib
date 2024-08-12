using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Trigger_Volume)]
public class TriggerVolumeChunk : NamedChunk
{
    public uint IsRect { get; set; }
    public Vector3 HalfExtents { get; set; }
    public Matrix4x4 Matrix { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(IsRect));
            data.AddRange(BinaryExtensions.GetBytes(HalfExtents));
            data.AddRange(BinaryExtensions.GetBytes(Matrix));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 16;

    public TriggerVolumeChunk(BinaryReader br) : base((uint)ChunkIdentifier.Trigger_Volume)
    {
        Name = br.ReadP3DString();
        IsRect = br.ReadUInt32();
        HalfExtents = br.ReadVector3();
        Matrix = br.ReadMatrix4x4();
    }

    public TriggerVolumeChunk(string name, uint isRect, Vector3 halfExtents, Matrix4x4 matrix) : base((uint)ChunkIdentifier.Trigger_Volume)
    {
        Name = name;
        IsRect = isRect;
        HalfExtents = halfExtents;
        Matrix = matrix;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(IsRect);
        bw.Write(HalfExtents);
        bw.Write(Matrix);
    }
}