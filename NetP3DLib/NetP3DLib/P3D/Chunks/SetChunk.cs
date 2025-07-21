using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SetChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Set;
    
    public uint Version { get; set; }
    public byte NumTextures => (byte)Children.Where(x => x.ID == (uint)ChunkIdentifier.Texture).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.Add(NumTextures);

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(byte);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public SetChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numTextures = br.ReadByte();
    }

    public SetChunk(string name, uint version) : base(ChunkID)
    {
        Name = name;
        Version = version;
    }

    public override void Validate()
    {
        if (Children.Where(x => x.ID == (uint)ChunkIdentifier.Texture).Count() > byte.MaxValue)
            throw new InvalidDataException($"The max number of child textures is {byte.MinValue}.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumTextures);
    }
}