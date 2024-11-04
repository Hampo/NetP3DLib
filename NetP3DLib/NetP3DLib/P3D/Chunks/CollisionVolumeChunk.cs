using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionVolumeChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Collision_Volume;
    
    public uint ObjectReferenceIndex { get; set; }
    public int OwnerIndex { get; set; }
    public uint NumSubVolumes => (uint)Children.Where(x => x.ID == ID).Count();

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

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(ObjectReferenceIndex);
        bw.Write(OwnerIndex);
        bw.Write(NumSubVolumes);
    }
}