using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropVisibilitiesDataChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_Visibilities_Data;

    private uint isVisible;
    public bool IsVisible
    {
        get => isVisible != 0;
        set => isVisible = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(isVisible));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);

    public StatePropVisibilitiesDataChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        isVisible = br.ReadUInt32();
    }

    public StatePropVisibilitiesDataChunk(string name, bool isVisible) : base(ChunkID)
    {
        _name = new(this, name);
        IsVisible = isVisible;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(isVisible);
    }

    protected override Chunk CloneSelf() => new StatePropVisibilitiesDataChunk(Name, IsVisible);
}