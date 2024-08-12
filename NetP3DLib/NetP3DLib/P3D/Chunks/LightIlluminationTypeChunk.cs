using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Light_Illumination_Type)]
public class LightIlluminationTypeChunk : Chunk
{
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

    public LightIlluminationTypeChunk(BinaryReader br) : base((uint)ChunkIdentifier.Light_Illumination_Type)
    {
        IlluminationType = (IlluminationTypes)br.ReadUInt32();
    }

    public LightIlluminationTypeChunk(IlluminationTypes illuminationType) : base((uint)ChunkIdentifier.Light_Illumination_Type)
    {
        IlluminationType = illuminationType;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write((uint)IlluminationType);
    }
}