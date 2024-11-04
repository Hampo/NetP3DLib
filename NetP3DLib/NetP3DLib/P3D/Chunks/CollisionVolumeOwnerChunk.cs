using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionVolumeOwnerChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Volume_Owner;
    
    public uint NumNames => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Collision_Volume_Owner_Name).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumNames));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public CollisionVolumeOwnerChunk(BinaryReader br) : base(ChunkID)
    {
        var numNames = br.ReadUInt32();
    }

    public CollisionVolumeOwnerChunk() : base(ChunkID)
    { }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumNames);
    }
}