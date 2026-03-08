using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MultiControllerTrackChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Multi_Controller_Track;
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    private readonly FourCC _type;
    [MaxLength(4)]
    public string Type
    {
        get => _type?.Value ?? string.Empty;
        set => _type.Value = value;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetFourCCBytes(Type));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + 4;

    public MultiControllerTrackChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        _type = new(this, br);
    }

    public MultiControllerTrackChunk(uint version, string name, string type) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        _type = new(this, type);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!Type.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(Type), Type);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(Type);
    }

    protected override Chunk CloneSelf() => new MultiControllerTrackChunk(Version, Name, Type);
}
