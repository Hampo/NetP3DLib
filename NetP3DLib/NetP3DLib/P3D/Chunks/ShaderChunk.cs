using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShaderChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Shader;
    
    public uint Version { get; set; }
    public string PddiShaderName { get; set; }
    public uint HasTranslucency { get; set; }
    public uint VertexNeeds { get; set; }
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
            data.AddRange(BitConverter.GetBytes(HasTranslucency));
            data.AddRange(BitConverter.GetBytes(VertexNeeds));
            data.AddRange(BitConverter.GetBytes(VertexMask));
            data.AddRange(BitConverter.GetBytes(NumParams));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(PddiShaderName).Length + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public ShaderChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        PddiShaderName = br.ReadP3DString();
        HasTranslucency = br.ReadUInt32();
        VertexNeeds = br.ReadUInt32();
        VertexMask = br.ReadUInt32();
        var numParams = br.ReadUInt32();
    }

    public ShaderChunk(string name, uint version, string pddiShaderName, uint hasTranslucency, uint vertexNeeds, uint vertexMask) : base(ChunkID)
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
        if (PddiShaderName == null)
            throw new InvalidDataException($"{nameof(PddiShaderName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(PddiShaderName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(PddiShaderName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(PddiShaderName);
        bw.Write(HasTranslucency);
        bw.Write(VertexNeeds);
        bw.Write(VertexMask);
        bw.Write(NumParams);
    }
}