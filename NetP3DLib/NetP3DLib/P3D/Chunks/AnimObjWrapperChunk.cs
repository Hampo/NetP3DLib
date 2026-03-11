using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class AnimObjWrapperChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Anim_Obj_Wrapper;

    [DefaultValue(1)]
    public byte Version { get; set; }
    private byte _hasAlpha;
    public bool HasAlpha
    {
        get => _hasAlpha != 0;
        set => _hasAlpha = (byte)(value ? 1 : 0);
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.Add(Version);
            data.Add(_hasAlpha);

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(byte) + sizeof(byte);

    public AnimObjWrapperChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadByte(), br.ReadByte())
    {
    }

    public AnimObjWrapperChunk(string name, byte version, bool hasAlpha) : this(name, version, (byte)(hasAlpha ? 1 : 0))
    {
    }

    public AnimObjWrapperChunk(string name, byte version, byte hasAlpha) : base(ChunkID, name)
    {
        Version = version;
        _hasAlpha = hasAlpha;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(_hasAlpha);
    }

    protected override Chunk CloneSelf() => new AnimObjWrapperChunk(Name, Version, HasAlpha);
}
