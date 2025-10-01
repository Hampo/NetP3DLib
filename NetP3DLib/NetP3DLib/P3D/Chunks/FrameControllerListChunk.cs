using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class FrameControllerListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Frame_Controller_List;
    
    public uint Version { get; set; }
    public uint NumControllers => GetChildCount(ChunkIdentifier.Frame_Controller);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumControllers));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public FrameControllerListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numControllers = br.ReadUInt32();
    }

    public FrameControllerListChunk(uint version) : base(ChunkID)
    {
        Version = version;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumControllers);
    }

    protected override Chunk CloneSelf() => new FrameControllerListChunk(Version);
}