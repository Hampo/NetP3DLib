using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionVolumeOwnerNameChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Volume_Owner_Name;
    
    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name);

    public CollisionVolumeOwnerNameChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
    }

    public CollisionVolumeOwnerNameChunk(string name) : base(ChunkID)
    {
        Name = name;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
    }
}