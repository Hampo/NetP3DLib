using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SmartPropChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Smart_Prop;
    
    public uint Version { get; set; }
    public string ObjectFactoryName { get; set; }
    public string Material { get; set; }
    public uint MaterialEnum { get; set; }
    public uint NumBreakables { get; set; }
    public uint RenderingCost { get; set; }
    public uint SimulationCost { get; set; }
    public uint NumStates { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ObjectFactoryName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Material));
            data.AddRange(BitConverter.GetBytes(MaterialEnum));
            data.AddRange(BitConverter.GetBytes(NumBreakables));
            data.AddRange(BitConverter.GetBytes(RenderingCost));
            data.AddRange(BitConverter.GetBytes(SimulationCost));
            data.AddRange(BitConverter.GetBytes(NumStates));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(ObjectFactoryName) + BinaryExtensions.GetP3DStringLength(Material) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public SmartPropChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        ObjectFactoryName = br.ReadP3DString();
        Material = br.ReadP3DString();
        MaterialEnum = br.ReadUInt32();
        NumBreakables = br.ReadUInt32();
        RenderingCost = br.ReadUInt32();
        SimulationCost = br.ReadUInt32();
        NumStates = br.ReadUInt32();
    }

    public SmartPropChunk(uint version, string name, string objectFactoryName, string material, uint materialEnum, uint numBreakables, uint renderingCost, uint simulationCost, uint numStates) : base(ChunkID)
    {
        Version = version;
        Name = name;
        ObjectFactoryName = objectFactoryName;
        Material = material;
        MaterialEnum = materialEnum;
        NumBreakables = numBreakables;
        RenderingCost = renderingCost;
        SimulationCost = simulationCost;
        NumStates = numStates;
    }

    public override void Validate()
    {
        if (!ObjectFactoryName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(ObjectFactoryName), ObjectFactoryName);

        if (!Material.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(Material), Material);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(ObjectFactoryName);
        bw.WriteP3DString(Material);
        bw.Write(MaterialEnum);
        bw.Write(NumBreakables);
        bw.Write(RenderingCost);
        bw.Write(SimulationCost);
        bw.Write(NumStates);
    }
}