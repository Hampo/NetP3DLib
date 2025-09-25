using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimDynaPhysWrapperChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Anim_Dyna_Phys_Wrapper;
    
    public uint Version { get; set; }
    private uint hasAlpha;
    public bool HasAlpha
    {
        get => hasAlpha != 0;
        set => hasAlpha = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(hasAlpha));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public AnimDynaPhysWrapperChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        hasAlpha = br.ReadUInt32();
    }

    public AnimDynaPhysWrapperChunk(string name, uint version, bool hasAlpha) : base(ChunkID)
    {
        Name = name;
        Version = version;
        HasAlpha = hasAlpha;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(hasAlpha);
    }

    internal override Chunk CloneSelf() => new AnimDynaPhysWrapperChunk(Name, Version, HasAlpha);
}