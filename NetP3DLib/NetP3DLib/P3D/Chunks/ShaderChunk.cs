using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShaderChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shader;

    [Flags]
    public enum VertexMasks : uint
    {
        None = 0,
        UVs = 1,
        UVs2 = 2,
        UVs3 = 3,
        UVs4 = 4,
        UVs5 = 5,
        UVs6 = 6,
        UVs7 = 7,
        UVs8 = 8,
        Normals = 1 << 4,
        Colours = 1 << 5,
        Specular = 1 << 6,
        Matrices = 1 << 7,
        Weights = 1 << 8,
        Size = 1 << 9,
        W = 1 << 10,
        BiNormal = 1 << 11,
        Tangent = 1 << 12,
        Position = 1 << 13,
        Colours2 = 1 << 14,
        ColourCount1 = 1 << 15,
        ColourCount2 = 2 << 15,
        ColourCount3 = 3 << 15,
        ColourCount4 = 4 << 15,
        ColourCount5 = 5 << 15,
        ColourCount6 = 6 << 15,
        ColourCount7 = 7 << 15,
    }

    [DefaultValue(0)]
    public uint Version { get; set; }
    public string PddiShaderName { get; set; }
    private uint hasTranslucency;
    public bool HasTranslucency
    {
        get => hasTranslucency != 0;
        set => hasTranslucency = value ? 1u : 0u;
    }
    public VertexMasks VertexNeeds { get; set; }
    public VertexMasks VertexMask { get; set; }
    public uint NumParams => (uint)Children.Count;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(PddiShaderName));
            data.AddRange(BitConverter.GetBytes(hasTranslucency));
            data.AddRange(BitConverter.GetBytes((uint)VertexNeeds));
            data.AddRange(BitConverter.GetBytes((uint)~VertexMask));
            data.AddRange(BitConverter.GetBytes(NumParams));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(PddiShaderName) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public ShaderChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        PddiShaderName = br.ReadP3DString();
        hasTranslucency = br.ReadUInt32();
        VertexNeeds = (VertexMasks)br.ReadUInt32();
        VertexMask = (VertexMasks)~br.ReadUInt32();
        var numParams = br.ReadUInt32();
    }

    public ShaderChunk(string name, uint version, string pddiShaderName, bool hasTranslucency, VertexMasks vertexNeeds, VertexMasks vertexMask) : base(ChunkID)
    {
        Name = name;
        Version = version;
        PddiShaderName = pddiShaderName;
        HasTranslucency = hasTranslucency;
        VertexNeeds = vertexNeeds;
        VertexMask = vertexMask;
    }

    public override void Validate()
    {
        if (!PddiShaderName.IsValidP3DString())
            throw new InvalidP3DStringException(this, nameof(PddiShaderName), PddiShaderName);

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(PddiShaderName);
        bw.Write(hasTranslucency);
        bw.Write((uint)VertexNeeds);
        bw.Write((uint)~VertexMask);
        bw.Write(NumParams);
    }

    protected override Chunk CloneSelf() => new ShaderChunk(Name, Version, PddiShaderName, HasTranslucency, VertexNeeds, VertexMask);
}
