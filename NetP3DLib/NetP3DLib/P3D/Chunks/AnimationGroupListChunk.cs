using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimationGroupListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Animation_Group_List;
    
    public uint Version { get; set; }
    public uint NumGroups => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Animation_Group).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumGroups));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public AnimationGroupListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numGroups = br.ReadUInt32();
    }

    public AnimationGroupListChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumGroups);
    }
}