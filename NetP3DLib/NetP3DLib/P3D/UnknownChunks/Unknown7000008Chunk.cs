using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.UnknownChunks;

[ChunkAttributes(0x7000008)]
public class Unknown7000008Chunk : Chunk
{    
    public uint NumUnknown
    {
        get => (uint)Unknown.Count;
        set
        {
            if (value == NumUnknown)
                return;

            if (value < NumUnknown)
            {
                while (NumUnknown > value)
                    Unknown.RemoveAt(Unknown.Count - 1);
            }
            else
            {
                while (NumUnknown < value)
                    Unknown.Add(default);
            }
        }
    }
    public List<short> Unknown { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumUnknown));
            foreach (var unknown in Unknown)
                data.AddRange(BitConverter.GetBytes(unknown));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(short) * NumUnknown;

    public Unknown7000008Chunk(BinaryReader br) : base(0x7000008)
    {
        var numUnknown = br.ReadInt32();
        Unknown = new(numUnknown);
        for (var i = 0; i < numUnknown; i++)
            Unknown.Add(br.ReadInt16());
    }

    public Unknown7000008Chunk(IList<short> unknown) : base(0x7000008)
    {
        Unknown.AddRange(unknown);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumUnknown);
        foreach (var unknown in Unknown)
            bw.Write(unknown);
    }

    internal override Chunk CloneSelf() => new Unknown7000008Chunk(Unknown);
}