using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawablePropChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_Prop;

    private uint isTranslucent;
    public bool IsTranslucent
    {
        get => isTranslucent != 0;
        set => isTranslucent = value ? 1u : 0u;
    }
    public uint SkeletonJointId { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(isTranslucent));
            data.AddRange(BitConverter.GetBytes(SkeletonJointId));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public CompositeDrawablePropChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        isTranslucent = br.ReadUInt32();
        SkeletonJointId = br.ReadUInt32();
    }

    public CompositeDrawablePropChunk(string name, bool isTranslucent, uint skeletonJointId) : base(ChunkID)
    {
        Name = name;
        IsTranslucent = isTranslucent;
        SkeletonJointId = skeletonJointId;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(isTranslucent);
        bw.Write(SkeletonJointId);
    }

    internal override Chunk CloneSelf() => new CompositeDrawablePropChunk(Name, IsTranslucent, SkeletonJointId);
}