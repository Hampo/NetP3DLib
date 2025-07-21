using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class BillboardTransformChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Billboard_Transform;
    
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

    public BillboardTransformChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Rotation = br.ReadQuaternion();
        Translation = br.ReadVector3();
    }

    public BillboardTransformChunk(uint version, Quaternion rotation, Vector3 translation) : base(ChunkID)
    {
        Version = version;
        Rotation = rotation;
        Translation = translation;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Rotation);
        bw.Write(Translation);
    }
}