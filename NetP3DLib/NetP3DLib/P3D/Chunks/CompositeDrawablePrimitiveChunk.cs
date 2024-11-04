using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Composite_Drawable_Primitive)]
public class CompositeDrawablePrimitiveChunk : NamedChunk
{
    public uint Version { get; set; }
    public uint CreateInstance { get; set; }
    // TODO: Type num
    public uint Type { get; set; }
    public uint SkeletonJointID { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(CreateInstance));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Type));
            data.AddRange(BitConverter.GetBytes(SkeletonJointID));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    public CompositeDrawablePrimitiveChunk(BinaryReader br) : base((uint)ChunkIdentifier.Composite_Drawable_Primitive)
    {
        Version = br.ReadUInt32();
        CreateInstance = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = br.ReadUInt32();
        SkeletonJointID = br.ReadUInt32();
    }

    public CompositeDrawablePrimitiveChunk(uint version, uint createInstance, string name, uint type, uint skeletonJointId) : base((uint)ChunkIdentifier.Composite_Drawable_Primitive)
    {
        Version = version;
        CreateInstance = createInstance;
        Name = name;
        Type = type;
        SkeletonJointID = skeletonJointId;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(CreateInstance);
        bw.WriteP3DString(Name);
        bw.Write(Type);
        bw.Write(SkeletonJointID);
    }
}