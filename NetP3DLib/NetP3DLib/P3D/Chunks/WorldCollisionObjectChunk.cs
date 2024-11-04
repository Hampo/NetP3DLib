using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.World_Collision_Object)]
public class WorldCollisionObjectChunk : NamedChunk
{
    public uint Version { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    public WorldCollisionObjectChunk(BinaryReader br) : base((uint)ChunkIdentifier.World_Collision_Object)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
    }

    public WorldCollisionObjectChunk(string name, uint version) : base((uint)ChunkIdentifier.World_Collision_Object)
    {
        Name = name;
        Version = version;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
    }
}