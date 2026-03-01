using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphLightGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Scenegraph_Light_Group;

    private string _lightGroupName = string.Empty;
    public string LightGroupName
    {
        get => _lightGroupName;
        set
        {
            if (_lightGroupName == value)
                return;

            _lightGroupName = value;
            RecalculateSize();
        }
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
        Name = br.ReadP3DString();
        LightGroupName = br.ReadP3DString();
    }

    public OldScenegraphLightGroupChunk(string name, string lightGroupName) : base(ChunkID)
    {
        Name = name;
        LightGroupName = lightGroupName;
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