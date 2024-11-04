using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExpressionGroupChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Expression_Group;
    
    public uint Version { get; set; }
    public string TargetName { get; set; }
    public uint NumStages
    {
        get => (uint)Stages.Count;
        set
        {
            if (value == NumStages)
                return;

            if (value < NumStages)
            {
                while (NumStages > value)
                    Stages.RemoveAt(Stages.Count - 1);
            }
            else
            {
                while (NumStages < value)
                    Stages.Add(default);
            }
        }
    }
    public List<uint> Stages { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(TargetName));
            data.AddRange(BitConverter.GetBytes(NumStages));
            foreach (var stage in Stages)
                data.AddRange(BitConverter.GetBytes(stage));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(TargetName).Length + sizeof(uint) + sizeof(uint) * NumStages;

    public ExpressionGroupChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        TargetName = br.ReadP3DString();
        var numStages = br.ReadInt32();
        Stages.Capacity = numStages;
        for (int i = 0; i < numStages; i++)
            Stages.Add(br.ReadUInt32());
    }

    public ExpressionGroupChunk(uint version, string name, string targetName, IList<uint> stages) : base(ChunkID)
    {
        Version = version;
        Name = name;
        TargetName = targetName;
        Stages.AddRange(stages);
    }

    public override void Validate()
    {
        if (TargetName == null)
            throw new InvalidDataException($"{nameof(TargetName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(TargetName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(TargetName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(TargetName);
        bw.Write(NumStages);
        foreach (var stage in Stages)
            bw.Write(stage);
    }
}