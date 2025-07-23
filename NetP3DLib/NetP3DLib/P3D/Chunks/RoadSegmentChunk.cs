using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class RoadSegmentChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Road_Segment;
    
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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + BinaryExtensions.GetP3DStringLength(RoadDataSegment) + sizeof(float) * 16 + sizeof(float) * 16;

    public RoadSegmentChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        RoadDataSegment = br.ReadP3DString();
        Transform = br.ReadMatrix4x4();
        Scale = br.ReadMatrix4x4();
    }

    public RoadSegmentChunk(string name, string roadDataSegment, Matrix4x4 transform, Matrix4x4 scale) : base(ChunkID)
    {
        Name = name;
        RoadDataSegment = roadDataSegment;
        Transform = transform;
        Scale = scale;
    }

    public override void Validate()
    {
        if (!RoadDataSegment.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(RoadDataSegment), RoadDataSegment);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.WriteP3DString(RoadDataSegment);
        bw.Write(Transform);
        bw.Write(Scale);
    }

    internal override Chunk CloneSelf() => new RoadSegmentChunk(Name, RoadDataSegment, Transform, Scale);
}