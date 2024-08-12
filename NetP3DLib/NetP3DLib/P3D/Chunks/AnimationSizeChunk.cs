using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Animation_Size)]
public class AnimationSizeChunk : Chunk
{
    public uint Version { get; set; }
    public uint PC { get; set; }
    public uint PS2 { get; set; }
    public uint XBOX { get; set; }
    public uint GC { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(PC));
            data.AddRange(BitConverter.GetBytes(PS2));
            data.AddRange(BitConverter.GetBytes(XBOX));
            data.AddRange(BitConverter.GetBytes(GC));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public AnimationSizeChunk(BinaryReader br) : base((uint)ChunkIdentifier.Animation_Size)
    {
        Version = br.ReadUInt32();
        PC = br.ReadUInt32();
        PS2 = br.ReadUInt32();
        XBOX = br.ReadUInt32();
        GC = br.ReadUInt32();
    }

    public AnimationSizeChunk(uint version, uint pc, uint ps2, uint xbox, uint gc) : base((uint)ChunkIdentifier.Animation_Size)
    {
        Version = version;
        PC = pc;
        PS2 = ps2;
        XBOX = xbox;
        GC = gc;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(PC);
        bw.Write(PS2);
        bw.Write(XBOX);
        bw.Write(GC);
    }
}