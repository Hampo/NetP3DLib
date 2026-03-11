using NetP3DLib.IO;
using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MultiControllerChunk : NamedChunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Multi_Controller;

    [DefaultValue(0)]
    public uint Version { get; set; }
    public float Length { get; set; }
    public float Framerate { get; set; }
    public uint NumTracks
    {
        get
        {
            uint numTracks = 0;
            foreach (var child in Children)
                if (child is MultiControllerTracksChunk multiControllerTracksChunk)
                    numTracks += multiControllerTracksChunk.NumTracks;
            return numTracks;
        }
    }

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
    public override uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(uint) + sizeof(float) + sizeof(float) + sizeof(uint);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "We want to read the value to progress the BinaryReader, but not set the value anywhere because it's calculated dynamically.")]
    public MultiControllerChunk(EndianAwareBinaryReader br) : this(br.ReadP3DString(), br.ReadUInt32(), br.ReadSingle(), br.ReadSingle())
    {
        var numTracks = br.ReadUInt32();
    }

    public MultiControllerChunk(string name, uint version, float length, float framerate) : base(ChunkID, name)
    {
        Version = version;
        Length = length;
        Framerate = framerate;
    }

    protected override void WriteData(EndianAwareBinaryWriter bw)
    {
        bw.WriteP3DString(Name);
        bw.Write(Version);
        bw.Write(Length);
        bw.Write(Framerate);
        bw.Write(NumTracks);
    }

    protected override Chunk CloneSelf() => new MultiControllerChunk(Name, Version, Length, Framerate);
}
