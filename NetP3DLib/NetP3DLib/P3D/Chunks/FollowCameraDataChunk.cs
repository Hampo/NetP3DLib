using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Follow_Camera_Data)]
public class FollowCameraDataChunk : Chunk
{
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

    public FollowCameraDataChunk(BinaryReader br) : base((uint)ChunkIdentifier.Follow_Camera_Data)
    {
        Index = br.ReadUInt32();
        Rotation = br.ReadSingle();
        Elevation = br.ReadSingle();
        Magnitude = br.ReadSingle();
        TargetOffset = br.ReadVector3();
    }

    public FollowCameraDataChunk(uint index, float rotation, float elevation, float magnitude, Vector3 targetOffset) : base((uint)ChunkIdentifier.Follow_Camera_Data)
    {
        Index = index;
        Rotation = rotation;
        Elevation = elevation;
        Magnitude = magnitude;
        TargetOffset = targetOffset;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Index);
        bw.Write(Rotation);
        bw.Write(Elevation);
        bw.Write(Magnitude);
        bw.Write(TargetOffset);
    }
}