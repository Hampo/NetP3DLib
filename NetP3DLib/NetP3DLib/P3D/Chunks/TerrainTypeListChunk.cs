using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TerrainTypeListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Terrain_Type_List;

    [DefaultValue(0)]
    public uint Version { get; set; }
    public uint NumTypes
    {
        get => (uint)Types.Count;
        set
        {
            if (value == NumTypes)
                return;

            if (value < NumTypes)
            {
                while (NumTypes > value)
                    Types.RemoveAt(Types.Count - 1);
            }
            else
            {
                while (NumTypes < value)
                    Types.Add(new());
            }
            RecalculateSize();
        }
    }
    public List<TerrainType> Types { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(NumTypes));
            foreach (var type in Types)
                data.Add(type.Value);

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(byte) * NumTypes;

    public TerrainTypeListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numTypes = br.ReadInt32();
        Types = new(numTypes);
        for (int i = 0; i < numTypes; i++)
            Types.Add(new(br));
    }

    public TerrainTypeListChunk(uint version, IList<TerrainType> types) : base(ChunkID)
    {
        Version = version;
        Types.AddRange(types);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumTypes);
        foreach (var type in Types)
            type.Write(bw);
    }

    protected override Chunk CloneSelf() => new TerrainTypeListChunk(Version, Types);

    public class TerrainType
    {
        public enum Types : byte
        {
            /// <summary>
            /// Default road terrain. Also used for sidewalk. This is default. If not set, it's this.
            /// </summary>
            Road,
            /// <summary>
            /// Grass type terrain most everything else which isn't road or sidewalk.
            /// </summary>
            Grass,
            /// <summary>
            /// Sand type terrain.
            /// </summary>
            Sand,
            /// <summary>
            /// Loose gravel type terrain.
            /// </summary>
            Gravel,
            /// <summary>
            /// Water on surface type terrain.
            /// </summary>
            Water,
            /// <summary>
            /// Boardwalks, docks type terrain.
            /// </summary>
            Wood,
            /// <summary>
            /// Powerplant and other structures.
            /// </summary>
            Metal,
            /// <summary>
            /// Dirt type terrain.
            /// </summary>
            Dirt
        }

        public Types Type { get; set; }
        public bool Interior { get; set; }
        internal byte Value
        {
            get
            {
                var value = (byte)Type;
                if (Interior)
                    value |= 0x80;
                return value;
            }
            set
            {
                Type = (Types)(value & ~0x80);
                Interior = (value & 0x80) == 0x80;
            }
        }

        public TerrainType(BinaryReader br)
        {
            Value = br.ReadByte();
        }

        public TerrainType(Types type, bool interior)
        {
            Type = type;
            Interior = interior;
        }

        public TerrainType()
        {
            Type = Types.Road;
            Interior = false;
        }

        internal void Write(BinaryWriter bw)
        {
            bw.Write(Value);
        }

        internal TerrainType Clone() => new(Type, Interior);

        public override string ToString() => $"{Type} | {Interior}";
    }
}
