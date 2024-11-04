using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Topology)]
public class TopologyChunk : Chunk
{
    public uint NumTopology
    {
        get => (uint)Topologies.Count;
        set
        {
            if (value == NumTopology)
                return;

            if (value < NumTopology)
            {
                while (NumTopology > value)
                    Topologies.RemoveAt(Topologies.Count - 1);
            }
            else
            {
                while (NumTopology < value)
                    Topologies.Add(new());
            }
        }
    }
    public List<Topology> Topologies { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumTopology));
            foreach (var entry in Topologies)
                data.AddRange(entry.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + Topology.Size * NumTopology;

    public TopologyChunk(BinaryReader br) : base((uint)ChunkIdentifier.Topology)
    {
        var numEntries = br.ReadInt32();
        Topologies.Capacity = numEntries;
        for (int i = 0; i < numEntries; i++)
            Topologies.Add(new(br));
    }

    public TopologyChunk(IList<Topology> entries) : base((uint)ChunkIdentifier.Topology)
    {
        Topologies.AddRange(entries);
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumTopology);
        foreach (var entry in Topologies)
            entry.Write(bw);
    }

    public class Topology
    {
        public const uint Size = sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort);

        public ushort V0 { get; set; }
        public ushort V1 { get; set; }
        public ushort V2 { get; set; }
        public ushort N0 { get; set; }
        public ushort N1 { get; set; }
        public ushort N2 { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BitConverter.GetBytes(V0));
                data.AddRange(BitConverter.GetBytes(V1));
                data.AddRange(BitConverter.GetBytes(V2));
                data.AddRange(BitConverter.GetBytes(N0));
                data.AddRange(BitConverter.GetBytes(N1));
                data.AddRange(BitConverter.GetBytes(N2));

                return [.. data];
            }
        }

        public Topology(BinaryReader br)
        {
            V0 = br.ReadUInt16();
            V1 = br.ReadUInt16();
            V2 = br.ReadUInt16();
            N0 = br.ReadUInt16();
            N1 = br.ReadUInt16();
            N2 = br.ReadUInt16();
        }

        public Topology(ushort v0, ushort v1, ushort v2, ushort n0, ushort n1, ushort n2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            N0 = n0;
            N1 = n1;
            N2 = n2;
        }

        public Topology()
        {
            V0 = 0;
            V1 = 0;
            V2 = 0;
            N0 = 0;
            N1 = 0;
            N2 = 0;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(V0);
            bw.Write(V1);
            bw.Write(V2);
            bw.Write(N0);
            bw.Write(N1);
            bw.Write(N2);
        }

        public override string ToString()
        {
            return $"{V0} | {V1} | {V2} | {N0} | {N1} | {N2}";
        }
    }
}