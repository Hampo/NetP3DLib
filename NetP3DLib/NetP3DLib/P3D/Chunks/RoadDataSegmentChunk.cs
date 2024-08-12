using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Road_Data_Segment)]
public class RoadDataSegmentChunk : NamedChunk
{
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
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) * 3;

    public RoadDataSegmentChunk(BinaryReader br) : base((uint)ChunkIdentifier.Road_Data_Segment)
    {
        Name = br.ReadP3DString();
        Type = br.ReadUInt32();
        Lanes = br.ReadUInt32();
        HasShoulder = br.ReadUInt32();
        Direction = br.ReadVector3();
        Top = br.ReadVector3();
        Bottom = br.ReadVector3();
    }

    public RoadDataSegmentChunk(string name, uint type, uint lanes, uint hasShoulder, Vector3 direction, Vector3 top, Vector3 bottom) : base((uint)ChunkIdentifier.Road_Data_Segment)
    {
        Name = name;
        Type = type;
        Lanes = lanes;
        HasShoulder = hasShoulder;
        Direction = direction;
        Top = top;
        Bottom = bottom;
    }

    public override void Validate()
    {
        base.Validate();
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