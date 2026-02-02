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
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
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
            foreach (var light in Lights)
                size += BinaryExtensions.GetP3DStringLength(light);
            return size;
        }
    }

    public LightGroupChunk(BinaryReader br) : base(ChunkID)
    {
        Lights = CreateSizeAwareList<string>();
        Name = br.ReadP3DString();
        var numLights = br.ReadInt32();
        Lights = CreateSizeAwareList<string>(numLights);
        Lights.SuspendNotifications();
        for (int i = 0; i < numLights; i++)
            Lights.Add(br.ReadP3DString());
        Lights.ResumeNotifications();
    }

    public LightGroupChunk(string name, IList<string> lights) : base(ChunkID)
    {
        Lights = CreateSizeAwareList<string>(lights.Count);
        Name = name;
        Lights.SuspendNotifications();
        Lights.AddRange(lights);
        Lights.ResumeNotifications();
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        foreach (var light in Lights)
            if (!light.IsValidP3DString())
                yield return new InvalidP3DStringException(this, nameof(Lights), light);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(NumLights);
        foreach (var light in Lights)
            bw.WriteP3DString(light);
    }

    protected override Chunk CloneSelf() => new LightGroupChunk(Name, Lights);
}