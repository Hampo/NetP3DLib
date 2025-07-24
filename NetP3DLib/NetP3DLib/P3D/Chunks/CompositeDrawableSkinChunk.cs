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

    private uint isTranslucent;
    public bool IsTranslucent
    {
        get => isTranslucent != 0;
        set => isTranslucent = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(isTranslucent));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);

    public CompositeDrawableSkinChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        isTranslucent = br.ReadUInt32();
    }

    public CompositeDrawableSkinChunk(string name, bool isTranslucent) : base(ChunkID)
    {
        Name = name;
        IsTranslucent = isTranslucent;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(isTranslucent);
    }

    internal override Chunk CloneSelf() => new CompositeDrawableSkinChunk(Name, IsTranslucent);
}