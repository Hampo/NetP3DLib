using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SurfaceTypeListChunk : Chunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Surface_Type_List;
    
    public uint Version { get; set; }
    public uint NumTypes
    {
        get => (uint)Types.Count;
        set
        {
            if (value == NumTypes)
                return;

            if (value < NumTypes)
            {
                while (NumTypes > value)
                    Types.RemoveAt(Types.Count - 1);
            }
            else
            {
                while (NumTypes < value)
                    Types.Add(default);
            }
        }
    }
    public List<byte> Types { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumTypes));
            data.AddRange(Types);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(byte) * NumTypes;

    public SurfaceTypeListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numTypes = br.ReadInt32();
        Types.Capacity = numTypes;
        Types.AddRange(br.ReadBytes(numTypes));
    }

    public SurfaceTypeListChunk(uint version, IList<byte> types) : base(ChunkID)
    {
        Version = version;
        Types.AddRange(types);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumTypes);
        bw.Write(Types.ToArray());
    }
}