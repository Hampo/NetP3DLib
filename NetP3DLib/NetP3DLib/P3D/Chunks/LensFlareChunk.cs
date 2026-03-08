using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LensFlareChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Lens_Flare;

    [DefaultValue(0)]
    public uint Version { get; set; }
    public uint NumOldBillboardQuadGroups => GetChildCount(ChunkIdentifier.Old_Billboard_Quad_Group);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumOldBillboardQuadGroups));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public LensFlareChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        Version = br.ReadUInt32();
        var numOldBillboardQuadGroups = br.ReadUInt32();
    }

    public LensFlareChunk(string name, uint version) : base(ChunkID)
    {
        _name = new(this, name);
        Version = version;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumOldBillboardQuadGroups);
    }

    protected override Chunk CloneSelf() => new LensFlareChunk(Name, Version);
}
