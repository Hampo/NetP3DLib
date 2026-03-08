using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SmartPropChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Smart_Prop;
    
    public uint Version { get; set; }
    private readonly P3DString _objectFactoryName;
    public string ObjectFactoryName
    {
        get => _objectFactoryName?.Value ?? string.Empty;
        set => _objectFactoryName.Value = value;
    }
    private readonly P3DString _material;
    public string Material
    {
        get => _material?.Value ?? string.Empty;
        set => _material.Value = value;
    }
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

    public SmartPropChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        _name = new(this, br);
        _objectFactoryName = new(this, br);
        _material = new(this, br);
        MaterialEnum = br.ReadUInt32();
        NumBreakables = br.ReadUInt32();
        RenderingCost = br.ReadUInt32();
        SimulationCost = br.ReadUInt32();
        NumStates = br.ReadUInt32();
    }

    public SmartPropChunk(uint version, string name, string objectFactoryName, string material, uint materialEnum, uint numBreakables, uint renderingCost, uint simulationCost, uint numStates) : base(ChunkID)
    {
        Version = version;
        _name = new(this, name);
        _objectFactoryName = new(this, objectFactoryName);
        _material = new(this, material);
        MaterialEnum = materialEnum;
        NumBreakables = numBreakables;
        RenderingCost = renderingCost;
        SimulationCost = simulationCost;
        NumStates = numStates;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!ObjectFactoryName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ObjectFactoryName), ObjectFactoryName);

        if (!Material.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(Material), Material);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
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

    protected override Chunk CloneSelf() => new SmartPropChunk(Version, Name, ObjectFactoryName, Material, MaterialEnum, NumBreakables, RenderingCost, SimulationCost, NumStates);
}