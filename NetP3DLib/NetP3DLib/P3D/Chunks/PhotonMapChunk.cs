using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class PhotonMapChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Photon_Map;
    
    public uint Version { get; set; }
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
            NumLightScales = value;
        }
    }
    public List<string> Lights { get; } = [];
    public uint NumLightScales
    {
        get => (uint)LightScales.Count;
        set
        {
            if (value == NumLightScales)
                return;

            if (value < NumLightScales)
            {
                while (NumLightScales > value)
                    LightScales.RemoveAt(LightScales.Count - 1);
            }
            else
            {
                while (NumLightScales < value)
                    LightScales.Add(default);
            }
            NumLights = value;
        }
    }
    public List<float> LightScales { get; } = [];
    public uint NumPhotons
    {
        get => (uint)Photons.Count;
        set
        {
            if (value == NumPhotons)
                return;

            if (value < NumPhotons)
            {
                while (NumPhotons > value)
                    Photons.RemoveAt(Photons.Count - 1);
            }
            else
            {
                while (NumPhotons < value)
                    Photons.Add(new());
            }
        }
    }
    public List<Photon> Photons { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumLights));
            foreach (var light in Lights)
                data.AddRange(BinaryExtensions.GetP3DStringBytes(light));
            foreach (var lightScale in LightScales)
                data.AddRange(BitConverter.GetBytes(lightScale));
            data.AddRange(BitConverter.GetBytes(NumPhotons));
            foreach (var photon in Photons)
                data.AddRange(photon.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength
    {
        get
        {
            uint size = BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(float) * NumLights + sizeof(uint) + Photon.Size * NumPhotons;
            foreach (var light in Lights)
                size += BinaryExtensions.GetP3DStringLength(light);
            return size;
        }
    }

    public PhotonMapChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        var numLights = br.ReadInt32();
        Lights = new(numLights);
        LightScales = new(numLights);
        for (var i = 0; i < numLights; i++)
            Lights.Add(br.ReadP3DString());
        for (var i = 0; i < numLights; i++)
            LightScales.Add(br.ReadSingle());
        var numPhotons = br.ReadInt32();
        Photons = new(numPhotons);
        for (var i = 0; i < numPhotons; i++)
            Photons.Add(new(br));
    }

    public PhotonMapChunk(string name, uint version, IList<string> lights, IList<float> lightScales, IList<Photon> photons) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Lights.AddRange(lights);
        LightScales.AddRange(lightScales);
        Photons.AddRange(photons);
    }

    public override void Validate()
    {
        if (Lights.Count != LightScales.Count)
            throw new InvalidP3DException($"{nameof(Lights)} and {nameof(LightScales)} must have equal counts.");

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(NumLights);
        foreach (var light in Lights)
            bw.WriteP3DString(light);
        foreach (var lightScale in LightScales)
            bw.Write(lightScale);
        bw.Write(NumPhotons);
        foreach (var photon in Photons)
            photon.Write(bw);
    }

    protected override Chunk CloneSelf()
    {
        var photons = new List<Photon>(Photons.Count);
        foreach (var photon in Photons)
            photons.Add(photon.Clone());
        return new PhotonMapChunk(Name, Version, Lights, LightScales, photons);
    }

    public class Photon
    {
        public const uint Size = sizeof(float) * 3 + sizeof(short) + sizeof(byte) + sizeof(byte) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(byte) + sizeof(byte);

        public Vector3 Position { get; set; }
        public short Plane { get; set; }
        public byte Theta { get; set; }
        public byte Phi { get; set; }
        public float RedPower { get; set; }
        public float GreenPower { get; set; }
        public float BluePower { get; set; }
        public float AlphaPower { get; set; }
        public byte NumScatterings { get; set; }
        public byte LightIndex { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BinaryExtensions.GetBytes(Position));
                data.AddRange(BitConverter.GetBytes(Plane));
                data.Add(Theta);
                data.Add(Phi);
                data.AddRange(BitConverter.GetBytes(RedPower));
                data.AddRange(BitConverter.GetBytes(GreenPower));
                data.AddRange(BitConverter.GetBytes(BluePower));
                data.AddRange(BitConverter.GetBytes(AlphaPower));
                data.Add(NumScatterings);
                data.Add(LightIndex);

                return [.. data];
            }
        }

        public Photon(BinaryReader br)
        {
            Position = br.ReadVector3();
            Plane = br.ReadInt16();
            Theta = br.ReadByte();
            Phi = br.ReadByte();
            RedPower = br.ReadSingle();
            GreenPower = br.ReadSingle();
            BluePower = br.ReadSingle();
            AlphaPower = br.ReadSingle();
            NumScatterings = br.ReadByte();
            LightIndex = br.ReadByte();
        }

        public Photon(Vector3 position, short plane, byte theta, byte phi, float redPower, float greenPower, float bluePower, float alphaPower, byte numScatterings, byte lightIndex)
        {
            Position = position;
            Plane = plane;
            Theta = theta;
            Phi = phi;
            RedPower = redPower;
            GreenPower = greenPower;
            BluePower = bluePower;
            AlphaPower = alphaPower;
            NumScatterings = numScatterings;
            LightIndex = lightIndex;
        }

        public Photon()
        {
            Position = new();
            Plane = 0;
            Theta = 0;
            Phi = 0;
            RedPower = 0;
            GreenPower = 0;
            BluePower = 0;
            AlphaPower = 0;
            NumScatterings = 0;
            LightIndex = 0;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Position);
            bw.Write(Plane);
            bw.Write(Theta);
            bw.Write(Phi);
            bw.Write(RedPower);
            bw.Write(GreenPower);
            bw.Write(BluePower);
            bw.Write(AlphaPower);
            bw.Write(NumScatterings);
            bw.Write(LightIndex);
        }

        internal Photon Clone() => new(Position, Plane, Theta, Phi, RedPower, GreenPower, BluePower, AlphaPower, NumScatterings, LightIndex);

        public override string ToString() => $"{Position} | {Plane} | {Theta} | {Phi} | {RedPower} | {GreenPower} | {BluePower} | {AlphaPower} | {NumScatterings} | {LightIndex}";
    }
}