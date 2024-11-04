using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MultiControllerChunk : NamedChunk
{
    public const uint ChunkID = (uint)ChunkIdentifier.Multi_Controller;
    
    public uint Version { get; set; }
    public float Length { get; set; }
    public float Framerate { get; set; }
    public uint NumTracks => (uint)GetChunksOfType<MultiControllerTracksChunk>().Sum(x => x.NumTracks);

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes(Length));
            data.AddRange(BitConverter.GetBytes(Framerate));
            data.AddRange(BitConverter.GetBytes(NumTracks));

            return [.. data];
        }
    }
    public override uint DataLength => (uint)BinaryExtensions.GetP3DStringBytes(Name).Length + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public MultiControllerChunk(BinaryReader br) : base(ChunkID)
    {
        Name = br.ReadP3DString();
        Version = br.ReadUInt32();
        Length = br.ReadSingle();
        Framerate = br.ReadSingle();
        var numTracks = br.ReadUInt32();
    }

    public MultiControllerChunk(string name, uint version, float length, float framerate) : base(ChunkID)
    {
        Name = name;
        Version = version;
        Length = length;
        Framerate = framerate;
    }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(Length);
        bw.Write(Framerate);
        bw.Write(NumTracks);
    }
}