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
    public string SkeletonName { get; set; }
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

    public override void Validate()
    {
        if (!SkeletonName.IsValidP3DString())
            throw new InvalidP3DStringException(nameof(SkeletonName), SkeletonName);

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.WriteP3DString(SkeletonName);
        bw.Write(NumVertices);
        bw.Write(NumTriangles);
    }
}