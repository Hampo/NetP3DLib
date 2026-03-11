using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class InstStatPhysChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Inst_Stat_Phys;

    [DefaultValue(0)]
    public uint Version { get; set; }
    private uint _hasAlpha;
    public bool HasAlpha
    {
        get => _hasAlpha != 0;
        set => _hasAlpha = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(_hasAlpha));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public InstStatPhysChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), br.ReadUInt32())
    {
    }

    public InstStatPhysChunk(string name, uint version, bool hasAlpha) : this(name, version, hasAlpha ? 1u : 0u)
    {
    }

    public InstStatPhysChunk(string name, uint version, uint hasAlpha) : base(ChunkID, name)
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

    protected override Chunk CloneSelf() => new InstStatPhysChunk(Name, Version, HasAlpha);
}
