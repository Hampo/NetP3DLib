using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphDrawableChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Old_Scenegraph_Drawable;
    
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(DrawableName).Length + sizeof(uint);

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
        if (DrawableName == null)
            throw new InvalidDataException($"{nameof(DrawableName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(DrawableName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(DrawableName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(DrawableName);
        bw.Write(IsTranslucent);
    }
}