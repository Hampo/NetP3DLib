using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Game_Attr)]
public class GameAttrChunk : NamedChunk
{
    public uint Version { get; set; }
    public uint NumChildren => (uint)Children.Count;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumChildren));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public GameAttrChunk(BinaryReader br) : base((uint)ChunkIdentifier.Game_Attr)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numChildren = br.ReadUInt32();
    }

    public GameAttrChunk(string name, uint version) : base((uint)ChunkIdentifier.Game_Attr)
    {
        Name = name;
        Version = version;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumChildren);
    }
}