using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionVolumeChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Volume;
    
    public uint ObjectReferenceIndex { get; set; }
    public int OwnerIndex { get; set; }
    public uint NumSubVolumes => GetChildCount(ChunkIdentifier.Collision_Volume);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(ObjectReferenceIndex));
            data.AddRange(BitConverter.GetBytes(OwnerIndex));
            data.AddRange(BitConverter.GetBytes(NumSubVolumes));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(int) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public CollisionVolumeChunk(BinaryReader br) : base(ChunkID)
    {
        ObjectReferenceIndex = br.ReadUInt32();
        OwnerIndex = br.ReadInt32();
        var numSubVolumes = br.ReadUInt32();
    }

    public CollisionVolumeChunk(uint objectReferenceIndex, int ownerIndex) : base(ChunkID)
    {
        ObjectReferenceIndex = objectReferenceIndex;
        OwnerIndex = ownerIndex;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(ObjectReferenceIndex);
        bw.Write(OwnerIndex);
        bw.Write(NumSubVolumes);
    }

    internal override Chunk CloneSelf() => new CollisionVolumeChunk(ObjectReferenceIndex, OwnerIndex);
}