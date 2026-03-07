using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphLightGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Scenegraph_Light_Group;

    private readonly P3DString _lightGroupName;
    public string LightGroupName
    {
        get => _lightGroupName?.Value ?? string.Empty;
        set => _lightGroupName.Value = value;
    }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(LightGroupName));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(LightGroupName);

    public OldScenegraphLightGroupChunk(BinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        _lightGroupName = new(this, br);
    }

    public OldScenegraphLightGroupChunk(string name, string lightGroupName) : base(ChunkID)
    {
        _name = new(this, name);
        _lightGroupName = new(this, lightGroupName);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!LightGroupName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(LightGroupName), LightGroupName);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(LightGroupName);
    }

    protected override Chunk CloneSelf() => new OldScenegraphLightGroupChunk(Name, LightGroupName);
}