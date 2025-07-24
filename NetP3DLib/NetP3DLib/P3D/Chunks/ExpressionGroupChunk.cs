using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExpressionGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Expression_Group;

    public enum Stage : uint
    {
        Stage1,
        Stage2,
        Stage3,
    }
    
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
    public List<Stage> Stages { get; } = [];

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
                data.AddRange(BitConverter.GetBytes((uint)stage));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(TargetName) + sizeof(uint) + sizeof(uint) * NumStages;

    public ExpressionGroupChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        TargetName = br.ReadP3DString();
        var numStages = br.ReadInt32();
        Stages = new(numStages);
        for (int i = 0; i < numStages; i++)
            Stages.Add((Stage)br.ReadUInt32());
    }

    public ExpressionGroupChunk(uint version, string name, string targetName, IList<Stage> stages) : base(ChunkID)
    {
        Version = version;
        Name = name;
        TargetName = targetName;
        Stages.AddRange(stages);
    }

    public override void Validate()
    {
        if (!TargetName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(TargetName), TargetName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(TargetName);
        bw.Write(NumStages);
        foreach (var stage in Stages)
            bw.Write((uint)stage);
    }

    internal override Chunk CloneSelf() => new ExpressionGroupChunk(Version, Name, TargetName, Stages);
}