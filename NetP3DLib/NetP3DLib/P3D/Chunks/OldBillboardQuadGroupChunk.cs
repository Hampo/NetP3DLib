using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBillboardQuadGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Billboard_Quad_Group;

    [DefaultValue(0)]
    public uint Version { get; set; }
    private readonly P3DString _shader;
    public string Shader
    {
        get => _shader?.Value ?? string.Empty;
        set => _shader.Value = value;
    }
    private uint zTest;
    public bool ZTest
    {
        get => zTest != 0;
        set => zTest = value ? 1u : 0u;
    }
    private uint zWrite;
    public bool ZWrite
    {
        get => zWrite != 0;
        set => zWrite = value ? 1u : 0u;
    }
    public uint Occlusion { get; set; }
    public uint NumQuads => GetChildCount(ChunkIdentifier.Old_Billboard_Quad);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Shader));
            data.AddRange(BitConverter.GetBytes(zTest));
            data.AddRange(BitConverter.GetBytes(zWrite));
            data.AddRange(BitConverter.GetBytes(Occlusion));
            data.AddRange(BitConverter.GetBytes(NumQuads));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(Shader) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldBillboardQuadGroupChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        _shader = new(this, br);
        zTest = br.ReadUInt32();
        zWrite = br.ReadUInt32();
        Occlusion = br.ReadUInt32();
        var numQuads = br.ReadUInt32();
    }

    public OldBillboardQuadGroupChunk(uint version, string name, string shader, bool zTest, bool zWrite, uint occlusion) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        _shader = new(this, shader);
        ZTest = zTest;
        ZWrite = zWrite;
        Occlusion = occlusion;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Shader.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Shader), Shader);

        if ((ParentChunk != null || ParentFile != null) && FindNamedChunkInParentHierarchy<ShaderChunk>(Shader) == null)
            yield return new InvalidP3DException(this, $"Could not find shader with name \"{Shader}\" in the parent hierarchy.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(Shader);
        bw.Write(zTest);
        bw.Write(zWrite);
        bw.Write(Occlusion);
        bw.Write(NumQuads);
    }

    protected override Chunk CloneSelf() => new OldBillboardQuadGroupChunk(Version, Name, Shader, ZTest, ZWrite, Occlusion);
}
