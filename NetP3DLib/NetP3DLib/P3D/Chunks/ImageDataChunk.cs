using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ImageDataChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Image_Data;
    
    public byte[] ImageData { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(ImageData.Length));
            data.AddRange(ImageData);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)ImageData.Length;

    public ImageDataChunk(BinaryReader br) : base(ChunkID)
    {
        int length = br.ReadInt32();
        ImageData = br.ReadBytes(length);
    }

    public ImageDataChunk(byte[] imageData) : base(ChunkID)
    {
        ImageData = (byte[])imageData.Clone();
    }

    public override void Validate()
    {
        if (ImageData.LongLength > int.MaxValue)
            throw new InvalidDataException($"The max length of {nameof(ImageData)} is {int.MaxValue} bytes.");

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(ImageData.Length);
        bw.Write(ImageData);
    }

    protected override Chunk CloneSelf() => new ImageDataChunk(ImageData);
}