using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Primitive_Group)]
public class OldPrimitiveGroupChunk : Chunk
{
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
        BiNormal = 1 << 11,
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

    public uint Version { get; set; }
    public string ShaderName { get; set; }
    public PrimitiveTypes PrimitiveType { get; set; }
    public VertexTypes VertexType { get; set; }
    public uint NumVertices { get; set; }
    public uint NumIndices { get; set; }
    public uint NumMatrices { get; set; }

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
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(ShaderName).Length + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    public OldPrimitiveGroupChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Primitive_Group)
    {
        Version = br.ReadUInt32();
        ShaderName = br.ReadP3DString();
        PrimitiveType = (PrimitiveTypes)br.ReadUInt32();
        VertexType = (VertexTypes)br.ReadUInt32();
        NumVertices = br.ReadUInt32();
        NumIndices = br.ReadUInt32();
        NumMatrices = br.ReadUInt32();
    }

    public OldPrimitiveGroupChunk(uint version, string shaderName, PrimitiveTypes primitiveType, VertexTypes vertexType, uint numVertices, uint numIndices, uint numMatrices) : base((uint)ChunkIdentifier.Old_Primitive_Group)
    {
        Version = version;
        ShaderName = shaderName;
        PrimitiveType = primitiveType;
        VertexType = vertexType;
        NumVertices = numVertices;
        NumIndices = numIndices;
        NumMatrices = numMatrices;
    }

    private static readonly Dictionary<uint, VertexTypes> VertexTypeMap = new() {
        { (uint)ChunkIdentifier.Packed_Normal_List, VertexTypes.Normals },
        { (uint)ChunkIdentifier.Normal_List, VertexTypes.Normals },
        { (uint)ChunkIdentifier.Colour_List, VertexTypes.Colours },
        //{ (uint)ChunkIdentifier.Multi_Colour_List, VertexTypes.Colours2 },
        { (uint)ChunkIdentifier.Matrix_List, VertexTypes.Matrices },
        { (uint)ChunkIdentifier.Matrix_Palette, VertexTypes.Matrices },
        { (uint)ChunkIdentifier.Weight_List, VertexTypes.Weights },
        { (uint)ChunkIdentifier.Position_List, VertexTypes.Position },
    };
    /// <summary>
    /// Generates a <see cref="VertexTypes"/> based on <see cref="Chunk.Children"/>.
    /// </summary>
    /// <returns>The correct <see cref="VertexTypes"/> for this chunk.</returns>
    /// <exception cref="InvalidDataException">Throws if there is an invalid number of <see cref="UVListChunk"/> children.</exception>
    public VertexTypes GetVertexType()
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
            throw new InvalidDataException("Old Primitive Groups can only have a maximum of 8 UV Lists.");

        if (uvN > 0)
            vertexType |= (VertexTypes)uvN;

        return vertexType;
    }

    public override void Validate()
    {
        if (ShaderName == null)
            throw new InvalidDataException($"{nameof(ShaderName)} cannot be null.");
        if (Encoding.UTF8.GetBytes(ShaderName).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(ShaderName)} is 255 bytes.");

        var expectedVertextType = GetVertexType() & ~VertexTypes.Position; // Hacky fix for "The Simpsons: Road Rage" as the Position List didn't exist then. "The Simpsons: Hit & Run" hardcodedly adds the Position List type to all Old Primitive Groups.
        if ((VertexType & expectedVertextType) != expectedVertextType)
            throw new InvalidDataException($"The {nameof(VertexType)} \"{VertexType}\" does not match expected value \"{expectedVertextType}\"");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(ShaderName);
        bw.Write((uint)PrimitiveType);
        bw.Write((uint)VertexType);
        bw.Write(NumVertices);
        bw.Write(NumIndices);
        bw.Write(NumMatrices);
    }
}