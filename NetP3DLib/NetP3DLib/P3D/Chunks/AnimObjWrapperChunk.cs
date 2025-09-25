using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimObjWrapperChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Anim_Obj_Wrapper;
    
    [DefaultValue(1)]
    public byte Version { get; set; }
    private byte hasAlpha;
    public bool HasAlpha
    {
        get => hasAlpha != 0;
        set => hasAlpha = (byte)(value ? 1 : 0);
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.Add(Version);
            data.Add(hasAlpha);

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(byte) + sizeof(byte);

    public AnimObjWrapperChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadByte();
        hasAlpha = br.ReadByte();
    }

    public AnimObjWrapperChunk(string name, byte version, bool hasAlpha) : base(ChunkID)
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

    internal override Chunk CloneSelf() => new AnimObjWrapperChunk(Name, Version, HasAlpha);
}
