using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphVisibilityChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Scenegraph_Visibility;
    
    public uint NumChildren => (uint)Children.Count;
    private uint isVisible;
    public bool IsVisible
    {
        get => isVisible != 0;
        set => isVisible = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumChildren));
            data.AddRange(BitConverter.GetBytes(isVisible));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldScenegraphVisibilityChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var numChildren = br.ReadUInt32();
        isVisible = br.ReadUInt32();
    }

    public OldScenegraphVisibilityChunk(string name, bool isVisible) : base(ChunkID)
    {
        Name = name;
        IsVisible = isVisible;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumChildren);
        bw.Write(isVisible);
    }

    internal override Chunk CloneSelf() => new OldScenegraphVisibilityChunk(Name, IsVisible);
}