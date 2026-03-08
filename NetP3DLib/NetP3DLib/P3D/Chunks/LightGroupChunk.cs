using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
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
        get => (uint)(Lights?.Count ?? 0);
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
    public SizeAwareList<string> Lights { get; }

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

            if (Lights != null)
                foreach (var light in Lights)
                    size += BinaryExtensions.GetP3DStringLength(light);

            return size;
        }
    }

    public LightGroupChunk(EndianAwareBinaryReader br) : base(ChunkID)
    {
        _name = new(this, br);
        var numLights = br.ReadInt32();
        var lights = new string[numLights];
        for (int i = 0; i < numLights; i++)
            lights[i] = br.ReadP3DString();
        Lights = CreateSizeAwareList(lights);
    }

    public LightGroupChunk(string name, IList<string> lights) : base(ChunkID)
    {
        _name = new(this, name);
        Lights = CreateSizeAwareList(lights);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        foreach (var light in Lights)
            if (!light.IsValidP3DString())
                yield return new InvalidP3DStringException(this, nameof(Lights), light);
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumLights);
        foreach (var light in Lights)
            bw.WriteP3DString(light);
    }

    protected override Chunk CloneSelf() => new LightGroupChunk(Name, Lights);
}