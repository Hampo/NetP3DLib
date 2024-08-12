using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Animation_Group)]
public class AnimationGroupChunk : NamedChunk
{
    public uint Version { get; set; }
    public uint GroupID { get; set; }
    public uint NumChannels => (uint)Children.Count;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(GroupID));
            data.AddRange(BitConverter.GetBytes(NumChannels));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public AnimationGroupChunk(BinaryReader br) : base((uint)ChunkIdentifier.Animation_Group)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        GroupID = br.ReadUInt32();
        var numChannels = br.ReadUInt32();
    }

    public AnimationGroupChunk(uint version, string name, uint groupID) : base((uint)ChunkIdentifier.Animation_Group)
    {
        Version = version;
        Name = name;
        GroupID = groupID;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(GroupID);
        bw.Write(NumChannels);
    }
}