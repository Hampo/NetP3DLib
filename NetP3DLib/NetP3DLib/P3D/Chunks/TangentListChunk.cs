using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TangentListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Tangent_List;
    
    public uint NumTangents
    {
        get => (uint)Tangents.Count;
        set
        {
            if (value == NumTangents)
                return;

            if (value < NumTangents)
            {
                while (NumTangents > value)
                    Tangents.RemoveAt(Tangents.Count - 1);
            }
            else
            {
                while (NumTangents < value)
                    Tangents.Add(default);
            }
            OnSizeChanged((int)(Size - _cachedSize));
            _cachedSize = Size;
        }
    }
    public List<Vector3> Tangents { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumTangents));
            foreach (var normal in Tangents)
                data.AddRange(BinaryExtensions.GetBytes(normal));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 * NumTangents;

    public TangentListChunk(BinaryReader br) : base(ChunkID)
    {
        var numNormals = br.ReadInt32();
        Tangents = new(numNormals);
        for (var i = 0; i < numNormals; i++)
            Tangents.Add(br.ReadVector3());
    }

    public TangentListChunk(IList<Vector3> tangents) : base(ChunkID)
    {
        Tangents.AddRange(tangents);
    }

    public override IEnumerable<InvalidP3DException> ValidateChunks()
    {
        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumTangents)
            yield return new InvalidP3DException(this, $"Num Tangents value {NumTangents} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");

        foreach (var error in base.ValidateChunks())
            yield return error;
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumTangents);
        foreach (var pos in Tangents)
            bw.Write(pos);
    }

    protected override Chunk CloneSelf() => new NormalListChunk(Tangents);
}