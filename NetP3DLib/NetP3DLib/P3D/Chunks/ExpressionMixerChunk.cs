using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ExpressionMixerChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Expression_Mixer;
    
    public uint Version { get; set; }
    public uint Type { get; set; }
    public string TargetName { get; set; }
    public string ExpressionGroupName { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Type));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(TargetName));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ExpressionGroupName));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(TargetName).Length + (uint)BinaryExtensions.GetP3DStringBytes(ExpressionGroupName).Length;

    public ExpressionMixerChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Type = br.ReadUInt32();
        TargetName = br.ReadP3DString();
        ExpressionGroupName = br.ReadP3DString();
    }

    public ExpressionMixerChunk(uint version, string name, uint type, string targetName, string expressionGroupName) : base(ChunkID)
    {
        Version = version;
        Name = name;
        Type = type;
        TargetName = targetName;
        ExpressionGroupName = expressionGroupName;
    }

    public override void Validate()
    {
        if (TargetName == null)
            throw new InvalidDataException($"{nameof(TargetName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(TargetName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(TargetName)} is 255 bytes.");

        if (ExpressionGroupName == null)
            throw new InvalidDataException($"{nameof(ExpressionGroupName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(ExpressionGroupName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(ExpressionGroupName)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.Write(Type);
        bw.WriteP3DString(TargetName);
        bw.WriteP3DString(ExpressionGroupName);
    }
}