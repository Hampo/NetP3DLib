using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LensFlareGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Lens_Flare_Group;
    
    public uint Version { get; set; }
    public string ShaderName { get; set; }
    public uint ZTest { get; set; }
    public uint ZWrite { get; set; }
    public uint Fog { get; set; }
    public float SourceRadius { get; set; }
    public float EdgeRadius { get; set; }
    public uint NumLensFlares => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Lens_Flare).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ShaderName));
            data.AddRange(BitConverter.GetBytes(ZTest));
            data.AddRange(BitConverter.GetBytes(ZWrite));
            data.AddRange(BitConverter.GetBytes(Fog));
            data.AddRange(BitConverter.GetBytes(SourceRadius));
            data.AddRange(BitConverter.GetBytes(EdgeRadius));
            data.AddRange(BitConverter.GetBytes(NumLensFlares));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(ShaderName) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public LensFlareGroupChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        ShaderName = br.ReadP3DString();
        ZTest = br.ReadUInt32();
        ZWrite = br.ReadUInt32();
        Fog = br.ReadUInt32();
        SourceRadius = br.ReadSingle();
        EdgeRadius = br.ReadSingle();
        var numLensFlares = br.ReadUInt32();
    }

    public LensFlareGroupChunk(uint version, string name, string shaderName, uint zTest, uint zWrite, uint fog, float sourceRadius, float edgeRadius) : base(ChunkID)
    {
        Version = version;
        Name = name;
        ShaderName = shaderName;
        ZTest = zTest;
        ZWrite = zWrite;
        Fog = fog;
        SourceRadius = sourceRadius;
        EdgeRadius = edgeRadius;
    }

    public override void Validate()
    {
        if (!ShaderName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(ShaderName), ShaderName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(ShaderName);
        bw.Write(ZTest);
        bw.Write(ZWrite);
        bw.Write(Fog);
        bw.Write(SourceRadius);
        bw.Write(EdgeRadius);
        bw.Write(NumLensFlares);
    }

    internal override Chunk CloneSelf() => new LensFlareGroupChunk(Version, Name, ShaderName, ZTest, ZWrite, Fog, SourceRadius, EdgeRadius);
}