using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FollowCameraDataChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Follow_Camera_Data;
    
    public uint Index { get; set; }
    public float Rotation { get; set; }
    public float Elevation { get; set; }
    public float Magnitude { get; set; }
    public Vector3 TargetOffset { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Index));
            data.AddRange(BitConverter.GetBytes(Rotation));
            data.AddRange(BitConverter.GetBytes(Elevation));
            data.AddRange(BitConverter.GetBytes(Magnitude));
            data.AddRange(BinaryExtensions.GetBytes(TargetOffset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) * 3;

    public FollowCameraDataChunk(BinaryReader br) : base(ChunkID)
    {
        Index = br.ReadUInt32();
        Rotation = br.ReadSingle();
        Elevation = br.ReadSingle();
        Magnitude = br.ReadSingle();
        TargetOffset = br.ReadVector3();
    }

    public FollowCameraDataChunk(uint index, float rotation, float elevation, float magnitude, Vector3 targetOffset) : base(ChunkID)
    {
        Index = index;
        Rotation = rotation;
        Elevation = elevation;
        Magnitude = magnitude;
        TargetOffset = targetOffset;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Index);
        bw.Write(Rotation);
        bw.Write(Elevation);
        bw.Write(Magnitude);
        bw.Write(TargetOffset);
    }

    internal override Chunk CloneSelf() => new FollowCameraDataChunk(Index, Rotation, Elevation, Magnitude, TargetOffset);
}