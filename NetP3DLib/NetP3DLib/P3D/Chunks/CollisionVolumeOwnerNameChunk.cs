using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Collision_Volume_Owner_Name)]
public class CollisionVolumeOwnerNameChunk : NamedChunk
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

    public CollisionVolumeOwnerNameChunk(BinaryReader br) : base((uint)ChunkIdentifier.Collision_Volume_Owner_Name)
    {
        Name = br.ReadP3DString();
    }

    public CollisionVolumeOwnerNameChunk(string name) : base((uint)ChunkIdentifier.Collision_Volume_Owner_Name)
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