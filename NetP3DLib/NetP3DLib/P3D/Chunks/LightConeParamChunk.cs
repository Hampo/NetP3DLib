using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightConeParamChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Light_Cone_Param;
    
    public float Phi { get; set; }
    public float Theta { get; set; }
    public float Falloff { get; set; }
    public float Range { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Phi));
            data.AddRange(BitConverter.GetBytes(Theta));
            data.AddRange(BitConverter.GetBytes(Falloff));
            data.AddRange(BitConverter.GetBytes(Range));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float);

    public LightConeParamChunk(BinaryReader br) : base(ChunkID)
    {
        Phi = br.ReadSingle();
        Theta = br.ReadSingle();
        Falloff = br.ReadSingle();
        Range = br.ReadSingle();
    }

    public LightConeParamChunk(float phi, float theta, float falloff, float range) : base(ChunkID)
    {
        Phi = phi;
        Theta = theta;
        Falloff = falloff;
        Range = range;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Phi);
        bw.Write(Theta);
        bw.Write(Falloff);
        bw.Write(Range);
    }
}