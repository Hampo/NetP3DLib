using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class LightGroupChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Light_Group;
    
    public uint NumLights
    {
        get => (uint)Lights.Count;
        set
        {
            if (value == NumLights)
                return;

            if (value < NumLights)
            {
                while (NumLights > value)
                    Lights.RemoveAt(Lights.Count - 1);
            }
            else
            {
                while (NumLights < value)
                    Lights.Add(string.Empty);
            }
        }
    }
    public List<string> Lights { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(NumLights));
            foreach (var light in Lights)
                data.AddRange(BinaryExtensions.GetP3DStringBytes(light));

            return [.. data];
        }
    }
    public override uint DataLength
    {
        get
        {
            uint size = BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint);
            foreach (var light in Lights)
                size += BinaryExtensions.GetP3DStringLength(light);
            return size;
        }
    }

    public LightGroupChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        var numLights = br.ReadInt32();
        Lights = new(numLights);
        for (int i = 0; i < numLights; i++)
            Lights.Add(br.ReadP3DString());
    }

    public LightGroupChunk(string name, IList<string> lights) : base(ChunkID)
    {
        Name = name;
        Lights.AddRange(lights);
    }

    public override void Validate()
    {
        foreach (var light in Lights)
            if (!light.IsValidP3DString())
                throw new InvalidP3DStringException(nameof(Lights), light);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumLights);
        foreach (var light in Lights)
            bw.WriteP3DString(light);
    }

    internal override Chunk CloneSelf() => new LightGroupChunk(Name, Lights);
}