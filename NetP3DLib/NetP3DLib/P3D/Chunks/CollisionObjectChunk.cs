using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionObjectChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Object;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    public string MaterialName { get; set; }
    public uint NumSubObjects { get; set; }
    public uint NumOwners => GetChildCount(ChunkIdentifier.Collision_Volume_Owner);

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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(MaterialName) + sizeof(uint) + sizeof(uint);

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
        if (!MaterialName.IsValidP3DString())
            throw new InvalidP3DStringException(this, nameof(MaterialName), MaterialName);

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(MaterialName);
        bw.Write(NumSubObjects);
        bw.Write(NumOwners);
    }

    protected override Chunk CloneSelf() => new CollisionObjectChunk(Name, Version, MaterialName, NumSubObjects);
}
