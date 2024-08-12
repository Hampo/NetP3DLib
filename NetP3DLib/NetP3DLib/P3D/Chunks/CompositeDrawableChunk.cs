using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Composite_Drawable)]
public class CompositeDrawableChunk : NamedChunk
{
    public string SkeletonName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(SkeletonName));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(SkeletonName).Length;

    public CompositeDrawableChunk(BinaryReader br) : base((uint)ChunkIdentifier.Composite_Drawable)
    {
        Name = br.ReadP3DString();
        SkeletonName = br.ReadP3DString();
    }

    public CompositeDrawableChunk(string name, string skeletonName) : base((uint)ChunkIdentifier.Composite_Drawable)
    {
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
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SkeletonName);
    }
}