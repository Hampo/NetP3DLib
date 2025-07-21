using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphDrawableChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Scenegraph_Drawable;
    
    public string DrawableName { get; set; }
    public uint IsTranslucent { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(DrawableName));
            data.AddRange(BitConverter.GetBytes(IsTranslucent));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(DrawableName) + sizeof(uint);

    public OldScenegraphDrawableChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        DrawableName = br.ReadP3DString();
        IsTranslucent = br.ReadUInt32();
    }

    public OldScenegraphDrawableChunk(string name, string drawableName, uint isTranslucent) : base(ChunkID)
    {
        Name = name;
        DrawableName = drawableName;
        IsTranslucent = isTranslucent;
    }

    public override void Validate()
    {
        if (!DrawableName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(DrawableName), DrawableName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(DrawableName);
        bw.Write(IsTranslucent);
    }
}