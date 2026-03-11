using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class RoadDataSegmentChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Road_Data_Segment;

    public uint Type { get; set; }
    public uint Lanes { get; set; }
    private uint _hasShoulder;
    public bool HasShoulder
    {
        get => _hasShoulder != 0;
        set => _hasShoulder = value ? 1u : 0u;
    }
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
            data.AddRange(BitConverter.GetBytes(_hasShoulder));
            data.AddRange(BinaryExtensions.GetBytes(Direction));
            data.AddRange(BinaryExtensions.GetBytes(Top));
            data.AddRange(BinaryExtensions.GetBytes(Bottom));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) * 3;

    public RoadDataSegmentChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), br.ReadVector3(), br.ReadVector3(), br.ReadVector3())
    {
    }

    public RoadDataSegmentChunk(string name, uint type, uint lanes, bool hasShoulder, Vector3 direction, Vector3 top, Vector3 bottom) : this(name, type, lanes, hasShoulder ? 1u : 0u, direction, top, bottom)
    {
    }

    public RoadDataSegmentChunk(string name, uint type, uint lanes, uint hasShoulder, Vector3 direction, Vector3 top, Vector3 bottom) : base(ChunkID, name)
    {
        Type = type;
        Lanes = lanes;
        _hasShoulder = hasShoulder;
        Direction = direction;
        Top = top;
        Bottom = bottom;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Type);
        bw.Write(Lanes);
        bw.Write(_hasShoulder);
        bw.Write(Direction);
        bw.Write(Top);
        bw.Write(Bottom);
    }

    protected override Chunk CloneSelf() => new RoadDataSegmentChunk(Name, Type, Lanes, HasShoulder, Direction, Top, Bottom);
}