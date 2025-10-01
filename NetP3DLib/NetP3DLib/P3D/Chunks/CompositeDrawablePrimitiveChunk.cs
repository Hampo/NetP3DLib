using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawablePrimitiveChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_Primitive;
    
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
    public override uint DataLength => sizeof(uint) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public CompositeDrawablePrimitiveChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        CreateInstance = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = br.ReadUInt32();
        SkeletonJointID = br.ReadUInt32();
    }

    public CompositeDrawablePrimitiveChunk(uint version, uint createInstance, string name, uint type, uint skeletonJointId) : base(ChunkID)
    {
        Version = version;
        CreateInstance = createInstance;
        Name = name;
        Type = type;
        SkeletonJointID = skeletonJointId;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(CreateInstance);
        bw.WriteP3DString(Name);
        bw.Write(Type);
        bw.Write(SkeletonJointID);
    }

    protected override Chunk CloneSelf() => new CompositeDrawablePrimitiveChunk(Version, CreateInstance, Name, Type, SkeletonJointID);
}