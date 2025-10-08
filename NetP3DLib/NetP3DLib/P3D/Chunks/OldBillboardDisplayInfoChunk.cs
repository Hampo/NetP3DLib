using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class OldBillboardDisplayInfoChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Billboard_Display_Info;
    
    [DefaultValue(1)]
    public uint Version { get; set; }
    public Quaternion Rotation { get; set; }
    [MaxLength(4)]
    public string CutOffMode { get; set; }
    public Vector2 UVOffsetRange { get; set; }
    public float SourceRange { get; set; }
    public float EdgeRange { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetBytes(Rotation));
            data.AddRange(BinaryExtensions.GetFourCCBytes(CutOffMode));
            data.AddRange(BinaryExtensions.GetBytes(UVOffsetRange));
            data.AddRange(BitConverter.GetBytes(SourceRange));
            data.AddRange(BitConverter.GetBytes(EdgeRange));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 4 + 4 + sizeof(float) * 2 + sizeof(float) + sizeof(float);

    public OldBillboardDisplayInfoChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Rotation = br.ReadQuaternion();
        CutOffMode = br.ReadFourCC();
        UVOffsetRange = br.ReadVector2();
        SourceRange = br.ReadSingle();
        EdgeRange = br.ReadSingle();
    }

    public OldBillboardDisplayInfoChunk(uint version, Quaternion rotation, string cutOffMode, Vector2 uvOffsetRange, float sourceRange, float edgeRange) : base(ChunkID)
    {
        Version = version;
        Rotation = rotation;
        CutOffMode = cutOffMode;
        UVOffsetRange = uvOffsetRange;
        SourceRange = sourceRange;
        EdgeRange = edgeRange;
    }

    public override void Validate()
    {
        if (!CutOffMode.IsValidFourCC())
            throw new InvalidFourCCException(this,nameof(CutOffMode), CutOffMode);

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.Write(Rotation);
        bw.WriteFourCC(CutOffMode);
        bw.Write(UVOffsetRange);
        bw.Write(SourceRange);
        bw.Write(EdgeRange);
    }

    protected override Chunk CloneSelf() => new OldBillboardDisplayInfoChunk(Version, Rotation, CutOffMode, UVOffsetRange, SourceRange, EdgeRange);
}
