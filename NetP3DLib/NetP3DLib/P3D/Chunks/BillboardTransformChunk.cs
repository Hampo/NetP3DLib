using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Billboard_Transform)]
public class BillboardTransformChunk : Chunk
{
    public uint Version { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Translation { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetBytes(Rotation));
            data.AddRange(BinaryExtensions.GetBytes(Translation));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 4 + sizeof(float) * 3;

    public BillboardTransformChunk(BinaryReader br) : base((uint)ChunkIdentifier.Billboard_Transform)
    {
        Version = br.ReadUInt32();
        Rotation = br.ReadQuaternion();
        Translation = br.ReadVector3();
    }

    public BillboardTransformChunk(uint version, Quaternion rotation, Vector3 translation) : base((uint)ChunkIdentifier.Billboard_Transform)
    {
        Version = version;
        Rotation = rotation;
        Translation = translation;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Rotation);
        bw.Write(Translation);
    }
}