using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBillboardQuadGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Billboard_Quad_Group;
    
    public uint Version { get; set; }
    public string Shader { get; set; }
    public uint ZTest { get; set; }
    public uint ZWrite { get; set; }
    public uint Fog { get; set; }
    public uint NumQuads => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Old_Billboard_Quad).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Shader));
            data.AddRange(BitConverter.GetBytes(ZTest));
            data.AddRange(BitConverter.GetBytes(ZWrite));
            data.AddRange(BitConverter.GetBytes(Fog));
            data.AddRange(BitConverter.GetBytes(NumQuads));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(Shader) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldBillboardQuadGroupChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Shader = br.ReadP3DString();
        ZTest = br.ReadUInt32();
        ZWrite = br.ReadUInt32();
        Fog = br.ReadUInt32();
        var numQuads = br.ReadUInt32();
    }

    public OldBillboardQuadGroupChunk(uint version, string name, string shader, uint zTest, uint zWrite, uint fog) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Shader = shader;
        ZTest = zTest;
        ZWrite = zWrite;
        Fog = fog;
    }

    public override void Validate()
    {
        if (!Shader.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Shader), Shader);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(Shader);
        bw.Write(ZTest);
        bw.Write(ZWrite);
        bw.Write(Fog);
        bw.Write(NumQuads);
    }
}