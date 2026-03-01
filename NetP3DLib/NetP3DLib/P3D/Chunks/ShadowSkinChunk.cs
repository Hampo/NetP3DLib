using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class ShadowSkinChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Shadow_Skin;
    
    public uint Version { get; set; }
    private string _skeletonName = string.Empty;
    public string SkeletonName
    {
        get => _skeletonName;
        set
        {
            if (_skeletonName == value)
                return;

            _skeletonName = value;
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    // TODO: Calculate from children
    public uint NumVertices { get; set; }
    public uint NumTriangles { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(SkeletonName));
            data.AddRange(BitConverter.GetBytes(NumVertices));
            data.AddRange(BitConverter.GetBytes(NumTriangles));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + BinaryExtensions.GetP3DStringLength(SkeletonName) + sizeof(uint) + sizeof(uint);

    public ShadowSkinChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        SkeletonName = br.ReadP3DString();
        NumVertices = br.ReadUInt32();
        NumTriangles = br.ReadUInt32();
    }

    public ShadowSkinChunk(string name, uint version, string skeletonName, uint numVertices, uint numTriangles) : base(ChunkID)
    {
        Name = name;
        Version = version;
        SkeletonName = skeletonName;
        NumVertices = numVertices;
        NumTriangles = numTriangles;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (!SkeletonName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(SkeletonName), SkeletonName);
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(SkeletonName);
        bw.Write(NumVertices);
        bw.Write(NumTriangles);
    }

    protected override Chunk CloneSelf() => new ShadowSkinChunk(Name, Version, SkeletonName, NumVertices, NumTriangles);
}