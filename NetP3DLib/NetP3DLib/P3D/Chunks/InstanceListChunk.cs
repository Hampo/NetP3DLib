using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Instance_List)]
public class InstanceListChunk : NamedChunk
{
    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length;

    public InstanceListChunk(BinaryReader br) : base((uint)ChunkIdentifier.Instance_List)
    {
        Name = br.ReadP3DString();
    }

    public InstanceListChunk(string name) : base((uint)ChunkIdentifier.Instance_List)
    {
        Name = name;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
    }
}