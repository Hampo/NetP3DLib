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
public class UVListChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.UV_List;

    public uint NumUVs
    {
        get => (uint)(UVs?.Count ?? 0);
        set
        {
            if (value == NumUVs)
                return;

            if (value < NumUVs)
            {
                while (NumUVs > value)
                    UVs.RemoveAt(UVs.Count - 1);
            }
            else
            {
                while (NumUVs < value)
                    UVs.Add(default);
            }
        }
    }
    
    private uint _channel;
    public uint Channel
    {
        get => _channel;
        set
        {
            if (_channel == value)
                return;
    
            _channel = value;
            OnPropertyChanged(nameof(Channel));
        }
    }
    
    public SizeAwareList<Vector2> UVs { get; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumUVs));
            data.AddRange(BitConverter.GetBytes(Channel));
            foreach (var uv in UVs)
                data.AddRange(BinaryExtensions.GetBytes(uv));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + sizeof(uint) + sizeof(float) * 2 * NumUVs;

    public UVListChunk(EndianAwareBinaryReader br) : this(ReadChannel(br, out var numUVs), ListHelper.ReadArray(numUVs, br.ReadVector2))
    {
    }

    private static uint ReadChannel(EndianAwareBinaryReader br, out int numUVs)
    {
        numUVs = br.ReadInt32();
        return br.ReadUInt32();
    }

    public UVListChunk(uint channel, IList<Vector2> uvs) : base(ChunkID)
    {
        _channel = channel;
        UVs = CreateSizeAwareList(uvs, UVs_CollectionChanged);
    }
    
    private void UVs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(UVs));

    public override IEnumerable<InvalidP3DException> ValidateChunk()
    {
        foreach (var error in base.ValidateChunk())
            yield return error;

        if (ParentChunk is OldPrimitiveGroupChunk oldPrimitiveGroup && oldPrimitiveGroup.NumVertices != NumUVs)
            yield return new InvalidP3DException(this, $"Num UVs value {NumUVs} does not match parent Num Vertices value {oldPrimitiveGroup.NumVertices}.");
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(NumUVs);
        bw.Write(Channel);
        foreach (var pos in UVs)
            bw.Write(pos);
    }

    protected override Chunk CloneSelf() => new UVListChunk(Channel, UVs);
}
