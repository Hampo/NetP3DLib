using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
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
    private string _type = string.Empty;
    [MaxLength(4)]
    public string Type
    {
        get => _type;
        set
        {
            if (_type == value)
                return;

            _type = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
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
        Name = br.ReadP3DString();
        Type = br.ReadFourCC();
    }

    public MultiControllerTrackChunk(uint version, string name, string type) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Type = type;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        if (!Type.IsValidFourCC())
            yield return new InvalidP3DFourCCException(this,nameof(Type), Type);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteFourCC(Type);
    }

    protected override Chunk CloneSelf() => new MultiControllerTrackChunk(Version, Name, Type);
}
