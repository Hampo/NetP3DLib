using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class RailCamChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Rail_Cam;
    
    public enum Behaviours : uint
    {
        Distance = 1,
        Projection,
    }

    public Behaviours Behaviour { get; set; }
    public float MinRadius { get; set; }
    public float MaxRadius { get; set; }
    private uint trackRail;
    public bool TrackRail
    {
        get => trackRail == 1;
        set => trackRail = value ? 1u : 0u;
    }
    public float TrackDist { get; set; }
    private uint reverseSense;
    public bool ReverseSense
    {
        get => reverseSense == 1;
        set => reverseSense = value ? 1u : 0u;
    }
    public float FOV { get; set; }
    public Vector3 TargetOffset { get; set; }
    public Vector3 AxisPlay { get; set; }
    public float PositionLag { get; set; }
    public float TargetLag { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes((uint)Behaviour));
            data.AddRange(BitConverter.GetBytes(MinRadius));
            data.AddRange(BitConverter.GetBytes(MaxRadius));
            data.AddRange(BitConverter.GetBytes(trackRail));
            data.AddRange(BitConverter.GetBytes(TrackDist));
            data.AddRange(BitConverter.GetBytes(reverseSense));
            data.AddRange(BitConverter.GetBytes(FOV));
            data.AddRange(BinaryExtensions.GetBytes(TargetOffset));
            data.AddRange(BinaryExtensions.GetBytes(AxisPlay));
            data.AddRange(BitConverter.GetBytes(PositionLag));
            data.AddRange(BitConverter.GetBytes(TargetLag));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(uint) + sizeof(float) + sizeof(uint) + sizeof(float) + sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) + sizeof(float);

    public RailCamChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Behaviour = (Behaviours)br.ReadUInt32();
        MinRadius = br.ReadSingle();
        MaxRadius = br.ReadSingle();
        trackRail = br.ReadUInt32();
        TrackDist = br.ReadSingle();
        reverseSense = br.ReadUInt32();
        FOV = br.ReadSingle();
        TargetOffset = br.ReadVector3();
        AxisPlay = br.ReadVector3();
        PositionLag = br.ReadSingle();
        TargetLag = br.ReadSingle();
    }

    public RailCamChunk(string name, Behaviours behaviour, float minRadius, float maxRadius, bool trackRail, float trackDist, bool reverseSense, float fov, Vector3 targetOffset, Vector3 axisPlay, float positionLag, float targetLag) : base(ChunkID)
    {
        Name = name;
        Behaviour = behaviour;
        MinRadius = minRadius;
        MaxRadius = maxRadius;
        TrackRail = trackRail;
        TrackDist = trackDist;
        ReverseSense = reverseSense;
        FOV = fov;
        TargetOffset = targetOffset;
        AxisPlay = axisPlay;
        PositionLag = positionLag;
        TargetLag = targetLag;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write((uint)Behaviour);
        bw.Write(MinRadius);
        bw.Write(MaxRadius);
        bw.Write(trackRail);
        bw.Write(TrackDist);
        bw.Write(reverseSense);
        bw.Write(FOV);
        bw.Write(TargetOffset);
        bw.Write(AxisPlay);
        bw.Write(PositionLag);
        bw.Write(TargetLag);
    }

    protected override Chunk CloneSelf() => new RailCamChunk(Name, Behaviour, MinRadius, MaxRadius, TrackRail, TrackDist, ReverseSense, FOV, TargetOffset, AxisPlay, PositionLag, TargetLag);
}