using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightIlluminationTypeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Light_Illumination_Type;
    
    public enum IlluminationTypes : uint
    {
        ZeroIlluminant = 1,
        NegativeIlluminant = 2,
        PositiveIlluminant = 3,
    }

    public IlluminationTypes IlluminationType { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes((uint)IlluminationType));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    public LightIlluminationTypeChunk(BinaryReader br) : base(ChunkID)
    {
        IlluminationType = (IlluminationTypes)br.ReadUInt32();
    }

    public LightIlluminationTypeChunk(IlluminationTypes illuminationType) : base(ChunkID)
    {
        IlluminationType = illuminationType;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write((uint)IlluminationType);
    }

    protected override Chunk CloneSelf() => new LightIlluminationTypeChunk(IlluminationType);
}