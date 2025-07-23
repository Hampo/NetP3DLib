using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SpriteChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Sprite;
    
    public uint NativeX { get; set; }
    public uint NativeY { get; set; }
    public string Shader { get; set; }
    public uint ImageWidth { get; set; }
    public uint ImageHeight { get; set; }
    public uint ImageCount => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Image).Count();
    public uint BlitBorder { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NativeX));
            data.AddRange(BitConverter.GetBytes(NativeY));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Shader));
            data.AddRange(BitConverter.GetBytes(ImageWidth));
            data.AddRange(BitConverter.GetBytes(ImageHeight));
            data.AddRange(BitConverter.GetBytes(ImageCount));
            data.AddRange(BitConverter.GetBytes(BlitBorder));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(Shader) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public SpriteChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        NativeX = br.ReadUInt32();
        NativeY = br.ReadUInt32();
        Shader = br.ReadP3DString();
        ImageWidth = br.ReadUInt32();
        ImageHeight = br.ReadUInt32();
        var imageCount = br.ReadUInt32();
        BlitBorder = br.ReadUInt32();
    }

    public SpriteChunk(string name, uint nativeX, uint nativeY, string shader, uint imageWidth, uint imageHeight, uint blitBorder) : base(ChunkID)
    {
        Name = name;
        NativeX = nativeX;
        NativeY = nativeY;
        Shader = shader;
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
        BlitBorder = blitBorder;
    }

    public override void Validate()
    {
        if (!Shader.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Shader), Shader);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NativeX);
        bw.Write(NativeY);
        bw.WriteP3DString(Shader);
        bw.Write(ImageWidth);
        bw.Write(ImageHeight);
        bw.Write(ImageCount);
        bw.Write(BlitBorder);
    }

    internal override Chunk CloneSelf() => new SpriteChunk(Name, NativeX, NativeY, Shader, ImageWidth, ImageHeight, BlitBorder);
}