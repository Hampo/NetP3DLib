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
    
    public string LightGroupName { get; set; }

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

    public override void Validate()
    {
        if (!LightGroupName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(LightGroupName), LightGroupName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(LightGroupName);
    }

    internal override Chunk CloneSelf() => new OldScenegraphLightGroupChunk(Name, LightGroupName);
}