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
    
    public uint IsVisible { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(IsVisible));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);

    public StatePropVisibilitiesDataChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        IsVisible = br.ReadUInt32();
    }

    public StatePropVisibilitiesDataChunk(string name, uint isVisible) : base(ChunkID)
    {
        Name = name;
        IsVisible = isVisible;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(IsVisible);
    }

    internal override Chunk CloneSelf() => new StatePropVisibilitiesDataChunk(Name, IsVisible);
}