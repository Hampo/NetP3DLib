using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawableChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(SkeletonName);

    public CompositeDrawableChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        SkeletonName = br.ReadP3DString();
    }

    public CompositeDrawableChunk(string name, string skeletonName) : base(ChunkID)
    {
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
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SkeletonName);
    }
}