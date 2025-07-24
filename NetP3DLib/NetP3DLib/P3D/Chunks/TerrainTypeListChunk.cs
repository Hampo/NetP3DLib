using NetP3DLib.P3D.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TerrainTypeListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Terrain_Type_List;
    
    public enum TerrainType : byte
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
                    Types.Add(default);
            }
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
            data.AddRange(Types.Cast<byte>());

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(byte) * NumTypes;

    public TerrainTypeListChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        var numTypes = br.ReadInt32();
        Types.Capacity = numTypes;
        Types.AddRange(br.ReadBytes(numTypes).Cast<TerrainType>());
    }

    public TerrainTypeListChunk(uint version, IList<TerrainType> types) : base(ChunkID)
    {
        Version = version;
        Types.AddRange(types);
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(NumTypes);
        bw.Write(Types.Cast<byte>().ToArray());
    }

    internal override Chunk CloneSelf() => new TerrainTypeListChunk(Version, Types);
}