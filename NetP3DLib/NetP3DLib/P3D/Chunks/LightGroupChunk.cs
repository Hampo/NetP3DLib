using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Light_Group)]
public class LightGroupChunk : NamedChunk
{
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
                    Lights.Add(default);
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)Lights.Sum(x => BinaryExtensions.GetP3DStringBytes(x).Length);

    public LightGroupChunk(BinaryReader br) : base((uint)ChunkIdentifier.Light_Group)
    {
        Name = br.ReadP3DString();
        var numLights = br.ReadInt32();
        Lights.Capacity = numLights;
        for (int i = 0; i < numLights; i++)
            Lights.Add(br.ReadP3DString());
    }

    public LightGroupChunk(string name, IList<string> lights) : base((uint)ChunkIdentifier.Light_Group)
    {
        Name = name;
        Lights.AddRange(lights);
    }

    public override void Validate()
    {
        foreach (var light in Lights)
        {
            if (light == null)
                throw new InvalidDataException($"No entry in {nameof(Lights)} can be null.");
            if (Encoding.UTF8.GetBytes(light).Length > 255)
                    throw new InvalidDataException($"The max length of an entry in {nameof(Lights)} is 255 bytes.");
        }

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumLights);
        foreach (var light in Lights)
            bw.WriteP3DString(light);
    }
}