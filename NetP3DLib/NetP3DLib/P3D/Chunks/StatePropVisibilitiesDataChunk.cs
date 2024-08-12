using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.State_Prop_Visibilities_Data)]
public class StatePropVisibilitiesDataChunk : NamedChunk
{
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint);

    public StatePropVisibilitiesDataChunk(BinaryReader br) : base((uint)ChunkIdentifier.State_Prop_Visibilities_Data)
    {
        Name = br.ReadP3DString();
        IsVisible = br.ReadUInt32();
    }

    public StatePropVisibilitiesDataChunk(string name, uint isVisible) : base((uint)ChunkIdentifier.State_Prop_Visibilities_Data)
    {
        Name = name;
        IsVisible = isVisible;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(IsVisible);
    }
}