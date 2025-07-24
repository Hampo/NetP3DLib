using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShaderChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shader;
    
    public uint Version { get; set; }
    public string PddiShaderName { get; set; }
    public uint VertexNeeds { get; set; }
    private uint hasTranslucency;
    public bool HasTranslucency
    {
        get => hasTranslucency != 0;
        set => hasTranslucency = value ? 1u : 0u;
    }
    public uint VertexMask { get; set; }
    public uint NumParams => (uint)Children.Count;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(PddiShaderName));
            data.AddRange(BitConverter.GetBytes(VertexNeeds));
            data.AddRange(BitConverter.GetBytes(hasTranslucency));
            data.AddRange(BitConverter.GetBytes(VertexMask));
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
        VertexNeeds = br.ReadUInt32();
        hasTranslucency = br.ReadUInt32();
        VertexMask = br.ReadUInt32();
        var numParams = br.ReadUInt32();
    }

    public ShaderChunk(string name, uint version, string pddiShaderName, bool hasTranslucency, uint vertexNeeds, uint vertexMask) : base(ChunkID)
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
            throw new InvalidP3DStringException(nameof(PddiShaderName), PddiShaderName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(PddiShaderName);
        bw.Write(VertexNeeds);
        bw.Write(hasTranslucency);
        bw.Write(VertexMask);
        bw.Write(NumParams);
    }

    internal override Chunk CloneSelf() => new ShaderChunk(Name, Version, PddiShaderName, HasTranslucency, VertexNeeds, VertexMask);
}