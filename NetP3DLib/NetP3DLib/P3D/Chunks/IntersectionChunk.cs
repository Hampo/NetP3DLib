using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class IntersectionChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Intersection;
    
    public enum TrafficBehaviours : uint
    {
        NoStop,
        NWay
    }

    public Vector3 Position { get; set; }
    public float Radius { get; set; }
    public TrafficBehaviours TrafficBehaviour { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetBytes(Position));
            data.AddRange(BitConverter.GetBytes(Radius));
            data.AddRange(BitConverter.GetBytes((uint)TrafficBehaviour));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(float) * 3 + sizeof(float) + sizeof(uint);

    public IntersectionChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Position = br.ReadVector3();
        Radius = br.ReadSingle();
        TrafficBehaviour = (TrafficBehaviours)br.ReadUInt32();
    }

    public IntersectionChunk(string name, Vector3 position, float radius, TrafficBehaviours trafficBehaviour) : base(ChunkID)
    {
        Name = name;
        Position = position;
        Radius = radius;
        TrafficBehaviour = trafficBehaviour;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Position);
        bw.Write(Radius);
        bw.Write((uint)TrafficBehaviour);
    }

    internal override Chunk CloneSelf() => new IntersectionChunk(Name, Position, Radius, TrafficBehaviour);
}