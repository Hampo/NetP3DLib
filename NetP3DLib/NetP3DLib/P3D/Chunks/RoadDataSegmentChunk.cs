using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class RoadDataSegmentChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Road_Data_Segment;
    
    public uint Type { get; set; }
    public uint Lanes { get; set; }
    public uint HasShoulder { get; set; }
    public Vector3 Direction { get; set; }
    public Vector3 Top { get; set; }
    public Vector3 Bottom { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Type));
            data.AddRange(BitConverter.GetBytes(Lanes));
            data.AddRange(BitConverter.GetBytes(HasShoulder));
            data.AddRange(BinaryExtensions.GetBytes(Direction));
            data.AddRange(BinaryExtensions.GetBytes(Top));
            data.AddRange(BinaryExtensions.GetBytes(Bottom));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) * 3;

    public RoadDataSegmentChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Type = br.ReadUInt32();
        Lanes = br.ReadUInt32();
        HasShoulder = br.ReadUInt32();
        Direction = br.ReadVector3();
        Top = br.ReadVector3();
        Bottom = br.ReadVector3();
    }

    public RoadDataSegmentChunk(string name, uint type, uint lanes, uint hasShoulder, Vector3 direction, Vector3 top, Vector3 bottom) : base(ChunkID)
    {
        Name = name;
        Type = type;
        Lanes = lanes;
        HasShoulder = hasShoulder;
        Direction = direction;
        Top = top;
        Bottom = bottom;
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Type);
        bw.Write(Lanes);
        bw.Write(HasShoulder);
        bw.Write(Direction);
        bw.Write(Top);
        bw.Write(Bottom);
    }
}