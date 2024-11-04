using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawable2Chunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Composite_Drawable_2;
    
    public uint Version { get; set; }
    public string SkeletonName { get; set; }
    public uint NumPrimitives => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Composite_Drawable_Primitive).Count();

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
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(SkeletonName).Length + sizeof(uint);

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
        if (SkeletonName == null)
            throw new InvalidDataException($"{nameof(SkeletonName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(SkeletonName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(SkeletonName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SkeletonName);
        bw.Write(NumPrimitives);
    }
}