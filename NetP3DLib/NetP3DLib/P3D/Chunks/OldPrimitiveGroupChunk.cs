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
public class OldPrimitiveGroupChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Old_Primitive_Group;
    
    public enum PrimitiveTypes : uint
    {
        TriangleList,
        TriangleStrip,
        LineList,
        LineStrip,
        Points,
    }

    [Flags]
    public enum VertexTypes : uint
    {
        None = 0,
        UVs = 1,
        UVs2 = 2,
        UVs3 = 3,
        UVs4 = 4,
        UVs5 = 5,
        UVs6 = 6,
        UVs7 = 7,
        UVs8 = 8,
        Normals = 1 << 4,
        Colours = 1 << 5,
        Specular = 1 << 6,
        Matrices = 1 << 7,
        Weights = 1 << 8,
        Size = 1 << 9,
        W = 1 << 10,
        Binormal = 1 << 11,
        Tangent = 1 << 12,
        Position = 1 << 13,
        Colours2 = 1 << 14,
        ColourCount1 = 1 << 15,
        ColourCount2 = 2 << 15,
        ColourCount3 = 3 << 15,
        ColourCount4 = 4 << 15,
        ColourCount5 = 5 << 15,
        ColourCount6 = 6 << 15,
        ColourCount7 = 7 << 15,
        ColourMaskOffset = 15,
    }

    private static readonly Dictionary<uint, VertexTypes> VertexTypeMap = new() {
        { (uint)ChunkIdentifier.Packed_Normal_List, VertexTypes.Normals },
        { (uint)ChunkIdentifier.Normal_List, VertexTypes.Normals },
        { (uint)ChunkIdentifier.Colour_List, VertexTypes.Colours },
        { (uint)ChunkIdentifier.Multi_Colour_List, VertexTypes.Colours2 },
        { (uint)ChunkIdentifier.Matrix_List, VertexTypes.Matrices },
        { (uint)ChunkIdentifier.Matrix_Palette, VertexTypes.Matrices },
        { (uint)ChunkIdentifier.Weight_List, VertexTypes.Weights },
        { (uint)ChunkIdentifier.Position_List, VertexTypes.Position },
        { (uint)ChunkIdentifier.Binormal_List, VertexTypes.Binormal },
        { (uint)ChunkIdentifier.Tangent_List, VertexTypes.Tangent },
    };

    [DefaultValue(0)]
    public uint Version { get; set; }
    public string ShaderName { get; set; }
    public PrimitiveTypes PrimitiveType { get; set; }
    public VertexTypes VertexType
    {
        get
        {
            VertexTypes vertexType = VertexTypes.None;

            var uvN = 0;
            foreach (var chunk in Children)
            {
                if (chunk.ID == (uint)ChunkIdentifier.UV_List)
                {
                    uvN++;
                }
                else if (VertexTypeMap.TryGetValue(chunk.ID, out var subType))
                {
                    vertexType |= subType;
                }
            }

            if (uvN > 8)
                throw new InvalidP3DException(this, "Old Primitive Groups can only have a maximum of 8 UV Lists.");

            if (uvN > 0)
                vertexType |= (VertexTypes)uvN;

            return vertexType;
        }
    }
    public uint NumVertices
    {
        get
        {
            foreach (var child in Children)
            {
                switch (child)
                {
                    case PositionListChunk positionList:
                        return positionList.NumPositions;
                    case NormalListChunk normalList:
                        return normalList.NumNormals;
                    case BinormalListChunk binormalList:
                        return binormalList.NumBinormals;
                    case PackedNormalListChunk packedNormalList:
                        return packedNormalList.NumNormals;
                    case ColourListChunk colourList:
                        return colourList.NumColours;
                    case MultiColourListChunk multiColourList:
                        return multiColourList.NumColours;
                    case UVListChunk uvList:
                        return uvList.NumUVs;
                    case WeightListChunk weightList:
                        return weightList.NumWeights;
                    case MatrixListChunk matrixList:
                        return matrixList.NumMatrices;
                    case TangentListChunk tangentList:
                        return tangentList.NumTangents;
                }
            }

            return 0;
        }
    }
    public uint NumIndices => GetFirstChunkOfType<IndexListChunk>()?.NumIndices ?? 0;
    public uint NumMatrices => GetFirstChunkOfType<MatrixPaletteChunk>()?.NumMatrices ?? 0;

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(ShaderName));
            data.AddRange(BitConverter.GetBytes((uint)PrimitiveType));
            data.AddRange(BitConverter.GetBytes((uint)VertexType));
            data.AddRange(BitConverter.GetBytes(NumVertices));
            data.AddRange(BitConverter.GetBytes(NumIndices));
            data.AddRange(BitConverter.GetBytes(NumMatrices));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + BinaryExtensions.GetP3DStringLength(ShaderName) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldPrimitiveGroupChunk(BinaryReader br) : base(ChunkID)
    {
        Version = br.ReadUInt32();
        ShaderName = br.ReadP3DString();
        PrimitiveType = (PrimitiveTypes)br.ReadUInt32();
        var vertexType = (VertexTypes)br.ReadUInt32();
        var numVertices = br.ReadUInt32();
        var numIndices = br.ReadUInt32();
        var numMatrices = br.ReadUInt32();
    }

    public OldPrimitiveGroupChunk(uint version, string shaderName, PrimitiveTypes primitiveType) : base(ChunkID)
    {
        Version = version;
        ShaderName = shaderName;
        PrimitiveType = primitiveType;
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (!ShaderName.IsValidP3DString())
            yield return new InvalidP3DStringException(this, nameof(ShaderName), ShaderName);

        if (GetChildCount(ChunkIdentifier.Matrix_Palette) != 0)
        {
            switch (ParentChunk)
            {
                case MeshChunk:
                    yield return new InvalidP3DException(this, "Old Primitive Group chunks cannot have a Matrix Palette if the parent chunk is a Mesh.");
                    break;
                case SkinChunk skinChunk:
                    if (FindNamedChunkInParentHierarchy<SkeletonChunk>(skinChunk.SkeletonName) == null)
                        yield return new InvalidP3DException(this, $"Could not find the skeleton named \"{skinChunk.SkeletonName}\" in the parent hierarchy. This is required when the Old Primitive Group has a Matrix Palette.");

                    break;
            }
        }

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(ShaderName);
        bw.Write((uint)PrimitiveType);
        bw.Write((uint)VertexType);
        bw.Write(NumVertices);
        bw.Write(NumIndices);
        bw.Write(NumMatrices);
    }

    protected override Chunk CloneSelf() => new OldPrimitiveGroupChunk(Version, ShaderName, PrimitiveType);

    public override string ToString() => $"\"{ShaderName}\" ({GetChunkType(this)} (0x{ID:X}))";
}
