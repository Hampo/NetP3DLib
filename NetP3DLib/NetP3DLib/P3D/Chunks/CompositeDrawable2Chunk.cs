using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawable2Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_2;
    
    public uint Version { get; set; }
    private readonly P3DString _skeletonName;
    public string SkeletonName
    {
        get => _skeletonName?.Value ?? string.Empty;
        set => _skeletonName.Value = value;
    }
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
        _name = new(this, br);
        _skeletonName = new(this, br);
        var numPrimitives = br.ReadUInt32();
    }

    public CompositeDrawable2Chunk(uint version, string name, string skeletonName) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        _skeletonName = new(this, skeletonName);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!SkeletonName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(SkeletonName), SkeletonName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SkeletonName);
        bw.Write(NumPrimitives);
    }

    protected override Chunk CloneSelf() => new CompositeDrawable2Chunk(Version, Name, SkeletonName);
}