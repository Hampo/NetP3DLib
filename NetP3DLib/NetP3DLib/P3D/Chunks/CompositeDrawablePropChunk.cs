using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Composite_Drawable_Prop)]
public class CompositeDrawablePropChunk : NamedChunk
{
    public uint IsTranslucent { get; set; }
    public uint SkeletonJointId { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(IsTranslucent));
            data.AddRange(BitConverter.GetBytes(SkeletonJointId));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    public CompositeDrawablePropChunk(BinaryReader br) : base((uint)ChunkIdentifier.Composite_Drawable_Prop)
    {
        Name = br.ReadP3DString();
        IsTranslucent = br.ReadUInt32();
        SkeletonJointId = br.ReadUInt32();
    }

    public CompositeDrawablePropChunk(string name, uint isTranslucent, uint skeletonJointId) : base((uint)ChunkIdentifier.Composite_Drawable_Prop)
    {
        Name = name;
        IsTranslucent = isTranslucent;
        SkeletonJointId = skeletonJointId;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(IsTranslucent);
        bw.Write(SkeletonJointId);
    }
}