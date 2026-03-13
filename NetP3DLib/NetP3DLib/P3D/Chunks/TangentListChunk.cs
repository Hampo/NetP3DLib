using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Collections;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using NetP3DLib.P3D.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class TangentListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Tangent_List;

    public uint NumTangents
    {
        get => (uint)(Tangents?.Count ?? 0);
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
        }
    }
    public SizeAwareList<Vector3> Tangents { get; }

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

    public TangentListChunk(EndianAwareBinaryReader br) : this(ListHelper.ReadArray(br.ReadInt32(), br.ReadVector3))
    {
    }

    public TangentListChunk(IList<Vector3> tangents) : base(ChunkID)
    {
        Tangents = CreateSizeAwareList(tangents, Tangents_CollectionChanged);
    }
    
    private void Tangents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(Tangents));

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumTangents)
            yield return new InvalidP3DException(this, $"Num Tangents value {NumTangents} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumTangents);
        foreach (var pos in Tangents)
            bw.Write(pos);
    }

    protected override Chunk CloneSelf() => new NormalListChunk(Tangents);
}
