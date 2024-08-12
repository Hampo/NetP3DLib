using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Scenegraph_Visibility)]
public class OldScenegraphVisibilityChunk : NamedChunk
{
    public uint NumChildren => (uint)Children.Count;
    public uint IsVisible { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumChildren));
            data.AddRange(BitConverter.GetBytes(IsVisible));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldScenegraphVisibilityChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Scenegraph_Visibility)
    {
        Name = br.ReadP3DString();
        var numChildren = br.ReadUInt32();
        IsVisible = br.ReadUInt32();
    }

    public OldScenegraphVisibilityChunk(string name, uint isVisible) : base((uint)ChunkIdentifier.Old_Scenegraph_Visibility)
    {
        Name = name;
        IsVisible = isVisible;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumChildren);
        bw.Write(IsVisible);
    }
}