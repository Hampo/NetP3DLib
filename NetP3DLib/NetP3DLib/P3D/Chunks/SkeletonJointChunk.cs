using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class SkeletonJointChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Skeleton_Joint;

    private uint _parent;
    public uint Parent
    {
        get => _parent;
        set
        {
            if (_parent == value)
                return;
    
            _parent = value;
            OnPropertyChanged(nameof(Parent));
        }
    }
    
    private int _dOF;
    public int DOF
    {
        get => _dOF;
        set
        {
            if (_dOF == value)
                return;
    
            _dOF = value;
            OnPropertyChanged(nameof(DOF));
        }
    }
    
    private int _freeAxis;
    public int FreeAxis
    {
        get => _freeAxis;
        set
        {
            if (_freeAxis == value)
                return;
    
            _freeAxis = value;
            OnPropertyChanged(nameof(FreeAxis));
        }
    }
    
    private int _primaryAxis;
    public int PrimaryAxis
    {
        get => _primaryAxis;
        set
        {
            if (_primaryAxis == value)
                return;
    
            _primaryAxis = value;
            OnPropertyChanged(nameof(PrimaryAxis));
        }
    }
    
    private int _secondaryAxis;
    public int SecondaryAxis
    {
        get => _secondaryAxis;
        set
        {
            if (_secondaryAxis == value)
                return;
    
            _secondaryAxis = value;
            OnPropertyChanged(nameof(SecondaryAxis));
        }
    }
    
    private int _twistAxis;
    public int TwistAxis
    {
        get => _twistAxis;
        set
        {
            if (_twistAxis == value)
                return;
    
            _twistAxis = value;
            OnPropertyChanged(nameof(TwistAxis));
        }
    }
    
    private Matrix4x4 _restPose;
    public Matrix4x4 RestPose
    {
        get => _restPose;
        set
        {
            if (_restPose == value)
                return;
    
            _restPose = value;
            OnPropertyChanged(nameof(RestPose));
        }
    }
    

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Parent));
            data.AddRange(BitConverter.GetBytes(DOF));
            data.AddRange(BitConverter.GetBytes(FreeAxis));
            data.AddRange(BitConverter.GetBytes(PrimaryAxis));
            data.AddRange(BitConverter.GetBytes(SecondaryAxis));
            data.AddRange(BitConverter.GetBytes(TwistAxis));
            data.AddRange(BinaryExtensions.GetBytes(RestPose));

            return [.. data];
        }
    }
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(float) * 16;

    public SkeletonJointChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadMatrix4x4())
    {
    }

    public SkeletonJointChunk(string name, uint parent, int dof, int freeAxis, int primaryAxis, int secondaryAxis, int twistAxis, Matrix4x4 restPose) : base(ChunkID, name)
    {
        _parent = parent;
        _dOF = dof;
        _freeAxis = freeAxis;
        _primaryAxis = primaryAxis;
        _secondaryAxis = secondaryAxis;
        _twistAxis = twistAxis;
        _restPose = restPose;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Parent);
        bw.Write(DOF);
        bw.Write(FreeAxis);
        bw.Write(PrimaryAxis);
        bw.Write(SecondaryAxis);
        bw.Write(TwistAxis);
        bw.Write(RestPose);
    }

    protected override Chunk CloneSelf() => new SkeletonJointChunk(Name, Parent, DOF, FreeAxis, PrimaryAxis, SecondaryAxis, TwistAxis, RestPose);
}
