using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExpressionMixerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Expression_Mixer;

    public enum MixerType : uint
    {
        Undefined,
        Pose,
        HSplineOffset,
        VertexOffset,
    }
    
    [DefaultValue(0)]
    public uint Version { get; set; }
    public MixerType Type { get; set; }
    public string TargetName { get; set; }
    public string ExpressionGroupName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes((uint)Type));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(TargetName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ExpressionGroupName));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(TargetName) + BinaryExtensions.GetP3DStringLength(ExpressionGroupName);

    public ExpressionMixerChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = (MixerType)br.ReadUInt32();
        TargetName = br.ReadP3DString();
        ExpressionGroupName = br.ReadP3DString();
    }

    public ExpressionMixerChunk(uint version, string name, MixerType type, string targetName, string expressionGroupName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Type = type;
        TargetName = targetName;
        ExpressionGroupName = expressionGroupName;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!TargetName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(TargetName), TargetName);

        if (!ExpressionGroupName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ExpressionGroupName), ExpressionGroupName);

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write((uint)Type);
        bw.WriteP3DString(TargetName);
        bw.WriteP3DString(ExpressionGroupName);
    }

    protected override Chunk CloneSelf() => new ExpressionMixerChunk(Version, Name, Type, TargetName, ExpressionGroupName);
}
