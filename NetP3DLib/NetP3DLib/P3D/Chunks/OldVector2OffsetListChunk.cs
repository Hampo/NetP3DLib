using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldVector2OffsetListChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Vector2_Offset_List;
    
    public uint Version { get; set; }
    public uint NumOffsets
    {
        get => (uint)Offsets.Count;
        set
        {
            if (value == NumOffsets)
                return;

            if (value < NumOffsets)
            {
                while (NumOffsets > value)
                    Offsets.RemoveAt(Offsets.Count - 1);
            }
            else
            {
                while (NumOffsets < value)
                    Offsets.Add(default);
            }
        }
    }
    public List<Vector2> Offsets { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumOffsets));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Param));
            foreach (var offset in Offsets)
                data.AddRange(BinaryExtensions.GetBytes(offset));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + 4 + sizeof(float) * 2 * NumOffsets;

    public OldVector2OffsetListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numOffsets = br.ReadInt32();
        Param = br.ReadFourCC();
        Offsets = new(numOffsets);
        for (int i = 0; i < numOffsets; i++)
            Offsets.Add(br.ReadVector2());
    }

    public OldVector2OffsetListChunk(uint version, string param, IList<Vector2> offsets) : base(ChunkID)
    {
        Version = version;
        Param = param;
        Offsets.AddRange(offsets);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumOffsets);
        bw.WriteFourCC(Param);
        foreach (var offset in Offsets)
            bw.Write(offset);
    }

    internal override Chunk CloneSelf() => new OldVector2OffsetListChunk(Version, Param, Offsets);
}