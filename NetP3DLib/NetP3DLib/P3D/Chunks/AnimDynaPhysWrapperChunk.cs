using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Anim_Dyna_Phys_Wrapper)]
public class AnimDynaPhysWrapperChunk : NamedChunk
{
    public uint Version { get; set; }
    public uint HasAlpha { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(HasAlpha));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    public AnimDynaPhysWrapperChunk(BinaryReader br) : base((uint)ChunkIdentifier.Anim_Dyna_Phys_Wrapper)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        HasAlpha = br.ReadUInt32();
    }

    public AnimDynaPhysWrapperChunk(string name, uint version, uint hasAlpha) : base((uint)ChunkIdentifier.Anim_Dyna_Phys_Wrapper)
    {
        Name = name;
        Version = version;
        HasAlpha = hasAlpha;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(HasAlpha);
    }
}