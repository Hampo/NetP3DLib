using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Billboard_Quad_Group)]
public class OldBillboardQuadGroupChunk : NamedChunk
{
    public uint Version { get; set; }
    public string Shader { get; set; }
    public uint ZTest { get; set; }
    public uint ZWrite { get; set; }
    public uint Fog { get; set; }
    public uint NumQuads => (uint)Children.Where(x => x.ID == (uint)ChunkIdentifier.Old_Billboard_Quad).Count();

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BinaryExtensions.GetP3DStringBytes(Shader));
            data.AddRange(BitConverter.GetBytes(ZTest));
            data.AddRange(BitConverter.GetBytes(ZWrite));
            data.AddRange(BitConverter.GetBytes(Fog));
            data.AddRange(BitConverter.GetBytes(NumQuads));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(uint) + (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + (uint)BinaryExtensions.GetP3DStringBytes(Shader).Length + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public OldBillboardQuadGroupChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Billboard_Quad_Group)
    {
        Version = br.ReadUInt32();
        Name = br.ReadP3DString();
        Shader = br.ReadP3DString();
        ZTest = br.ReadUInt32();
        ZWrite = br.ReadUInt32();
        Fog = br.ReadUInt32();
        var numQuads = br.ReadUInt32();
    }

    public OldBillboardQuadGroupChunk(uint version, string name, string shader, uint zTest, uint zWrite, uint fog) : base((uint)ChunkIdentifier.Old_Billboard_Quad_Group)
    {
        Version = version;
        Name = name;
        Shader = shader;
        ZTest = zTest;
        ZWrite = zWrite;
        Fog = fog;
    }

    public override void Validate()
    {
        if (Shader == null)
            throw new InvalidDataException($"{nameof(Shader)} cannot be null.");
        if (Encoding.UTF8.GetBytes(Shader).Length > 255)
            throw new InvalidDataException($"The max length of {nameof(Shader)} is 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(Version);
        bw.WriteP3DString(Name);
        bw.WriteP3DString(Shader);
        bw.Write(ZTest);
        bw.Write(ZWrite);
        bw.Write(Fog);
        bw.Write(NumQuads);
    }
}