using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.History)]
public class HistoryChunk : Chunk
{
    public ushort NumHistory
    {
        get => (ushort)History.Count;
        set
        {
            if (value == NumHistory)
                return;

            if (value < NumHistory)
            {
                while (NumHistory > value)
                    History.RemoveAt(History.Count - 1);
            }
            else
            {
                while (NumHistory < value)
                    History.Add(string.Empty);
            }
        }
    }
    public List<string> History { get; } = [];

    public override byte[] DataBytes
    {
        get
        {
            List<byte> data = [];
            
            data.AddRange(BitConverter.GetBytes(NumHistory));
            foreach (string item in History)
                data.AddRange(BinaryExtensions.GetP3DStringBytes(item));

            return [.. data];
        }
    }
    public override uint DataLength => sizeof(ushort) + (uint)History.Sum(x => BinaryExtensions.GetP3DStringBytes(x).Length);

    public HistoryChunk(BinaryReader br) : base((uint)ChunkIdentifier.History)
    {
        ushort lineCount = br.ReadUInt16();
        History.Capacity = lineCount;
        for (int i = 0; i < lineCount; i++)
            History.Add(br.ReadP3DString());
    }

    public HistoryChunk(IList<string> history) : base((uint)ChunkIdentifier.History)
    {
        if (history.Any(x => x == null || x.Length > 255))
            throw new ArgumentException("All history lines must have a value, with a max length of 255 bytes.");

        History.AddRange(history);
    }

    public override void Validate()
    {
        if (History.Count > ushort.MaxValue)
            throw new InvalidDataException($"The max number of history lines is {ushort.MaxValue}.");

        if (History.Any(x => x == null || x.Length > 255))
            throw new InvalidDataException("All history lines must have a value, with a max length of 255 bytes.");

        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    {
        bw.Write(NumHistory);
        foreach (string item in History)
            bw.WriteP3DString(item);
    }
}
