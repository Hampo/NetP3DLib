using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawableChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable;

    private string _skeletonName = string.Empty;
    public string SkeletonName
    {
        get => _skeletonName;
        set
        {
            if (_skeletonName == value)
                return;

            _skeletonName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }

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

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!SkeletonName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(SkeletonName), SkeletonName);

        if ((ParentChunk != null || ParentFile != null) && FindNamedChunkInParentHierarchy<SkeletonChunk>(SkeletonName) == null && FindNamedChunkInParentHierarchy<Skeleton2Chunk>(SkeletonName) == null)
            yield return new InvalidP3DException(this, $"Could not find skeleton with name \"{SkeletonName}\" in the parent hierarchy.");

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(SkeletonName);
    }

    protected override Chunk CloneSelf() => new CompositeDrawableChunk(Name, SkeletonName);
}