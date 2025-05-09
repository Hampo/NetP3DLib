using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class StatePropDataV1Chunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.State_Prop_Data_V1;
    
    public uint Version { get; set; }
    public string ObjectFactoryName { get; set; }
    public uint NumStates => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.State_Prop_State_Data_V1).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ObjectFactoryName));
            data.AddRange(BitConverter.GetBytes(NumStates));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(ObjectFactoryName).Length + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public StatePropDataV1Chunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        ObjectFactoryName = br.ReadP3DString();
        var numStates = br.ReadUInt32();
    }

    public StatePropDataV1Chunk(uint version, string name, string objectFactoryName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        ObjectFactoryName = objectFactoryName;
    }

    public override void Validate()
    {
        if (ObjectFactoryName == null)
            throw new InvalidDataException($"{nameof(ObjectFactoryName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(ObjectFactoryName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(ObjectFactoryName)} is 255 bytes.");
        
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(ObjectFactoryName);
        bw.Write(NumStates);
    }
}