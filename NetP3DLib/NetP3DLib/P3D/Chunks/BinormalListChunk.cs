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
public class BinormalListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Binormal_List;
    
    public uint NumBinormals
    {
        get => (uint)Binormals.Count;
        set
        {
            if (value == NumBinormals)
                return;

            if (value < NumBinormals)
            {
                while (NumBinormals > value)
                    Binormals.RemoveAt(Binormals.Count - 1);
            }
            else
            {
                while (NumBinormals < value)
                    Binormals.Add(default);
            }
        }
    }
    public List<Vector3> Binormals { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumBinormals));
            foreach (var normal in Binormals)
                data.AddRange(BinaryExtensions.GetBytes(normal));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(float) * 3 * NumBinormals;

    public BinormalListChunk(BinaryReader br) : base(ChunkID)
    {
        var numNormals = br.ReadInt32();
        Binormals = new(numNormals);
        for (var i = 0; i < numNormals; i++)
            Binormals.Add(br.ReadVector3());
    }

    public BinormalListChunk(IList<Vector3> binormals) : base(ChunkID)
    {
        Binormals.AddRange(binormals);
    }

    public override void Validate()
    {
        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumBinormals)
            throw new InvalidP3DException($"Num Binormals value {NumBinormals} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumBinormals);
        foreach (var pos in Binormals)
            bw.Write(pos);
    }

    protected override Chunk CloneSelf() => new NormalListChunk(Binormals);
}