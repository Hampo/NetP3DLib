using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphDrawableChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Scenegraph_Drawable;

    private readonly P3DString _drawableName;
    public string DrawableName
    {
        get => _drawableName?.Value ?? string.Empty;
        set => _drawableName.Value = value;
    }
    private uint _isTranslucent;
    public bool IsTranslucent
    {
        get => _isTranslucent == 1;
        set => _isTranslucent = value ? 1u : 0u;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(DrawableName));
            data.AddRange(BitConverter.GetBytes(_isTranslucent));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(DrawableName) + sizeof(uint);

    public OldScenegraphDrawableChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        _drawableName = new(this, br);
        _isTranslucent = br.ReadUInt32();
    }

    public OldScenegraphDrawableChunk(string name, string drawableName, bool isTranslucent) : base(ChunkID)
    {
        _name = new(this, name);
        _drawableName = new(this, drawableName);
        IsTranslucent = isTranslucent;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!DrawableName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(DrawableName), DrawableName);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(DrawableName);
        bw.Write(_isTranslucent);
    }

    protected override Chunk CloneSelf() => new OldScenegraphDrawableChunk(Name, DrawableName, IsTranslucent);
}