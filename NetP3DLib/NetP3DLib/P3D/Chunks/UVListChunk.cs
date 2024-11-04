using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class UVListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.UV_List;
    
    public uint NumUVs
    {
        get => (uint)UVs.Count;
        set
        {
            if (value == NumUVs)
                return;

            if (value < NumUVs)
            {
                while (NumUVs > value)
                    UVs.RemoveAt(UVs.Count - 1);
            }
            else
            {
                while (NumUVs < value)
                    UVs.Add(default);
            }
        }
    }
    public uint Channel { get; set; }
    public List<Vector2> UVs { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumUVs));
            data.AddRange(BitConverter.GetBytes(Channel));
            foreach (var uv in UVs)
                data.AddRange(BinaryExtensions.GetBytes(uv));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(float) * 2 * NumUVs;

    public UVListChunk(BinaryReader br) : base(ChunkID)
    {
        var numUVs = br.ReadInt32();
        Channel = br.ReadUInt32();
        UVs.Capacity = numUVs;
        for (int i = 0; i < numUVs; i++)
            UVs.Add(br.ReadVector2());
    }

    public UVListChunk(uint channel, IList<Vector2> uvs) : base(ChunkID)
    {
        Channel = channel;
        UVs.AddRange(uvs);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumUVs);
        bw.Write(Channel);
        foreach (var pos in UVs)
            bw.Write(pos);
    }
}