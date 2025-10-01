using NetP3DLib.P3D.Attributes;
using NetP3DLib.P3D.Enums;
using NetP3DLib.P3D.Exceptions;
using NetP3DLib.P3D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes(ChunkID)]
public class MultiControllerTracksChunk : Chunk
{
    public const ChunkIdentifier ChunkID = ChunkIdentifier.Multi_Controller_Tracks;
    
    public uint NumTracks
    {
        get => (uint)Tracks.Count;
        set
        {
            if (value == NumTracks)
                return;

            if (value < NumTracks)
            {
                while (NumTracks > value)
                    Tracks.RemoveAt(Tracks.Count - 1);
            }
            else
            {
                while (NumTracks < value)
                    Tracks.Add(new());
            }
        }
    }
    public List<Track> Tracks { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(NumTracks));
            foreach (var track in Tracks)
                data.AddRange(track.DataBytes);

            return [.. data];
        }
    }
    public override uint DataLength
    {
        get
        {
            uint size = sizeof(uint);
            foreach (var track in Tracks)
                size += track.DataLength;
            return size;
        }
    }

    public MultiControllerTracksChunk(BinaryReader br) : base(ChunkID)
    {
        var numTracks = br.ReadInt32();
        Tracks = new(numTracks);
        for (int i = 0; i < numTracks; i++)
            Tracks.Add(new(br));
    }

    public MultiControllerTracksChunk(IList<Track> tracks) : base(ChunkID)
    {
        Tracks.AddRange(tracks);
    }

    public override void Validate()
    {
        foreach (var track in Tracks)
            track.Validate();

        base.Validate();
    }

    protected override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumTracks);
        foreach (var track in Tracks)
            track.Write(bw);
    }

    protected override Chunk CloneSelf()
    {
        var tracks = new List<Track>(Tracks.Count);
        foreach (var track in Tracks)
            tracks.Add(track.Clone());
        return new MultiControllerTracksChunk(tracks);
    }

    public class Track
    {
        public string Name { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float Scale { get; set; }

        public byte[] DataBytes
        {
            get
            {
                List<byte> data = [];

                data.AddRange(BinaryExtensions.GetP3DStringBytes(Name));
                data.AddRange(BitConverter.GetBytes(StartTime));
                data.AddRange(BitConverter.GetBytes(EndTime));
                data.AddRange(BitConverter.GetBytes(Scale));

                return [.. data];
            }
        }

        public uint DataLength => BinaryExtensions.GetP3DStringLength(Name) + sizeof(float) + sizeof(float) + sizeof(float);

        public Track(BinaryReader br)
        {
            Name = br.ReadP3DString();
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();
            Scale = br.ReadSingle();
        }

        public Track(string name, float startTime, float endTime, float scale)
        {
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
            Scale = scale;
        }

        public Track()
        {
            Name = string.Empty;
            StartTime = 0;
            EndTime = 0;
            Scale = 0;
        }

        public void Validate()
        {
            if (!Name.IsValidP3DString())
                throw new InvalidP3DStringException(nameof(Name), Name);
        }

        internal void Write(BinaryWriter bw)
        {
            bw.WriteP3DString(Name);
            bw.Write(StartTime);
            bw.Write(EndTime);
            bw.Write(Scale);
        }

        internal Track Clone() => new(Name, StartTime, EndTime, Scale);

        public override string ToString() => $"{Name} | {StartTime} | {EndTime} | {Scale}";
    }
}