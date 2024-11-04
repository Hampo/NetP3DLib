using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldScenegraphLightGroupChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Old_Scenegraph_Light_Group;
    
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(LightGroupName).Length;

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
        if (LightGroupName == null)
            throw new InvalidDataException($"{nameof(LightGroupName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(LightGroupName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(LightGroupName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(LightGroupName);
    }
}