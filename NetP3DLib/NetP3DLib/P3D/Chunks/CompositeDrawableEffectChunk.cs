using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawableEffectChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Composite_Drawable_Effect;
    
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

    public CompositeDrawableEffectChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        IsTranslucent = br.ReadUInt32();
        SkeletonJointId = br.ReadUInt32();
    }

    public CompositeDrawableEffectChunk(string name, uint isTranslucent, uint skeletonJointId) : base(ChunkID)
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