using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Road_Segment)]
public class RoadSegmentChunk : NamedChunk
{
    public string RoadDataSegment { get; set; }
    public Matrix4x4 Transform { get; set; }
    public Matrix4x4 Scale { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(RoadDataSegment));
            data.AddRange(BinaryExtensions.GetBytes(Transform));
            data.AddRange(BinaryExtensions.GetBytes(Scale));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(RoadDataSegment).Length + sizeof(float) * 16 + sizeof(float) * 16;

    public RoadSegmentChunk(BinaryReader br) : base((uint)ChunkIdentifier.Road_Segment)
    {
        Name = br.ReadP3DString();
        RoadDataSegment = br.ReadP3DString();
        Transform = br.ReadMatrix4x4();
        Scale = br.ReadMatrix4x4();
    }

    public RoadSegmentChunk(string name, string roadDataSegment, Matrix4x4 transform, Matrix4x4 scale) : base((uint)ChunkIdentifier.Road_Segment)
    {
        Name = name;
        RoadDataSegment = roadDataSegment;
        Transform = transform;
        Scale = scale;
    }

    public override void Validate()
    {
        if (RoadDataSegment == null)
            throw new InvalidDataException($"{nameof(RoadDataSegment)} cannot be null.");
        if (Encoding.UTF8.GetBytes(RoadDataSegment).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(RoadDataSegment)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(RoadDataSegment);
        bw.Write(Transform);
        bw.Write(Scale);
    }
}