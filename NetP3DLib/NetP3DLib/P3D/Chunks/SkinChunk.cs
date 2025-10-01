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
public class SkinChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skin;
    
    [DefaultValue(4)]
    public uint Version { get; set; }
    public string SkeletonName { get; set; }
    public uint NumOldPrimitiveGroups => GetChildCount(ChunkIdentifier.Old_Primitive_Group);
    public uint NumPrimitiveGroups => GetChildCount(ChunkIdentifier.Primitive_Group);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(SkeletonName));
            data.AddRange(BitConverter.GetBytes(NumOldPrimitiveGroups + NumPrimitiveGroups));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(SkeletonName) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public SkinChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        SkeletonName = br.ReadP3DString();
        var numPrimitiveGroups = br.ReadUInt32();
    }

    public SkinChunk(string name, uint version, string skeletonName) : base(ChunkID)
    {
        Name = name;
        Version = version;
        SkeletonName = skeletonName;
    }

    public override void Validate()
    {
        if (!SkeletonName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(SkeletonName), SkeletonName);

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(SkeletonName);
        bw.Write(NumOldPrimitiveGroups + NumPrimitiveGroups);
    }

    protected override Chunk CloneSelf() => new SkinChunk(Name, Version, SkeletonName);
}
