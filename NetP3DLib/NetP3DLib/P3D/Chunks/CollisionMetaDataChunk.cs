using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionMetaDataChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Meta_Data;
    
    public uint Version { get; set; }
    public uint NumChannels => (uint)Children.Count; // TODO: Potentially calculate from channel chunks specifically

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumChannels));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public CollisionMetaDataChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numChannels = br.ReadUInt32();
    }

    public CollisionMetaDataChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumChannels);
    }
}