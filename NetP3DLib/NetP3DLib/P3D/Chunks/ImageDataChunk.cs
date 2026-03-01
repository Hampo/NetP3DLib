using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ImageDataChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Image_Data;

    private byte[] _imageData = [];
    public byte[] ImageData
    {
        get => _imageData;
        set
        {
            if (ReferenceEquals(_imageData, value))
                return;

            int oldSize = _imageData.Length;
            _imageData = value ?? [];
            int delta = _imageData.Length - oldSize;

            OnSizeChanged(delta);
            _cachedSize = Size;
        }
    }

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

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (ImageData.LongLength > int.MaxValue)
            yield return new InvalidP3DException(this, $"The max length of {nameof(ImageData)} is {int.MaxValue} bytes.");
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(ImageData.Length);
        bw.Write(ImageData);
    }

    protected override Chunk CloneSelf() => new ImageDataChunk(ImageData);
}