using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LensFlareGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Lens_Flare_Group;
    
    public uint Version { get; set; }
    private string _shaderName = string.Empty;
    public string ShaderName
    {
        get => _shaderName;
        set
        {
            if (_shaderName == value)
                return;

            _shaderName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    private uint zTest;
    public bool ZTest
    {
        get => zTest == 1;
        set => zTest = value ? 1u : 0u;
    }
    private uint zWrite;
    public bool ZWrite
    {
        get => zWrite == 1;
        set => zWrite = value ? 1u : 0u;
    }
    private uint fog;
    public bool Fog
    {
        get => fog == 1;
        set => fog = value ? 1u : 0u;
    }
    public float SourceRadius { get; set; }
    public float EdgeRadius { get; set; }
    public uint NumLensFlares => GetChildCount(ChunkIdentifier.Lens_Flare);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ShaderName));
            data.AddRange(BitConverter.GetBytes(zTest));
            data.AddRange(BitConverter.GetBytes(zWrite));
            data.AddRange(BitConverter.GetBytes(fog));
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
        zTest = br.ReadUInt32();
        zWrite = br.ReadUInt32();
        fog = br.ReadUInt32();
        SourceRadius = br.ReadSingle();
        EdgeRadius = br.ReadSingle();
        var numLensFlares = br.ReadUInt32();
    }

    public LensFlareGroupChunk(uint version, string name, string shaderName, bool zTest, bool zWrite, bool fog, float sourceRadius, float edgeRadius) : base(ChunkID)
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

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!ShaderName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ShaderName), ShaderName);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(ShaderName);
        bw.Write(zTest);
        bw.Write(zWrite);
        bw.Write(fog);
        bw.Write(SourceRadius);
        bw.Write(EdgeRadius);
        bw.Write(NumLensFlares);
    }

    protected override Chunk CloneSelf() => new LensFlareGroupChunk(Version, Name, ShaderName, ZTest, ZWrite, Fog, SourceRadius, EdgeRadius);
}