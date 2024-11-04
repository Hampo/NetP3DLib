using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class WalkerCameraDataChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Walker_Camera_Data;
    
    public uint Index { get; set; }
    public float MinMagnitude { get; set; }
    public float MaxMagnitude { get; set; }
    public float Elevation { get; set; }
    public Vector3 TargetOffset { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Index));
            data.AddRange(BitConverter.GetBytes(MinMagnitude));
            data.AddRange(BitConverter.GetBytes(MaxMagnitude));
            data.AddRange(BitConverter.GetBytes(Elevation));
            data.AddRange(BinaryExtensions.GetBytes(TargetOffset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) * 3;

    public WalkerCameraDataChunk(BinaryReader br) : base(ChunkID)
    {
        Index = br.ReadUInt32();
        MinMagnitude = br.ReadSingle();
        MaxMagnitude = br.ReadSingle();
        Elevation = br.ReadSingle();
        TargetOffset = br.ReadVector3();
    }

    public WalkerCameraDataChunk(uint index, float minMagnitude, float maxMagnitude, float elevation, Vector3 targetOffset) : base(ChunkID)
    {
        Index = index;
        MinMagnitude = minMagnitude;
        MaxMagnitude = maxMagnitude;
        Elevation = elevation;
        TargetOffset = targetOffset;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Index);
        bw.Write(MinMagnitude);
        bw.Write(MaxMagnitude);
        bw.Write(Elevation);
        bw.Write(TargetOffset);
    }
}