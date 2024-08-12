using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Tree_Node_2)]
public class TreeNode2Chunk : Chunk
{
    public enum Axis : sbyte
    {
        None = -1,
        X = 0,
        Y = 1,
        Z = 2,
    }

    public Axis SplitAxis { get; set; }
    public float SplitPosition { get; set; }
    public uint StaticEntityLimit { get; set; }
    public uint StaticPhysEntityLimit { get; set; }
    public uint IntersectEntityLimit { get; set; }
    public uint DynaPhysEntityLimit { get; set; }
    public uint FenceEntityLimit { get; set; }
    public uint RoadSegmentEntityLimit { get; set; }
    public uint PathSegmentEntityLimit { get; set; }
    public uint AnimEntityLimit { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.Add((byte)SplitAxis);
            data.AddRange(BitConverter.GetBytes(SplitPosition));
            data.AddRange(BitConverter.GetBytes(StaticEntityLimit));
            data.AddRange(BitConverter.GetBytes(StaticPhysEntityLimit));
            data.AddRange(BitConverter.GetBytes(IntersectEntityLimit));
            data.AddRange(BitConverter.GetBytes(DynaPhysEntityLimit));
            data.AddRange(BitConverter.GetBytes(FenceEntityLimit));
            data.AddRange(BitConverter.GetBytes(RoadSegmentEntityLimit));
            data.AddRange(BitConverter.GetBytes(PathSegmentEntityLimit));
            data.AddRange(BitConverter.GetBytes(AnimEntityLimit));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(sbyte) + sizeof(float) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public TreeNode2Chunk(BinaryReader br) : base((uint)ChunkIdentifier.Tree_Node_2)
    {
        SplitAxis = (Axis)br.ReadSByte();
        SplitPosition = br.ReadSingle();
        StaticEntityLimit = br.ReadUInt32();
        StaticPhysEntityLimit = br.ReadUInt32();
        IntersectEntityLimit = br.ReadUInt32();
        DynaPhysEntityLimit = br.ReadUInt32();
        FenceEntityLimit = br.ReadUInt32();
        RoadSegmentEntityLimit = br.ReadUInt32();
        PathSegmentEntityLimit = br.ReadUInt32();
        AnimEntityLimit = br.ReadUInt32();
    }

    public TreeNode2Chunk(Axis splitAxis, float splitPosition, uint staticEntityLimit, uint staticPhysEntityLimit, uint intersectEntityLimit, uint dynaPhysEntityLimit, uint fencyEntityLimit, uint roadSegmentEntityLimit, uint pathSegmentEntityLimit, uint animEntityLimit) : base((uint)ChunkIdentifier.Tree_Node_2)
    {
        SplitAxis = splitAxis;
        SplitPosition = splitPosition;
        StaticEntityLimit = staticEntityLimit;
        StaticPhysEntityLimit = staticPhysEntityLimit;
        IntersectEntityLimit = intersectEntityLimit;
        DynaPhysEntityLimit = dynaPhysEntityLimit;
        FenceEntityLimit = fencyEntityLimit;
        RoadSegmentEntityLimit = roadSegmentEntityLimit;
        PathSegmentEntityLimit = pathSegmentEntityLimit;
        AnimEntityLimit = animEntityLimit;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write((sbyte)SplitAxis);
        bw.Write(SplitPosition);
        bw.Write(StaticEntityLimit);
        bw.Write(StaticPhysEntityLimit);
        bw.Write(IntersectEntityLimit);
        bw.Write(DynaPhysEntityLimit);
        bw.Write(FenceEntityLimit);
        bw.Write(RoadSegmentEntityLimit);
        bw.Write(PathSegmentEntityLimit);
        bw.Write(AnimEntityLimit);
    }
}