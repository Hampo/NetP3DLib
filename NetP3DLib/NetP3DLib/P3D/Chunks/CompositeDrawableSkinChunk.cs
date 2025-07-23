using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawableSkinChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_Skin;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);

    public CompositeDrawableSkinChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        IsTranslucent = br.ReadUInt32();
    }

    public CompositeDrawableSkinChunk(string name, uint isTranslucent) : base(ChunkID)
    {
        Name = name;
        IsTranslucent = isTranslucent;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(IsTranslucent);
    }

    internal override Chunk CloneSelf() => new CompositeDrawableSkinChunk(Name, IsTranslucent);
}