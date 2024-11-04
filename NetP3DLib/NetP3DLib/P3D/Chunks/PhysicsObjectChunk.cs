using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PhysicsObjectChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Physics_Object;
    
    public uint Version { get; set; }
    public string MaterialName { get; set; }
    public uint NumJoints { get; set; }
    public float Volume { get; set; }
    public float RestingSensitivity { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(MaterialName));
            data.AddRange(BitConverter.GetBytes(NumJoints));
            data.AddRange(BitConverter.GetBytes(Volume));
            data.AddRange(BitConverter.GetBytes(RestingSensitivity));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(MaterialName).Length + sizeof(uint) + sizeof(float) + sizeof(float);

    public PhysicsObjectChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        MaterialName = br.ReadP3DString();
        NumJoints = br.ReadUInt32();
        Volume = br.ReadSingle();
        RestingSensitivity = br.ReadSingle();
    }

    public PhysicsObjectChunk(string name, uint version, string materialName, uint numJoints, float volume, float restingSensitivity) : base(ChunkID)
    {
        Name = name;
        Version = version;
        MaterialName = materialName;
        NumJoints = numJoints;
        Volume = volume;
        RestingSensitivity = restingSensitivity;
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
        bw.Write(NumJoints);
        bw.Write(Volume);
        bw.Write(RestingSensitivity);
    }
}