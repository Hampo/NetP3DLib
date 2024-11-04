using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class CameraChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Camera;
    
    public uint Version { get; set; }
    public float FOV { get; set; }
    public float AspectRatio { get; set; }
    public float NearClip { get; set; }
    public float FarClip { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Look { get; set; }
    public Vector3 Up { get; set; }

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(FOV));
            data.AddRange(BitConverter.GetBytes(AspectRatio));
            data.AddRange(BitConverter.GetBytes(NearClip));
            data.AddRange(BitConverter.GetBytes(FarClip));
            data.AddRange(BinaryExtensions.GetBytes(Position));
            data.AddRange(BinaryExtensions.GetBytes(Look));
            data.AddRange(BinaryExtensions.GetBytes(Up));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) + sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) * 3;

    public CameraChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        FOV = br.ReadSingle();
        AspectRatio = br.ReadSingle();
        NearClip = br.ReadSingle();
        FarClip = br.ReadSingle();
        Position = br.ReadVector3();
        Look = br.ReadVector3();
        Up = br.ReadVector3();
    }

    public CameraChunk(string name, uint version, float fov, float aspectRatio, float nearClip, float farClip, Vector3 position, Vector3 look, Vector3 up) : base(ChunkID)
    {
        Name = name;
        Version = version;
        FOV = fov;
        AspectRatio = aspectRatio;
        NearClip = nearClip;
        FarClip = farClip;
        Position = position;
        Look = look;
        Up = up;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(FOV);
        bw.Write(AspectRatio);
        bw.Write(NearClip);
        bw.Write(FarClip);
        bw.Write(Position);
        bw.Write(Look);
        bw.Write(Up);
    }
}