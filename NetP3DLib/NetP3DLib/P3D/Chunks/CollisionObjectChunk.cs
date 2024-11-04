using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionObjectChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Object;
    
    public uint Version { get; set; }
    public string MaterialName { get; set; }
    public uint NumSubObjects { get; set; }
    public uint NumOwners => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Collision_Volume_Owner).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(MaterialName));
            data.AddRange(BitConverter.GetBytes(NumSubObjects));
            data.AddRange(BitConverter.GetBytes(NumOwners));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(MaterialName).Length + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public CollisionObjectChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        MaterialName = br.ReadP3DString();
        NumSubObjects = br.ReadUInt32();
        var numOwners = br.ReadUInt32();
    }

    public CollisionObjectChunk(string name, uint version, string materialName, uint numSubObjects) : base(ChunkID)
    {
        Name = name;
        Version = version;
        MaterialName = materialName;
        NumSubObjects = numSubObjects;
    }

    public override void Validate()
    {
        if (MaterialName == null)
            throw new InvalidDataException($"{nameof(MaterialName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(MaterialName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(MaterialName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(MaterialName);
        bw.Write(NumSubObjects);
        bw.Write(NumOwners);
    }
}