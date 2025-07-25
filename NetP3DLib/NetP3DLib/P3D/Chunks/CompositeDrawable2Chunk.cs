using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawable2Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_2;
    
    public uint Version { get; set; }
    public string SkeletonName { get; set; }
    public uint NumPrimitives => GetChildCount(ChunkIdentifier.Composite_Drawable_Primitive);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(SkeletonName));
            data.AddRange(BitConverter.GetBytes(NumPrimitives));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(SkeletonName) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public CompositeDrawable2Chunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        SkeletonName = br.ReadP3DString();
        var numPrimitives = br.ReadUInt32();
    }

    public CompositeDrawable2Chunk(uint version, string name, string skeletonName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        SkeletonName = skeletonName;
    }

    public override void Validate()
    {
        if (!SkeletonName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(SkeletonName), SkeletonName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SkeletonName);
        bw.Write(NumPrimitives);
    }

    internal override Chunk CloneSelf() => new CompositeDrawable2Chunk(Version, Name, SkeletonName);
}