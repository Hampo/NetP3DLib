using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldVectorOffsetListChunk : ParamChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Vector_Offset_List;
    
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
    public List<Vector3> Offsets { get; } = [];

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
    public override uint DataLength => sizeof(uint) + sizeof(uint) + 4 + sizeof(float) * 3 * NumOffsets;

    public OldVectorOffsetListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numOffsets = br.ReadInt32();
        Param = br.ReadFourCC();
        Offsets.Capacity = numOffsets;
        for (int i = 0; i < numOffsets; i++)
            Offsets.Add(br.ReadVector3());
    }

    public OldVectorOffsetListChunk(uint version, string param, IList<Vector3> offsets) : base(ChunkID)
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
}