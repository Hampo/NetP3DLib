using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Composite_Drawable_Skin)]
public class CompositeDrawableSkinChunk : NamedChunk
{
    public uint IsTranslucent { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(IsTranslucent));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint);

    public CompositeDrawableSkinChunk(BinaryReader br) : base((uint)ChunkIdentifier.Composite_Drawable_Skin)
    {
        Name = br.ReadP3DString();
        IsTranslucent = br.ReadUInt32();
    }

    public CompositeDrawableSkinChunk(string name, uint isTranslucent) : base((uint)ChunkIdentifier.Composite_Drawable_Skin)
    {
        Name = name;
        IsTranslucent = isTranslucent;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(IsTranslucent);
    }
}