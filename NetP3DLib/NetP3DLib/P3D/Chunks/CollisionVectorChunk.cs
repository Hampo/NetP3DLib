using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CollisionVectorChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Collision_Vector;

    private Vector3 _vector;
    public Vector3 Vector
    {
        get => _vector;
        set
        {
            if (_vector == value)
                return;
    
            _vector = value;
            OnPropertyChanged(nameof(Vector));
        }
    }
    

    public override byte[] DataBytes
    {
        get
        {
            var data = new List<byte>((int)DataLength);

            data.AddRange(BinaryExtensions.GetBytes(Vector));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(float) * 3;

    public CollisionVectorChunk(EndianAwareBinaryReader br) : this(br.ReadVector3())
    {
    }

    public CollisionVectorChunk(Vector3 vector) : base(ChunkID)
    {
        _vector = vector;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.Write(Vector);
    }

    protected override Chunk CloneSelf() => new CollisionVectorChunk(Vector);

    public override string ToString()
    {
        var name = ParentChunk switch
        {
            CollisionSphereChunk => IndexInParent switch
            {
                0 => $"Centre",
                _ => string.Empty
            },
            CollisionCylinderChunk => IndexInParent switch
            {
                0 => $"Centre",
                1 => $"Orientation",
                _ => string.Empty
            },
            CollisionOrientedBoundingBoxChunk => IndexInParent switch
            {
                0 => $"Centre",
                1 => $"Rotation Matrix X",
                2 => $"Rotation Matrix Y",
                3 => $"Rotation Matrix Z",
                _ => string.Empty
            },
            CollisionWallChunk => IndexInParent switch
            {
                0 => $"Position",
                1 => $"Normal Ref",
                _ => string.Empty
            },
            _ => string.Empty,
        };

        if (name.Length == 0)
            return base.ToString();

        var sb = new StringBuilder($"{name}");

        if (IncludeChunkTypeInString)
            sb.Append($" ({ChunkType}");

        if (IncludeChunkIDInString)
            sb.Append($" (0x{ID:X})");

        if (IncludeChunkTypeInString)
            sb.Append(')');

        return sb.ToString();
    }
}
