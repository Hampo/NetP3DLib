using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CompositeDrawableEffectChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Composite_Drawable_Effect;

    private uint _isTranslucent;
    public bool IsTranslucent
    {
        get => _isTranslucent != 0;
        set
        {
            if (IsTranslucent == value)
                return;

            _isTranslucent = value ? 1u : 0u;
            OnPropertyChanged(nameof(IsTranslucent));
        }
    }
    
    private uint _skeletonJointId;
    public uint SkeletonJointId
    {
        get => _skeletonJointId;
        set
        {
            if (_skeletonJointId == value)
                return;
    
            _skeletonJointId = value;
            OnPropertyChanged(nameof(SkeletonJointId));
        }
    }
    

    public override byte[] DataBytes
    {
        get
        {
            var data = new List<byte>((int)DataLength);

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(_isTranslucent));
            data.AddRange(BitConverter.GetBytes(SkeletonJointId));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(uint);

    public CompositeDrawableEffectChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), br.ReadUInt32())
    {
    }

    public CompositeDrawableEffectChunk(string name, bool isTranslucent, uint skeletonJointId) : this(name, isTranslucent ? 1u : 0u, skeletonJointId)
    {
    }

    public CompositeDrawableEffectChunk(string name, uint isTranslucent, uint skeletonJointId) : base(ChunkID, name)
    {
        _isTranslucent = isTranslucent;
        _skeletonJointId = skeletonJointId;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(_isTranslucent);
        bw.Write(SkeletonJointId);
    }

    protected override Chunk CloneSelf() => new CompositeDrawableEffectChunk(Name, IsTranslucent, SkeletonJointId);
}
